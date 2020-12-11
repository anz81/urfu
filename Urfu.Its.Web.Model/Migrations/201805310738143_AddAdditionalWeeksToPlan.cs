namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdditionalWeeksToPlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "additionalWeeks", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "additionalWeeks");
        }
    }
}
