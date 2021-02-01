using PhotoOrganizer.Model;
using System.Data.Entity;

namespace PhotoOrganizer.DataAccess
{
    public class PhotoOrganizerDbContext : DbContext
    {
        public PhotoOrganizerDbContext() : base("PhotoOrganizerDb")
        {
        }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Year> Years { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
