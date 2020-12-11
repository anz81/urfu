namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanAdditional_TLString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlanAdditionals", "controls", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl1", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl2", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl3", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl4", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl5", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl6", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl7", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl8", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl9", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl10", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl11", c => c.String());
            AlterColumn("dbo.PlanAdditionals", "tl12", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlanAdditionals", "tl12", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl11", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl10", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl9", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl8", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl7", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl6", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl5", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl4", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl3", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl2", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl1", c => c.Int());
            DropColumn("dbo.PlanAdditionals", "controls");
        }
    }
}
