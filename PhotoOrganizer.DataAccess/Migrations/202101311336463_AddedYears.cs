namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedYears : DbMigration
    {
        public override void Up()
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
            CreateIndex("dbo.Photos", "YearId");
            AddForeignKey("dbo.Photos", "YearId", "dbo.Years", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "YearId", "dbo.Years");
            DropIndex("dbo.Photos", new[] { "YearId" });
            DropColumn("dbo.Photos", "YearId");
            DropTable("dbo.Years");
        }
    }
}
