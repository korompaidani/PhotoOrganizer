namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileEntryTableCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageFilePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileEntries");
        }
    }
}
