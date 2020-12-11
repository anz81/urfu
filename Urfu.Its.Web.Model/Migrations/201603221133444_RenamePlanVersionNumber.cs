namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamePlanVersionNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "PlanVersionNumber", c => c.Int(nullable: false));
            Sql("update EduPrograms set PlanVersionNumber =  PlanNumber");
            DropColumn("dbo.EduPrograms", "PlanNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EduPrograms", "PlanNumber", c => c.Int(nullable: false));
            DropColumn("dbo.EduPrograms", "PlanVersionNumber");
        }
    }
}
