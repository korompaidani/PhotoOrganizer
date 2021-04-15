namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tabularasa : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .Index(t => t.LocationId);
            
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
                "dbo.Aliases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nick = c.String(nullable: false),
                        PeopleId = c.Int(nullable: false),
                        Description = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PeopleId, cascadeDelete: true)
                .Index(t => t.PeopleId);
            
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
                "dbo.FileEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OriginalImagePath = c.String(),
                        ThumbnailPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PhotoAlbums",
                c => new
                    {
                        Photo_Id = c.Int(nullable: false),
                        Album_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Photo_Id, t.Album_Id })
                .ForeignKey("dbo.Photos", t => t.Photo_Id, cascadeDelete: true)
                .ForeignKey("dbo.Albums", t => t.Album_Id, cascadeDelete: true)
                .Index(t => t.Photo_Id)
                .Index(t => t.Album_Id);
            
            CreateTable(
                "dbo.PeoplePhotoes",
                c => new
                    {
                        People_Id = c.Int(nullable: false),
                        Photo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.People_Id, t.Photo_Id })
                .ForeignKey("dbo.People", t => t.People_Id, cascadeDelete: true)
                .ForeignKey("dbo.Photos", t => t.Photo_Id, cascadeDelete: true)
                .Index(t => t.People_Id)
                .Index(t => t.Photo_Id);
            
            CreateTable(
                "dbo.ShelvePhotoes",
                c => new
                    {
                        Shelve_Id = c.Int(nullable: false),
                        Photo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Shelve_Id, t.Photo_Id })
                .ForeignKey("dbo.Shelves", t => t.Shelve_Id, cascadeDelete: true)
                .ForeignKey("dbo.Photos", t => t.Photo_Id, cascadeDelete: true)
                .Index(t => t.Shelve_Id)
                .Index(t => t.Photo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShelvePhotoes", "Photo_Id", "dbo.Photos");
            DropForeignKey("dbo.ShelvePhotoes", "Shelve_Id", "dbo.Shelves");
            DropForeignKey("dbo.PeoplePhotoes", "Photo_Id", "dbo.Photos");
            DropForeignKey("dbo.PeoplePhotoes", "People_Id", "dbo.People");
            DropForeignKey("dbo.Aliases", "PeopleId", "dbo.People");
            DropForeignKey("dbo.Photos", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.PhotoAlbums", "Album_Id", "dbo.Albums");
            DropForeignKey("dbo.PhotoAlbums", "Photo_Id", "dbo.Photos");
            DropIndex("dbo.ShelvePhotoes", new[] { "Photo_Id" });
            DropIndex("dbo.ShelvePhotoes", new[] { "Shelve_Id" });
            DropIndex("dbo.PeoplePhotoes", new[] { "Photo_Id" });
            DropIndex("dbo.PeoplePhotoes", new[] { "People_Id" });
            DropIndex("dbo.PhotoAlbums", new[] { "Album_Id" });
            DropIndex("dbo.PhotoAlbums", new[] { "Photo_Id" });
            DropIndex("dbo.Aliases", new[] { "PeopleId" });
            DropIndex("dbo.Photos", new[] { "LocationId" });
            DropTable("dbo.ShelvePhotoes");
            DropTable("dbo.PeoplePhotoes");
            DropTable("dbo.PhotoAlbums");
            DropTable("dbo.FileEntries");
            DropTable("dbo.Shelves");
            DropTable("dbo.Aliases");
            DropTable("dbo.People");
            DropTable("dbo.Locations");
            DropTable("dbo.Photos");
            DropTable("dbo.Albums");
        }
    }
}
