namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GpsTableNameToLocation : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Gps", newName: "Locations");
            AddColumn("dbo.Photos", "ColorFlag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "ColorFlag");
            RenameTable(name: "dbo.Locations", newName: "Gps");
        }
    }
}
