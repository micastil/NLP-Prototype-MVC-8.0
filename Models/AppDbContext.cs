using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NLP_Prototype_MVC_8._0.Areas.Identity.Data;

namespace NLP_Prototype_MVC_8._0.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>
            options) : base(options)
        {

        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<Topic> Topics { get; set; }
    }
}
