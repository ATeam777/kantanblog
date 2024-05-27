using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KantanBlog001.Models
{
    public class Article
    {

        public Article() 
        {
            Category = "未分類";
            Email = "anonymous@example.com";
        }

        public int ArticleId { get; set; }

        //[Required]
        //public DateTime InputDateTime { get; set; }

        [EmailAddress]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "タイトルを入力して下さい。")]//川村：エラーメッセージ追記
        [Column(TypeName = "varchar(60)")]
        public string Title { get; set; }

        [Column(TypeName = "varchar(60)")]
        [Required(ErrorMessage = "カテゴリーを入力して下さい。")]
        public string Category { get; set; }

        [Required(ErrorMessage = "本文を入力して下さい。")]
        public string Text { get; set; }

        [Required]
        public DateTime Create_Time { get; set; }

        [Required]
        public DateTime Update_Time { get; set; }
    }
}
