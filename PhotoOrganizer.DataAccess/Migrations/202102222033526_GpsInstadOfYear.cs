namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GpsInstadOfYear : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Photos", "YearId", "dbo.Years");
            DropIndex("dbo.Photos", new[] { "YearId" });
            CreateTable(
                "dbo.Gps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Coordinates = c.String(nullable: false),
                        LocationName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Photos", "LocationId", c => c.Int());
            CreateIndex("dbo.Photos", "LocationId");
            AddForeignKey("dbo.Photos", "LocationId", "dbo.Gps", "Id");
            DropColumn("dbo.Photos", "YearId");
            DropTable("dbo.Years");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Years",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PhotoTakenYear = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Photos", "YearId", c => c.Int());
            DropForeignKey("dbo.Photos", "LocationId", "dbo.Gps");
            DropIndex("dbo.Photos", new[] { "LocationId" });
            DropColumn("dbo.Photos", "LocationId");
            DropTable("dbo.Gps");
            CreateIndex("dbo.Photos", "YearId");
            AddForeignKey("dbo.Photos", "YearId", "dbo.Years", "Id");
        }
    }
}
