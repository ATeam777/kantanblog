using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KantanBlog001.Models;

namespace KantanBlog001.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<KantanBlog001.Models.Person> Person { get; set; } = default!;
        public DbSet<KantanBlog001.Models.Article> Article { get; set; } = default!;
        public DbSet<KantanBlog001.Models.Comment> Comment { get; set; } = default!;
    }
}
