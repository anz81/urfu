namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KindAction_Competence : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Competences", "KindActionId", "dbo.KindAction");
            DropIndex("dbo.Competences", new[] { "KindActionId" });
            AddColumn("dbo.KindAction", "CompetenceId", c => c.Int());
            CreateIndex("dbo.KindAction", "CompetenceId");
            AddForeignKey("dbo.KindAction", "CompetenceId", "dbo.Competences", "Id");
            DropColumn("dbo.Competences", "KindActionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Competences", "KindActionId", c => c.Int());
            DropForeignKey("dbo.KindAction", "CompetenceId", "dbo.Competences");
            DropIndex("dbo.KindAction", new[] { "CompetenceId" });
            DropColumn("dbo.KindAction", "CompetenceId");
            CreateIndex("dbo.Competences", "KindActionId");
            AddForeignKey("dbo.Competences", "KindActionId", "dbo.KindAction", "Id");
        }
    }
}
