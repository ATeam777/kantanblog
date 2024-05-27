using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KantanBlog001.Models
{
    public class Comment
    {
        public Comment()
        {
            UserName = "匿名";
        }

        [Required]
        public int CommentId { get; set; }

        public int ArticleId { get; set; }

        public Article Article { get; set; }

        [Display(Name = "名前")]
        [Column(TypeName = "varchar(60)")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "コメント")]
        public string CommentText { get; set; }

        [Required]
        [Display(Name = "入力日時")]
        public DateTime Create_Time { get; set; }

        [Required]
        public DateTime Update_Time { get; set; }

       
    }
}