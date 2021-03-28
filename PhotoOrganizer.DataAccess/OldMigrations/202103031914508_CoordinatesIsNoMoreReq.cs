namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CoordinatesIsNoMoreReq : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Locations", "Coordinates", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Locations", "Coordinates", c => c.String(nullable: false));
        }
    }
}
