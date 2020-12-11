namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanTerms_eduplanUUID : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PlanTerms", "IX_PlanTerms_QueryIndex");
            AddColumn("dbo.PlanTerms", "eduplanUUID", c => c.String(maxLength: 128));
            CreateIndex("dbo.PlanTerms", "eduplanUUID", name: "IX_PlanTerms_QueryIndex");
            DropColumn("dbo.PlanTerms", "disciplineUID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlanTerms", "disciplineUID", c => c.String(maxLength: 127));
            DropIndex("dbo.PlanTerms", "IX_PlanTerms_QueryIndex");
            DropColumn("dbo.PlanTerms", "eduplanUUID");
            CreateIndex("dbo.PlanTerms", "disciplineUID", name: "IX_PlanTerms_QueryIndex");
        }
    }
}
