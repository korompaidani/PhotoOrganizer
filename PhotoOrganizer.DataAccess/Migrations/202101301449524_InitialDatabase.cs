namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        FullPath = c.String(nullable: false),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        HHMMSS = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Photos");
        }
    }
}
