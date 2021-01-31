namespace PhotoOrganizer.DataAccess.Migrations
{
    using PhotoOrganizer.FileHandler;
    using PhotoOrganizer.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PhotoOrganizer.DataAccess.PhotoOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PhotoOrganizer.DataAccess.PhotoOrganizerDbContext context)
        {            
            context.Years.AddOrUpdate(y => y.PhotoTakenYear,
                new Year { PhotoTakenYear = 2019},
                new Year { PhotoTakenYear = 1988 });
        }
    }
}
