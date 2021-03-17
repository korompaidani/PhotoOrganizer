using PhotoOrganizer.Model;
using System.Data.Entity;

namespace PhotoOrganizer.DataAccess
{
    public class PhotoOrganizerDbContext : DbContext
    {
        public PhotoOrganizerDbContext() : base("PhotoOrganizerReleaseDb")
        {
        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<People> People { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Shelve> Shelves { get; set; }
        public DbSet<FileEntry> FileEntries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
