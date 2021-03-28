namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionAndCreator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "Description", c => c.String());
            AddColumn("dbo.Photos", "Creator", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "Creator");
            DropColumn("dbo.Photos", "Description");
        }
    }
}
