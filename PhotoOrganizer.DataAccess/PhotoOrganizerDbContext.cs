using PhotoOrganizer.Model;
using System.Data.Entity;

namespace PhotoOrganizer.DataAccess
{
    public class PhotoOrganizerDbContext : DbContext
    {
        public PhotoOrganizerDbContext() : base("PhotoOrganizerSandboxDb")
        {
        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Year> Years { get; set; }
        public DbSet<People> People { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
