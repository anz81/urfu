namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignDefaultPlannumber : DbMigration
    {
        public override void Up()
        {
            Sql("update EduPrograms set PlanNumber = 1 where PlanNumber is null");
        }
        
        public override void Down()
        {
        }
    }
}
