namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFlagFieldsTakeDatesFromGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Practices", "TakeDatesfromGroup", c => c.Boolean(nullable: false));
            AddColumn("dbo.Practices", "TakeReportDatesfromGroup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Practices", "TakeReportDatesfromGroup");
            DropColumn("dbo.Practices", "TakeDatesfromGroup");
        }
    }
}
