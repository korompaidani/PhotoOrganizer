namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotoTableWithStringCoordinates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "Coordinates", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "Coordinates");
        }
    }
}
