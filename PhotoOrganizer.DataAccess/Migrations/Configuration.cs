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
            context.Photos.AddOrUpdate(
                p => p.FullPath, 
                new Photo { Title = "SAN_20190315_121052_Canon_EOS_450D_IMG_2153", 
                    FullPath = @".\..\..\Resources\TestResources\Photos\SAN_20190315_121052_Canon_EOS_450D_IMG_2153.JPG" },
                new Photo
                {
                    Title = "SAN_20190314_080308_Canon_EOS_450D_IMG_1645",
                    FullPath = @".\..\..\Resources\TestResources\Photos\SAN_20190314_080308_Canon_EOS_450D_IMG_1645.JPG"
                }
                );

            context.Years.AddOrUpdate(y => y.PhotoTakenYear,
                new Year { PhotoTakenYear = 2019},
                new Year { PhotoTakenYear = 1988 });

            context.SaveChanges();

            context.People.AddOrUpdate(p => p.FirstName,
                new People { FirstName = "Daniel", PhotoId = context.Photos.First().Id });
        }
    }
}
