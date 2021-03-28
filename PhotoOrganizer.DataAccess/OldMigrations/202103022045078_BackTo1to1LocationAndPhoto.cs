namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BackTo1to1LocationAndPhoto : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Photos", name: "Location_Id", newName: "LocationId");
            RenameIndex(table: "dbo.Photos", name: "IX_Location_Id", newName: "IX_LocationId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Photos", name: "IX_LocationId", newName: "IX_Location_Id");
            RenameColumn(table: "dbo.Photos", name: "LocationId", newName: "Location_Id");
        }
    }
}
