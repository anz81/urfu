namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateProjectCompetitionGroupTables : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ProjectDisciplineTmerPeriodDivisions", newName: "ProjectDisciplineTmerPeriodDivision");
            RenameColumn(table: "dbo.ProjectDisciplineTmerPeriodDivision", name: "ProjectDisciplineTmerPeriod_Id", newName: "ProjectDisciplineTmerPeriodId");
            RenameColumn(table: "dbo.ProjectDisciplineTmerPeriodDivision", name: "Division_uuid", newName: "DivisionId");
            RenameIndex(table: "dbo.ProjectDisciplineTmerPeriodDivision", name: "IX_ProjectDisciplineTmerPeriod_Id", newName: "IX_ProjectDisciplineTmerPeriodId");
            RenameIndex(table: "dbo.ProjectDisciplineTmerPeriodDivision", name: "IX_Division_uuid", newName: "IX_DivisionId");
            CreateTable(
                "dbo.ProjectProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectCompetitionGroupId = c.Int(nullable: false),
                        ProjectId = c.String(maxLength: 128),
                        LimitId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContractLimits", t => t.LimitId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .ForeignKey("dbo.ProjectCompetitionGroups", t => t.ProjectCompetitionGroupId, cascadeDelete: true)
                .Index(t => t.ProjectCompetitionGroupId)
                .Index(t => t.ProjectId)
                .Index(t => t.LimitId);
            
            CreateTable(
                "dbo.ProjectCompetitionGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ShortName = c.String(nullable: false),
                        StudentCourse = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.ProjectCompetitionGroupContents",
                c => new
                    {
                        ProjectCompetitionGroupId = c.Int(nullable: false),
                        GroupId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ProjectCompetitionGroupId, t.GroupId })
                .ForeignKey("dbo.ProjectCompetitionGroups", t => t.ProjectCompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.ProjectCompetitionGroupId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.ProjectTeachers",
                c => new
                    {
                        ProjectPropertyId = c.Int(nullable: false),
                        TeacherId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.ProjectPropertyId, t.TeacherId })
                .ForeignKey("dbo.ProjectProperties", t => t.ProjectPropertyId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.ProjectPropertyId)
                .Index(t => t.TeacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectTeachers", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.ProjectTeachers", "ProjectPropertyId", "dbo.ProjectProperties");
            DropForeignKey("dbo.ProjectCompetitionGroups", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.ProjectProperties", "ProjectCompetitionGroupId", "dbo.ProjectCompetitionGroups");
            DropForeignKey("dbo.ProjectCompetitionGroupContents", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.ProjectCompetitionGroupContents", "ProjectCompetitionGroupId", "dbo.ProjectCompetitionGroups");
            DropForeignKey("dbo.ProjectProperties", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectProperties", "LimitId", "dbo.ContractLimits");
            DropIndex("dbo.ProjectTeachers", new[] { "TeacherId" });
            DropIndex("dbo.ProjectTeachers", new[] { "ProjectPropertyId" });
            DropIndex("dbo.ProjectCompetitionGroupContents", new[] { "GroupId" });
            DropIndex("dbo.ProjectCompetitionGroupContents", new[] { "ProjectCompetitionGroupId" });
            DropIndex("dbo.ProjectCompetitionGroups", new[] { "SemesterId" });
            DropIndex("dbo.ProjectProperties", new[] { "LimitId" });
            DropIndex("dbo.ProjectProperties", new[] { "ProjectId" });
            DropIndex("dbo.ProjectProperties", new[] { "ProjectCompetitionGroupId" });
            DropTable("dbo.ProjectTeachers");
            DropTable("dbo.ProjectCompetitionGroupContents");
            DropTable("dbo.ProjectCompetitionGroups");
            DropTable("dbo.ProjectProperties");
            RenameIndex(table: "dbo.ProjectDisciplineTmerPeriodDivision", name: "IX_DivisionId", newName: "IX_Division_uuid");
            RenameIndex(table: "dbo.ProjectDisciplineTmerPeriodDivision", name: "IX_ProjectDisciplineTmerPeriodId", newName: "IX_ProjectDisciplineTmerPeriod_Id");
            RenameColumn(table: "dbo.ProjectDisciplineTmerPeriodDivision", name: "DivisionId", newName: "Division_uuid");
            RenameColumn(table: "dbo.ProjectDisciplineTmerPeriodDivision", name: "ProjectDisciplineTmerPeriodId", newName: "ProjectDisciplineTmerPeriod_Id");
            RenameTable(name: "dbo.ProjectDisciplineTmerPeriodDivision", newName: "ProjectDisciplineTmerPeriodDivisions");
        }
    }
}
