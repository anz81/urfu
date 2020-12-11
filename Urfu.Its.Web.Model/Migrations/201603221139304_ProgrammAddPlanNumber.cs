namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProgrammAddPlanNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "PlanNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EduPrograms", "PlanNumber");
        }
    }
}
