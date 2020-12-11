namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanAddtionals_NullableFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PlanAdditionals", "tl1", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl2", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl3", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl4", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl5", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl6", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl7", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl8", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl9", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl10", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl11", c => c.Int());
            AlterColumn("dbo.PlanAdditionals", "tl12", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlanAdditionals", "tl12", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl11", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl10", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl9", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl8", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl7", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl6", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl5", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl4", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl3", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl2", c => c.Int(nullable: false));
            AlterColumn("dbo.PlanAdditionals", "tl1", c => c.Int(nullable: false));
        }
    }
}
