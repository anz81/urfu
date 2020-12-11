namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Competence_KindAction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KindAction", "CompetenceId", "dbo.Competences");
            DropIndex("dbo.KindAction", new[] { "CompetenceId" });
            AddColumn("dbo.Competences", "KindActionId", c => c.Int());
            CreateIndex("dbo.Competences", "KindActionId");
            AddForeignKey("dbo.Competences", "KindActionId", "dbo.KindAction", "Id");
            DropColumn("dbo.KindAction", "CompetenceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KindAction", "CompetenceId", c => c.Int());
            DropForeignKey("dbo.Competences", "KindActionId", "dbo.KindAction");
            DropIndex("dbo.Competences", new[] { "KindActionId" });
            DropColumn("dbo.Competences", "KindActionId");
            CreateIndex("dbo.KindAction", "CompetenceId");
            AddForeignKey("dbo.KindAction", "CompetenceId", "dbo.Competences", "Id");
        }
    }
}
