using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KantanBlog001.Data;
using KantanBlog001.Models;
using KantanBlog001.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace KantanBlog001.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Articles/Delete/5
        [Authorize]
        public async Task<IActionResult> ArticleDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }
        // POST: Articles/Delete/5
        [Authorize]
        [HttpPost, ActionName("ArticleDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleDeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                _context.Article.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BloggerArticleList));
        }

        // GET: Articles/UserArticleList
        // 閲覧者：記事一覧
        //川村：ユーザー専用ページ用のメソッドを追加
        public async Task<IActionResult> UserArticleList(
            int? pageNumber, string? category, string? date, string sortOrder)
        {

            //既定の並べ替え順序は昇順("Date"と同じ)
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            // IQueryable<T> を取得
            // 全件検索
            IQueryable<Article> articles = _context.Article.AsQueryable();

            if (category != null)
            {
                //カテゴリーが存在する場合、カテゴリ検索を行う
                articles = articles.Where(a => a.Category == category);
            }

            //日付が存在する場合、該当日付範囲で検索する
            if (date != null)
            {
                //日付
                DateTime startDate = DateTime.Parse(date);
                DateTime endDate = DateTime.Parse(date + " 23:59:59");

                //日付検索
                articles = articles.Where(s => s.Create_Time >= startDate && s.Create_Time <= endDate);
            }

            switch (sortOrder)
            {
                case "Date":
                    articles = articles.OrderBy(m => m.Create_Time);
                    break;
                case "date_desc":
                    articles = articles.OrderByDescending(m => m.Create_Time);
                    break;
                default:
                    articles = articles.OrderByDescending(m => m.Create_Time);
                    break;
            }

            int pageSize = 5; // 1ページに表示するアイテム数

            // ページ番号に基づいて記事を取得
            var paginatedArticles = await PaginatedList<Article>.CreateAsync(articles.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(paginatedArticles); // ページネーションされた記事一覧をビューに渡す
        }

        // GET: Articles/BloggerArticleList
        // ブロガー管理者：記事一覧
        // GET: Articles
        [Authorize]
        public async Task<IActionResult> BloggerArticleList(int? pageNumber, string sortOrder)
        {
            //既定の並べ替え順序は昇順("Date"と同じ)
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            
            var articles = _context.Article
                .AsQueryable().Where(a => a.Email == User.Identity.Name);

            switch (sortOrder)
            {
                case "Date":
                    articles = articles.OrderBy(m => m.Create_Time);
                    break;
                case "date_desc":
                    articles = articles.OrderByDescending(m => m.Create_Time);
                    break;
                default:
                    articles = articles.OrderByDescending(m => m.Create_Time);
                    break;
            }
            //ここでページの表示数を変更できる
            int pageSize = 5; // 1ページに表示するアイテム数
            // ページ番号に基づいて記事を取得
            var paginatedArticles = await PaginatedList<Article>.CreateAsync(articles.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(paginatedArticles); // ページネーションされた記事一覧をビューに渡す
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/ArticleCreate
        [Authorize]
        public IActionResult ArticleCreate()
        {
            return View();
        }

        // POST: Articles/ArticleCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleCreate(
            [Bind("ArticleId,Title,Category,Text,Create_Time,Update_Time,Email")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.Create_Time = DateTime.Now;
                article.Update_Time = DateTime.Now;

                if(User.Identity.Name != null) article.Email = User.Identity.Name;

                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(BloggerArticleList));
            }
            return View(article);
        }
        // GET: Articles/ArticleEdit/5
        [Authorize]
        public async Task<IActionResult> ArticleEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/ArticleEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleEdit(int id, [Bind("ArticleId," +
            "InputDateTime,Title,Category,Text,Update_Time")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing article from the database
                    var existingArticle = await _context.Article.AsNoTracking().FirstOrDefaultAsync(a => a.ArticleId == id);

                    if (existingArticle == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing article with the new values
                    //existingArticle.InputDateTime = article.InputDateTime;
                    existingArticle.Title = article.Title;
                    existingArticle.Category = article.Category;
                    existingArticle.Text = article.Text;
                    existingArticle.Update_Time = DateTime.Now;

                    // Update the existing article in the database
                    _context.Entry(existingArticle).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(BloggerArticleList));
            }
            return View(article);
        }

        // GET: Articles/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId," +
            "InputDateTime,Title,Category,Text,Create_Time,Update_Time")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    article.Update_Time = DateTime.Now;

                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(BloggerArticleList));
            }
            return View(article);
        }

        // GET: Articles/ArticleView
        public async Task<IActionResult> ArticleView(int? id)
        {

            if (id == null)
            {
                //IDがNULLの場合、IDのMAX値を取ってくる
                id = await _context.Article.MaxAsync(a => a.ArticleId);
            }

            //記事取得
            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            //カテゴリーを集計する
            List<CategoryCount> categoryCount = await _context.Article
                .GroupBy(g => g.Category)
                .OrderBy(o => o.Key)
                .Select(group => new CategoryCount
                {
                    Category = group.Key,
                    CategoryGroupCount = group.Count()
                }).ToListAsync();

            ViewData["categoryCount"] = categoryCount;

            //取得準備
            DateTime dtNow = DateTime.Now;
            DateTime startDate2 = DateTime.Parse(dtNow.Year + "/01/01 00:00:00");
            DateTime endDate2 = DateTime.Parse(dtNow.Year + "/12/31 23:59:59");

            // カレンダーリンク用取得（パラメータは使用しない独自に取得する）
            List<ArticleDate> listDate = await _context.Article
                .Where(s => s.Create_Time >= startDate2 && s.Create_Time <= endDate2)
                //.OrderBy(o => o.Key)
                .Select(r => new ArticleDate
                {
                    date = r.Create_Time.ToString("yyyy-M-d"),
                    link = "/Articles/UserArticleList?date="
                            + r.Create_Time.ToString("yyyy/MM/dd")
                }).ToListAsync();

            // 取得したLISTをJSON文字列に変換
            var json = JsonSerializer.Serialize(listDate);
            ViewData["listDate"] = json;

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                _context.Article.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BloggerArticleList));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.ArticleId == id);
        }
    }
}
