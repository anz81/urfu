namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTablesForMUPs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MUPCompetitionGroups",
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
                "dbo.MUPProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPCompetitionGroupId = c.Int(nullable: false),
                        MUPId = c.String(maxLength: 128),
                        Limit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPCompetitionGroups", t => t.MUPCompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.MUPs", t => t.MUPId)
                .Index(t => t.MUPCompetitionGroupId)
                .Index(t => t.MUPId);
            
            CreateTable(
                "dbo.MUPs",
                c => new
                    {
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        ModuleTechId = c.Int(nullable: false),
                        ShowInLC = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.ModuleTeches", t => t.ModuleTechId, cascadeDelete: true)
                .Index(t => t.ModuleId)
                .Index(t => t.ModuleTechId);

            CreateTable(
                "dbo.MUPPeriods",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    MUPId = c.String(maxLength: 128),
                    Year = c.Int(nullable: false),
                    SemesterId = c.Int(nullable: false),
                    SelectionDeadline = c.DateTime(),
                    Course = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPs", t => t.MUPId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.MUPId)
                .Index(t => t.SemesterId);

            CreateTable(
                "dbo.MUPDisciplines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPId = c.String(maxLength: 128),
                        DisciplineUid = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineUid)
                .ForeignKey("dbo.MUPs", t => t.MUPId)
                .Index(t => t.MUPId)
                .Index(t => t.DisciplineUid);
            
            CreateTable(
                "dbo.MUPDisciplineTmers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPDisciplineId = c.Int(nullable: false),
                        TmerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPDisciplines", t => t.MUPDisciplineId, cascadeDelete: true)
                .ForeignKey("dbo.Tmer", t => t.TmerId)
                .Index(t => t.MUPDisciplineId)
                .Index(t => t.TmerId);
            
            CreateTable(
                "dbo.MUPDisciplineTmerPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPDisciplineTmerId = c.Int(nullable: false),
                        MUPPeriodId = c.Int(nullable: false),
                        Distribution = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPPeriods", t => t.MUPPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.MUPDisciplineTmers", t => t.MUPDisciplineTmerId, cascadeDelete: true)
                .Index(t => t.MUPDisciplineTmerId)
                .Index(t => t.MUPPeriodId);
            
            CreateTable(
                "dbo.MUPSubgroupCounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPDisciplineTmerPeriodId = c.Int(nullable: false),
                        CompetitionGroupId = c.Int(nullable: false),
                        GroupCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPCompetitionGroups", t => t.CompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.MUPDisciplineTmerPeriods", t => t.MUPDisciplineTmerPeriodId)
                .Index(t => t.MUPDisciplineTmerPeriodId)
                .Index(t => t.CompetitionGroupId);
            
            CreateTable(
                "dbo.MUPSubgroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Limit = c.Int(nullable: false),
                        InnerNumber = c.Int(nullable: false),
                        ParentId = c.Int(),
                        SubgroupCountId = c.Int(nullable: false),
                        ExpectedChildCount = c.Double(),
                        TeacherId = c.String(maxLength: 127),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPSubgroupCounts", t => t.SubgroupCountId, cascadeDelete: true)
                .ForeignKey("dbo.MUPSubgroups", t => t.ParentId)
                .ForeignKey("dbo.Teachers", t => t.TeacherId)
                .Index(t => t.ParentId)
                .Index(t => t.SubgroupCountId)
                .Index(t => t.TeacherId);
            
            CreateTable(
                "dbo.MUPSubgroupMemberships",
                c => new
                    {
                        SubgroupId = c.Int(nullable: false),
                        studentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SubgroupId, t.studentId })
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.MUPSubgroups", t => t.SubgroupId, cascadeDelete: true)
                .Index(t => t.SubgroupId)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.MUPAdmissions",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        MUPCompetitionGroupId = c.Int(nullable: false),
                        MUPId = c.String(nullable: false, maxLength: 128),
                        Published = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.MUPCompetitionGroupId, t.MUPId })
                .ForeignKey("dbo.MUPs", t => t.MUPId, cascadeDelete: true)
                .ForeignKey("dbo.MUPCompetitionGroups", t => t.MUPCompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => new { t.MUPId, t.MUPCompetitionGroupId, t.Status }, name: "IX_MUPAdmission_Count")
                .Index(t => t.MUPCompetitionGroupId, name: "IX_MUPAdmission_MUPCompetitionGroupId");
            
            CreateTable(
                "dbo.MUPDisciplineTmerPeriodDivision",
                c => new
                    {
                        MUPDisciplineTmerPeriodId = c.Int(nullable: false),
                        DivisionId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.MUPDisciplineTmerPeriodId, t.DivisionId })
                .ForeignKey("dbo.MUPDisciplineTmerPeriods", t => t.MUPDisciplineTmerPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.Divisions", t => t.DivisionId, cascadeDelete: true)
                .Index(t => t.MUPDisciplineTmerPeriodId)
                .Index(t => t.DivisionId);
            
            CreateTable(
                "dbo.MUPCompetitionGroupContents",
                c => new
                    {
                        MUPCompetitionGroupId = c.Int(nullable: false),
                        GroupId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.MUPCompetitionGroupId, t.GroupId })
                .ForeignKey("dbo.MUPCompetitionGroups", t => t.MUPCompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.MUPCompetitionGroupId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.MUPTeachers",
                c => new
                    {
                        MUPPropertyId = c.Int(nullable: false),
                        TeacherId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.MUPPropertyId, t.TeacherId })
                .ForeignKey("dbo.MUPProperties", t => t.MUPPropertyId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.MUPPropertyId)
                .Index(t => t.TeacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MUPAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.MUPAdmissions", "MUPCompetitionGroupId", "dbo.MUPCompetitionGroups");
            DropForeignKey("dbo.MUPAdmissions", "MUPId", "dbo.MUPs");
            DropForeignKey("dbo.MUPTeachers", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.MUPTeachers", "MUPPropertyId", "dbo.MUPProperties");
            DropForeignKey("dbo.MUPProperties", "MUPId", "dbo.MUPs");
            DropForeignKey("dbo.MUPs", "ModuleTechId", "dbo.ModuleTeches");
            DropForeignKey("dbo.MUPs", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.MUPDisciplineTmers", "TmerId", "dbo.Tmer");
            DropForeignKey("dbo.MUPDisciplineTmerPeriods", "MUPDisciplineTmerId", "dbo.MUPDisciplineTmers");
            DropForeignKey("dbo.MUPDisciplineTmerPeriods", "MUPPeriodId", "dbo.MUPPeriods");
            DropForeignKey("dbo.MUPPeriods", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.MUPPeriods", "MUPId", "dbo.MUPs");
            DropForeignKey("dbo.MUPSubgroups", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.MUPSubgroupMemberships", "SubgroupId", "dbo.MUPSubgroups");
            DropForeignKey("dbo.MUPSubgroupMemberships", "studentId", "dbo.Students");
            DropForeignKey("dbo.MUPSubgroups", "ParentId", "dbo.MUPSubgroups");
            DropForeignKey("dbo.MUPSubgroups", "SubgroupCountId", "dbo.MUPSubgroupCounts");
            DropForeignKey("dbo.MUPSubgroupCounts", "MUPDisciplineTmerPeriodId", "dbo.MUPDisciplineTmerPeriods");
            DropForeignKey("dbo.MUPSubgroupCounts", "CompetitionGroupId", "dbo.MUPCompetitionGroups");
            DropForeignKey("dbo.MUPCompetitionGroups", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.MUPProperties", "MUPCompetitionGroupId", "dbo.MUPCompetitionGroups");
            DropForeignKey("dbo.MUPCompetitionGroupContents", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.MUPCompetitionGroupContents", "MUPCompetitionGroupId", "dbo.MUPCompetitionGroups");
            DropForeignKey("dbo.MUPDisciplineTmerPeriodDivision", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.MUPDisciplineTmerPeriodDivision", "MUPDisciplineTmerPeriodId", "dbo.MUPDisciplineTmerPeriods");
            DropForeignKey("dbo.MUPDisciplineTmers", "MUPDisciplineId", "dbo.MUPDisciplines");
            DropForeignKey("dbo.MUPDisciplines", "MUPId", "dbo.MUPs");
            DropForeignKey("dbo.MUPDisciplines", "DisciplineUid", "dbo.Disciplines");
            DropIndex("dbo.MUPTeachers", new[] { "TeacherId" });
            DropIndex("dbo.MUPTeachers", new[] { "MUPPropertyId" });
            DropIndex("dbo.MUPCompetitionGroupContents", new[] { "GroupId" });
            DropIndex("dbo.MUPCompetitionGroupContents", new[] { "MUPCompetitionGroupId" });
            DropIndex("dbo.MUPDisciplineTmerPeriodDivision", new[] { "DivisionId" });
            DropIndex("dbo.MUPDisciplineTmerPeriodDivision", new[] { "MUPDisciplineTmerPeriodId" });
            DropIndex("dbo.MUPAdmissions", "IX_MUPAdmission_MUPCompetitionGroupId");
            DropIndex("dbo.MUPAdmissions", "IX_MUPAdmission_Count");
            DropIndex("dbo.MUPAdmissions", new[] { "studentId" });
            DropIndex("dbo.MUPPeriods", new[] { "SemesterId" });
            DropIndex("dbo.MUPPeriods", new[] { "MUPId" });
            DropIndex("dbo.MUPSubgroupMemberships", new[] { "studentId" });
            DropIndex("dbo.MUPSubgroupMemberships", new[] { "SubgroupId" });
            DropIndex("dbo.MUPSubgroups", new[] { "TeacherId" });
            DropIndex("dbo.MUPSubgroups", new[] { "SubgroupCountId" });
            DropIndex("dbo.MUPSubgroups", new[] { "ParentId" });
            DropIndex("dbo.MUPCompetitionGroups", new[] { "SemesterId" });
            DropIndex("dbo.MUPSubgroupCounts", new[] { "CompetitionGroupId" });
            DropIndex("dbo.MUPSubgroupCounts", new[] { "MUPDisciplineTmerPeriodId" });
            DropIndex("dbo.MUPDisciplineTmerPeriods", new[] { "MUPPeriodId" });
            DropIndex("dbo.MUPDisciplineTmerPeriods", new[] { "MUPDisciplineTmerId" });
            DropIndex("dbo.MUPDisciplineTmers", new[] { "TmerId" });
            DropIndex("dbo.MUPDisciplineTmers", new[] { "MUPDisciplineId" });
            DropIndex("dbo.MUPDisciplines", new[] { "DisciplineUid" });
            DropIndex("dbo.MUPDisciplines", new[] { "MUPId" });
            DropIndex("dbo.MUPs", new[] { "ModuleTechId" });
            DropIndex("dbo.MUPs", new[] { "ModuleId" });
            DropIndex("dbo.MUPProperties", new[] { "MUPId" });
            DropIndex("dbo.MUPProperties", new[] { "MUPCompetitionGroupId" });
            DropTable("dbo.MUPTeachers");
            DropTable("dbo.MUPCompetitionGroupContents");
            DropTable("dbo.MUPDisciplineTmerPeriodDivision");
            DropTable("dbo.MUPAdmissions");
            DropTable("dbo.MUPPeriods");
            DropTable("dbo.MUPSubgroupMemberships");
            DropTable("dbo.MUPSubgroups");
            DropTable("dbo.MUPCompetitionGroups");
            DropTable("dbo.MUPSubgroupCounts");
            DropTable("dbo.MUPDisciplineTmerPeriods");
            DropTable("dbo.MUPDisciplineTmers");
            DropTable("dbo.MUPDisciplines");
            DropTable("dbo.MUPs");
            DropTable("dbo.MUPProperties");
        }
    }
}
