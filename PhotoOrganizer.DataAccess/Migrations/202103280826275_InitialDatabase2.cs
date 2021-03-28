namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PhotoAlbums", "Photo_Id", "dbo.Photos");
            DropForeignKey("dbo.PhotoAlbums", "Album_Id", "dbo.Albums");
            DropForeignKey("dbo.Photos", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Aliases", "PeopleId", "dbo.People");
            DropForeignKey("dbo.PeoplePhotoes", "People_Id", "dbo.People");
            DropForeignKey("dbo.PeoplePhotoes", "Photo_Id", "dbo.Photos");
            DropForeignKey("dbo.ShelvePhotoes", "Shelve_Id", "dbo.Shelves");
            DropForeignKey("dbo.ShelvePhotoes", "Photo_Id", "dbo.Photos");
            DropIndex("dbo.Photos", new[] { "LocationId" });
            DropIndex("dbo.Aliases", new[] { "PeopleId" });
            DropIndex("dbo.PhotoAlbums", new[] { "Photo_Id" });
            DropIndex("dbo.PhotoAlbums", new[] { "Album_Id" });
            DropIndex("dbo.PeoplePhotoes", new[] { "People_Id" });
            DropIndex("dbo.PeoplePhotoes", new[] { "Photo_Id" });
            DropIndex("dbo.ShelvePhotoes", new[] { "Shelve_Id" });
            DropIndex("dbo.ShelvePhotoes", new[] { "Photo_Id" });
            DropTable("dbo.Albums");
            DropTable("dbo.Photos");
            DropTable("dbo.Locations");
            DropTable("dbo.People");
            DropTable("dbo.Aliases");
            DropTable("dbo.Shelves");
            DropTable("dbo.FileEntries");
            DropTable("dbo.PhotoAlbums");
            DropTable("dbo.PeoplePhotoes");
            DropTable("dbo.ShelvePhotoes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ShelvePhotoes",
                c => new
                    {
                        Shelve_Id = c.Int(nullable: false),
                        Photo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Shelve_Id, t.Photo_Id });
            
            CreateTable(
                "dbo.PeoplePhotoes",
                c => new
                    {
                        People_Id = c.Int(nullable: false),
                        Photo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.People_Id, t.Photo_Id });
            
            CreateTable(
                "dbo.PhotoAlbums",
                c => new
                    {
                        Photo_Id = c.Int(nullable: false),
                        Album_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Photo_Id, t.Album_Id });
            
            CreateTable(
                "dbo.FileEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageFilePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shelves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        PhotoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Aliases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nick = c.String(nullable: false),
                        PeopleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        PhotoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Coordinates = c.String(),
                        LocationName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        FullPath = c.String(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Description = c.String(),
                        Comment = c.String(),
                        Creator = c.String(),
                        Coordinates = c.String(),
                        LocationId = c.Int(),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        HHMMSS = c.Time(nullable: false, precision: 7),
                        ColorFlag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.ShelvePhotoes", "Photo_Id");
            CreateIndex("dbo.ShelvePhotoes", "Shelve_Id");
            CreateIndex("dbo.PeoplePhotoes", "Photo_Id");
            CreateIndex("dbo.PeoplePhotoes", "People_Id");
            CreateIndex("dbo.PhotoAlbums", "Album_Id");
            CreateIndex("dbo.PhotoAlbums", "Photo_Id");
            CreateIndex("dbo.Aliases", "PeopleId");
            CreateIndex("dbo.Photos", "LocationId");
            AddForeignKey("dbo.ShelvePhotoes", "Photo_Id", "dbo.Photos", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ShelvePhotoes", "Shelve_Id", "dbo.Shelves", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PeoplePhotoes", "Photo_Id", "dbo.Photos", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PeoplePhotoes", "People_Id", "dbo.People", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Aliases", "PeopleId", "dbo.People", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Photos", "LocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.PhotoAlbums", "Album_Id", "dbo.Albums", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PhotoAlbums", "Photo_Id", "dbo.Photos", "Id", cascadeDelete: true);
        }
    }
}
