namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSectionFKTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SectionFKDisciplineTmerPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SectionFKDisciplineTmerId = c.Int(nullable: false),
                        SectionFKPeriodId = c.Int(nullable: false),
                        GroupCount = c.Int(nullable: false),
                        Distribution = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SectionFKPeriods", t => t.SectionFKPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.SectionFKDisciplineTmers", t => t.SectionFKDisciplineTmerId, cascadeDelete: true)
                .Index(t => t.SectionFKDisciplineTmerId)
                .Index(t => t.SectionFKPeriodId);
            
            CreateTable(
                "dbo.SectionFKPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SectionFKId = c.String(maxLength: 128),
                        Year = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                        SelectionDeadline = c.DateTime(),
                        MinStudentsCount = c.Int(nullable: false),
                        MaxStudentsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SectionFKs", t => t.SectionFKId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.SectionFKId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.SectionFKs",
                c => new
                    {
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        SectionFKTechId = c.Int(nullable: false),
                        ShowInLC = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.SectionFKTeches", t => t.SectionFKTechId, cascadeDelete: true)
                .Index(t => t.ModuleId)
                .Index(t => t.SectionFKTechId);
            
            CreateTable(
                "dbo.SectionFKDisciplines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SectionFKId = c.String(maxLength: 128),
                        DisciplineUid = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineUid)
                .ForeignKey("dbo.SectionFKs", t => t.SectionFKId)
                .Index(t => t.SectionFKId)
                .Index(t => t.DisciplineUid);
            
            CreateTable(
                "dbo.SectionFKDisciplineTmers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SectionFKDisciplineId = c.Int(nullable: false),
                        TmerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SectionFKDisciplines", t => t.SectionFKDisciplineId, cascadeDelete: true)
                .ForeignKey("dbo.Tmer", t => t.TmerId)
                .Index(t => t.SectionFKDisciplineId)
                .Index(t => t.TmerId);
            
            CreateTable(
                "dbo.SectionFKTeches",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 127),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SectionFKSubgroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Limit = c.Int(nullable: false),
                        InnerNumber = c.Int(nullable: false),
                        ParentId = c.Int(),
                        MetaSubgroupId = c.Int(nullable: false),
                        ExpectedChildCount = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SectionFKDisciplineTmerPeriods", t => t.MetaSubgroupId, cascadeDelete: true)
                .ForeignKey("dbo.SectionFKSubgroups", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.MetaSubgroupId);
            
            CreateTable(
                "dbo.SectionFKSubgroupMemberships",
                c => new
                    {
                        SubgroupId = c.Int(nullable: false),
                        studentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SubgroupId, t.studentId })
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.SectionFKSubgroups", t => t.SubgroupId, cascadeDelete: true)
                .Index(t => t.SubgroupId)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.SectionFKDisciplineTmerPeriodDivision",
                c => new
                    {
                        SectionFKDisciplineTmerPeriodId = c.Int(nullable: false),
                        DivisionId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.SectionFKDisciplineTmerPeriodId, t.DivisionId })
                .ForeignKey("dbo.SectionFKDisciplineTmerPeriods", t => t.SectionFKDisciplineTmerPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.Divisions", t => t.DivisionId, cascadeDelete: true)
                .Index(t => t.SectionFKDisciplineTmerPeriodId)
                .Index(t => t.DivisionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKDisciplineTmerPeriods", "SectionFKDisciplineTmerId", "dbo.SectionFKDisciplineTmers");
            DropForeignKey("dbo.SectionFKSubgroupMemberships", "SubgroupId", "dbo.SectionFKSubgroups");
            DropForeignKey("dbo.SectionFKSubgroupMemberships", "studentId", "dbo.Students");
            DropForeignKey("dbo.SectionFKSubgroups", "ParentId", "dbo.SectionFKSubgroups");
            DropForeignKey("dbo.SectionFKSubgroups", "MetaSubgroupId", "dbo.SectionFKDisciplineTmerPeriods");
            DropForeignKey("dbo.SectionFKDisciplineTmerPeriods", "SectionFKPeriodId", "dbo.SectionFKPeriods");
            DropForeignKey("dbo.SectionFKPeriods", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.SectionFKPeriods", "SectionFKId", "dbo.SectionFKs");
            DropForeignKey("dbo.SectionFKs", "SectionFKTechId", "dbo.SectionFKTeches");
            DropForeignKey("dbo.SectionFKs", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.SectionFKDisciplineTmers", "TmerId", "dbo.Tmer");
            DropForeignKey("dbo.SectionFKDisciplineTmers", "SectionFKDisciplineId", "dbo.SectionFKDisciplines");
            DropForeignKey("dbo.SectionFKDisciplines", "SectionFKId", "dbo.SectionFKs");
            DropForeignKey("dbo.SectionFKDisciplines", "DisciplineUid", "dbo.Disciplines");
            DropForeignKey("dbo.SectionFKDisciplineTmerPeriodDivision", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.SectionFKDisciplineTmerPeriodDivision", "SectionFKDisciplineTmerPeriodId", "dbo.SectionFKDisciplineTmerPeriods");
            DropIndex("dbo.SectionFKDisciplineTmerPeriodDivision", new[] { "DivisionId" });
            DropIndex("dbo.SectionFKDisciplineTmerPeriodDivision", new[] { "SectionFKDisciplineTmerPeriodId" });
            DropIndex("dbo.SectionFKSubgroupMemberships", new[] { "studentId" });
            DropIndex("dbo.SectionFKSubgroupMemberships", new[] { "SubgroupId" });
            DropIndex("dbo.SectionFKSubgroups", new[] { "MetaSubgroupId" });
            DropIndex("dbo.SectionFKSubgroups", new[] { "ParentId" });
            DropIndex("dbo.SectionFKDisciplineTmers", new[] { "TmerId" });
            DropIndex("dbo.SectionFKDisciplineTmers", new[] { "SectionFKDisciplineId" });
            DropIndex("dbo.SectionFKDisciplines", new[] { "DisciplineUid" });
            DropIndex("dbo.SectionFKDisciplines", new[] { "SectionFKId" });
            DropIndex("dbo.SectionFKs", new[] { "SectionFKTechId" });
            DropIndex("dbo.SectionFKs", new[] { "ModuleId" });
            DropIndex("dbo.SectionFKPeriods", new[] { "SemesterId" });
            DropIndex("dbo.SectionFKPeriods", new[] { "SectionFKId" });
            DropIndex("dbo.SectionFKDisciplineTmerPeriods", new[] { "SectionFKPeriodId" });
            DropIndex("dbo.SectionFKDisciplineTmerPeriods", new[] { "SectionFKDisciplineTmerId" });
            DropTable("dbo.SectionFKDisciplineTmerPeriodDivision");
            DropTable("dbo.SectionFKSubgroupMemberships");
            DropTable("dbo.SectionFKSubgroups");
            DropTable("dbo.SectionFKTeches");
            DropTable("dbo.SectionFKDisciplineTmers");
            DropTable("dbo.SectionFKDisciplines");
            DropTable("dbo.SectionFKs");
            DropTable("dbo.SectionFKPeriods");
            DropTable("dbo.SectionFKDisciplineTmerPeriods");
        }
    }
}
