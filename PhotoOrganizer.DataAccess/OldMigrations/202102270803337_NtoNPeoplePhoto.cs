namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NtoNPeoplePhoto : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.People", "PhotoId", "dbo.Photos");
            DropIndex("dbo.People", new[] { "PhotoId" });
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PeoplePhotoes", "Photo_Id", "dbo.Photos");
            DropForeignKey("dbo.PeoplePhotoes", "People_Id", "dbo.People");
            DropIndex("dbo.PeoplePhotoes", new[] { "Photo_Id" });
            DropIndex("dbo.PeoplePhotoes", new[] { "People_Id" });
            DropTable("dbo.PeoplePhotoes");
            CreateIndex("dbo.People", "PhotoId");
            AddForeignKey("dbo.People", "PhotoId", "dbo.Photos", "Id", cascadeDelete: true);
        }
    }
}
