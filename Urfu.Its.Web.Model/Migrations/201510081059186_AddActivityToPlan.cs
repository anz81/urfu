namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActivityToPlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Plans", "versionStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "versionStatus");
            DropColumn("dbo.Plans", "active");
        }
    }
}
