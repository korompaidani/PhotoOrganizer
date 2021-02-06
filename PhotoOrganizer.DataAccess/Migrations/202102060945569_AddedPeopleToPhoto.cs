namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPeopleToPhoto : DbMigration
    {
        public override void Up()
        {
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
                .ForeignKey("dbo.People", t => t.PeopleId, cascadeDelete: true) //cascadeDelete:true means that I delete the photo, People will be also deleted
                .Index(t => t.PeopleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.People", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.Aliases", "PeopleId", "dbo.People");
            DropIndex("dbo.Aliases", new[] { "PeopleId" });
            DropIndex("dbo.People", new[] { "PhotoId" });
            DropTable("dbo.Aliases");
            DropTable("dbo.People");
        }
    }
}
