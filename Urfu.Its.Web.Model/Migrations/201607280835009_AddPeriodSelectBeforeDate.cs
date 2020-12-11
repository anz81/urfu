namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPeriodSelectBeforeDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorPeriods", "SelectionDeadline", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MinorPeriods", "SelectionDeadline");
        }
    }
}
