namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShelveCreatedAgain : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shelves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Photos", "Shelve_Id", c => c.Int());
            CreateIndex("dbo.Photos", "Shelve_Id");
            AddForeignKey("dbo.Photos", "Shelve_Id", "dbo.Shelves", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "Shelve_Id", "dbo.Shelves");
            DropIndex("dbo.Photos", new[] { "Shelve_Id" });
            DropColumn("dbo.Photos", "Shelve_Id");
            DropTable("dbo.Shelves");
        }
    }
}
