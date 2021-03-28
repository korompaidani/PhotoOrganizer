namespace PhotoOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PeopleDisplayNameProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "DisplayName", c => c.String(nullable: false));
            AlterColumn("dbo.People", "FirstName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "FirstName", c => c.String(nullable: false));
            DropColumn("dbo.People", "DisplayName");
        }
    }
}
