namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
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
                        Title = c.String(nullable: false),
                        FullPath = c.String(nullable: false),
                        YearId = c.Int(),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        HHMMSS = c.Time(nullable: false, precision: 7),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Years", t => t.YearId)
                .Index(t => t.YearId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(),
                        PhotoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .Index(t => t.PhotoId);
            
            CreateTable(
                "dbo.Aliases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nick = c.String(nullable: false),
                        PeopleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PeopleId, cascadeDelete: true)
                .Index(t => t.PeopleId);
            
            CreateTable(
                "dbo.Years",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PhotoTakenYear = c.Int(nullable: false),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "YearId", "dbo.Years");
            DropForeignKey("dbo.People", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.Aliases", "PeopleId", "dbo.People");
            DropForeignKey("dbo.PhotoAlbums", "Album_Id", "dbo.Albums");
            DropForeignKey("dbo.PhotoAlbums", "Photo_Id", "dbo.Photos");
            DropIndex("dbo.PhotoAlbums", new[] { "Album_Id" });
            DropIndex("dbo.PhotoAlbums", new[] { "Photo_Id" });
            DropIndex("dbo.Aliases", new[] { "PeopleId" });
            DropIndex("dbo.People", new[] { "PhotoId" });
            DropIndex("dbo.Photos", new[] { "YearId" });
            DropTable("dbo.PhotoAlbums");
            DropTable("dbo.Years");
            DropTable("dbo.Aliases");
            DropTable("dbo.People");
            DropTable("dbo.Photos");
            DropTable("dbo.Albums");
        }
    }
}
