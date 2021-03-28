namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentHasBeenAddedToPhoto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "Comment");
        }
    }
}
