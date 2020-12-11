namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateProjectCompetencesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectCompetences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.String(maxLength: 128, nullable: false),
                        CompetenceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Competences", t => t.CompetenceId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId)
                .Index(t => t.CompetenceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectCompetences", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectCompetences", "CompetenceId", "dbo.Competences");
            DropIndex("dbo.ProjectCompetences", new[] { "CompetenceId" });
            DropIndex("dbo.ProjectCompetences", new[] { "ProjectId" });
            DropTable("dbo.ProjectCompetences");
        }
    }
}
