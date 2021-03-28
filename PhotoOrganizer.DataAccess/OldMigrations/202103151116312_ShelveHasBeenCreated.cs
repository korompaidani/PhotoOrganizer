namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShelveHasBeenCreated : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Photos", "Title", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Photos", "Title", c => c.String(nullable: false));
        }
    }
}
