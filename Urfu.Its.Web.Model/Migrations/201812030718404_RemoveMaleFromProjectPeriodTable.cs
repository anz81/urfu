namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMaleFromProjectPeriodTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProjectPeriods", "Male");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectPeriods", "Male", c => c.Boolean());
        }
    }
}
