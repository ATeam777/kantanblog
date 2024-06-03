using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KantanBlog001.Data;
using KantanBlog001.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace KantanBlog001.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comment/Index
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comment.Include(c => c.Article);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comment/CommentView
        public async Task<IActionResult> CommentView(int? articleId)
        {

            IIncludableQueryable<Comment, Article> applicationDbContext;
            //IQueryable<Comment> comment;
            ViewData["ArticleId"] = articleId;

            if (articleId == null)
            {
                //記事IDがない場合、全件表示
                applicationDbContext = _context.Comment.Include(c => c.Article);
                return View(await applicationDbContext.OrderByDescending(o => o.Create_Time).ToListAsync());
            }
            else
            {
                //記事IDがある場合、該当コメント表示
                applicationDbContext = _context.Comment.Include(c => c.Article);
                return View(
                    await applicationDbContext.Where(a => a.ArticleId == articleId)
                            .OrderByDescending(o => o.Create_Time).ToListAsync()
                    );

            }

            //var applicationDbContext = _context.Comment.Include(c => c.Article);
            //return View(await applicationDbContext.ToListAsync());

        }

        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Article)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        // GET: Comment/CommentCreate
        public IActionResult CommentCreate(int? articleId)
        {

            if (articleId == null)
            {
                return NotFound();
            }

            ViewData["ArticleId"] = articleId;
            return View();

        }

        // POST: Comment/CommentCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentCreate(
            [Bind("CommentId,ArticleId,UserName,CommentText,Create_Time,Update_Time")]
            Comment comment)
        {
            if (!ModelState.IsValid)
            {
                comment.Create_Time = DateTime.Now;
                comment.Update_Time = DateTime.Now;

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("ArticleView", "Articles", new { id = comment.ArticleId });
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, "ArticleId", "Title");
            return View(comment);
        }
        // GET: Comment/Create
        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.Article, "ArticleId", "ArticleId");
            return View();
        }

        // POST: Comment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CommentId,ArticleId,UserName,CommentText,Create_Time,Update_Time")]
            Comment comment)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, "ArticleId", "Title");
            return View(comment);
        }

        // GET: Comment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, "ArticleId", "Text", comment.ArticleId);
            return View(comment);
        }

        // POST: Comment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,ArticleId,UserName,CommentText,Create_Time,Update_Time")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, "ArticleId", "Text", comment.ArticleId);
            return View(comment);
        }

        // GET: Comment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Article)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                _context.Comment.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.CommentId == id);
        }
    }
}
