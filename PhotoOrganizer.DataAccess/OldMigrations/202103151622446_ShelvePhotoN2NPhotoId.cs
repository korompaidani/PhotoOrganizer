namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShelvePhotoN2NPhotoId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shelves", "PhotoId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shelves", "PhotoId");
        }
    }
}
