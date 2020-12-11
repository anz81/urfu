namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateProjectAdmissionAndStudentsPrioritiesTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectAdmissions",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        ProjectCompetitionGroupId = c.Int(nullable: false),
                        ProjectId = c.String(nullable: false, maxLength: 128),
                        Published = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.ProjectCompetitionGroupId, t.ProjectId })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.ProjectCompetitionGroups", t => t.ProjectCompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => new { t.ProjectId, t.ProjectCompetitionGroupId, t.Status }, name: "IX_ProjectAdmission_Count")
                .Index(t => t.ProjectCompetitionGroupId, name: "IX_ProjectAdmission_ProjectCompetitionGroupId");
            
            CreateTable(
                "dbo.ProjectStudentSelectionPriorities",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        competitionGroupId = c.Int(nullable: false),
                        projectId = c.String(nullable: false, maxLength: 128),
                        priority = c.Int(),
                        modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.competitionGroupId, t.projectId })
                .ForeignKey("dbo.ProjectCompetitionGroups", t => t.competitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.projectId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.competitionGroupId)
                .Index(t => t.projectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectStudentSelectionPriorities", "studentId", "dbo.Students");
            DropForeignKey("dbo.ProjectStudentSelectionPriorities", "projectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectStudentSelectionPriorities", "competitionGroupId", "dbo.ProjectCompetitionGroups");
            DropForeignKey("dbo.ProjectAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.ProjectAdmissions", "ProjectCompetitionGroupId", "dbo.ProjectCompetitionGroups");
            DropForeignKey("dbo.ProjectAdmissions", "ProjectId", "dbo.Projects");
            DropIndex("dbo.ProjectStudentSelectionPriorities", new[] { "projectId" });
            DropIndex("dbo.ProjectStudentSelectionPriorities", new[] { "competitionGroupId" });
            DropIndex("dbo.ProjectStudentSelectionPriorities", new[] { "studentId" });
            DropIndex("dbo.ProjectAdmissions", "IX_ProjectAdmission_ProjectCompetitionGroupId");
            DropIndex("dbo.ProjectAdmissions", "IX_ProjectAdmission_Count");
            DropIndex("dbo.ProjectAdmissions", new[] { "studentId" });
            DropTable("dbo.ProjectStudentSelectionPriorities");
            DropTable("dbo.ProjectAdmissions");
        }
    }
}
