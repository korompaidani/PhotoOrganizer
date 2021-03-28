namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShelvePhotoN2N : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Photos", "Shelve_Id", "dbo.Shelves");
            DropIndex("dbo.Photos", new[] { "Shelve_Id" });
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
            
            DropColumn("dbo.Photos", "Shelve_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "Shelve_Id", c => c.Int());
            DropForeignKey("dbo.ShelvePhotoes", "Photo_Id", "dbo.Photos");
            DropForeignKey("dbo.ShelvePhotoes", "Shelve_Id", "dbo.Shelves");
            DropIndex("dbo.ShelvePhotoes", new[] { "Photo_Id" });
            DropIndex("dbo.ShelvePhotoes", new[] { "Shelve_Id" });
            DropTable("dbo.ShelvePhotoes");
            CreateIndex("dbo.Photos", "Shelve_Id");
            AddForeignKey("dbo.Photos", "Shelve_Id", "dbo.Shelves", "Id");
        }
    }
}
