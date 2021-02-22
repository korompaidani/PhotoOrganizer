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
            //context.Coordinates.AddOrUpdate(y => y.Coordinates,
            //    new Gps { Coordinates = "Vajda Lajos utca 22."},
            //    new Gps { Coordinates = "Kenese Zrínyi út 5." });

            //context.SaveChanges();

            //context.People.AddOrUpdate(p => p.FirstName,
            //    new People { FirstName = "Daniel", PhotoId = context.Photos.First().Id });

            //context.Albums.AddOrUpdate(p => p.Title,
            //    new Album
            //    {
            //        Title = "Los Angeles",
            //        DateFrom = new DateTime(2019, 3, 14),
            //        DateTo = new DateTime(2019, 3, 15),
            //        Photos = new List<Photo>
            //        {
            //            context.Photos.Single(p => p.Title.Contains("20190315_121052")),
            //            context.Photos.Single(p => p.Title.Contains("20190314_080308"))
            //        }
            //    });
        }
    }
}
