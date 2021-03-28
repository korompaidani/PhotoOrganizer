namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ColorFlagToEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Photos", "ColorFlag", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Photos", "ColorFlag", c => c.String());
        }
    }
}
