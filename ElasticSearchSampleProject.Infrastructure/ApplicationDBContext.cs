using ElasticSearchSampleProject.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElasticSearchSampleProject.Infrastructure
{
    public class ApplicationDBContext : DbContext
    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

         public DbSet<Products> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
