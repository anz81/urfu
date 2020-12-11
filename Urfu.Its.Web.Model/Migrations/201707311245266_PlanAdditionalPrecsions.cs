namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanAdditionalPrecsions : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PlanAdditionals", "contactTotal", c => c.Decimal(precision: 7, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactSelf", c => c.Decimal(precision: 7, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactControl", c => c.Decimal(precision: 7, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactLecture", c => c.Decimal(precision: 7, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactPractice", c => c.Decimal(precision: 7, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactLabs", c => c.Decimal(precision: 7, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlanAdditionals", "contactLabs", c => c.Decimal(precision: 6, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactPractice", c => c.Decimal(precision: 6, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactLecture", c => c.Decimal(precision: 6, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactControl", c => c.Decimal(precision: 6, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactSelf", c => c.Decimal(precision: 6, scale: 2));
            AlterColumn("dbo.PlanAdditionals", "contactTotal", c => c.Decimal(precision: 6, scale: 2));
        }
    }
}
