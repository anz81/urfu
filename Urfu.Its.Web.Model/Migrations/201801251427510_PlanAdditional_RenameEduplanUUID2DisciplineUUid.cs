namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanAdditional_RenameEduplanUUID2DisciplineUUid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.PlanAdditionals");
            RenameColumn("dbo.PlanAdditionals", "eduplanUUID", "disciplineUUID");
            AddPrimaryKey("dbo.PlanAdditionals", new[] { "versionUUID", "disciplineUUID" });
        }

        public override void Down()
        {
            DropPrimaryKey("dbo.PlanAdditionals");
            RenameColumn("dbo.PlanAdditionals", "disciplineUUID", "eduplanUUID");
            AddPrimaryKey("dbo.PlanAdditionals", new[] { "versionUUID", "eduplanUUID" });
        }
    }
}
