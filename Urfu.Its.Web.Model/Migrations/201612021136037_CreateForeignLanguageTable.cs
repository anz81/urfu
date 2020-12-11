namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateForeignLanguageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForeignLanguageDisciplineTmerPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForeignLanguageDisciplineTmerId = c.Int(nullable: false),
                        ForeignLanguagePeriodId = c.Int(nullable: false),
                        GroupCount = c.Int(nullable: false),
                        Distribution = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForeignLanguagePeriods", t => t.ForeignLanguagePeriodId, cascadeDelete: true)
                .ForeignKey("dbo.ForeignLanguageDisciplineTmers", t => t.ForeignLanguageDisciplineTmerId, cascadeDelete: true)
                .Index(t => t.ForeignLanguageDisciplineTmerId)
                .Index(t => t.ForeignLanguagePeriodId);
            
            CreateTable(
                "dbo.ForeignLanguagePeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForeignLanguageId = c.String(maxLength: 128),
                        Year = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                        SelectionDeadline = c.DateTime(),
                        MinStudentsCount = c.Int(nullable: false),
                        MaxStudentsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForeignLanguages", t => t.ForeignLanguageId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.ForeignLanguageId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.ForeignLanguages",
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
                "dbo.ForeignLanguageDisciplines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForeignLanguageId = c.String(maxLength: 128),
                        DisciplineUid = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineUid)
                .ForeignKey("dbo.ForeignLanguages", t => t.ForeignLanguageId)
                .Index(t => t.ForeignLanguageId)
                .Index(t => t.DisciplineUid);
            
            CreateTable(
                "dbo.ForeignLanguageDisciplineTmers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForeignLanguageDisciplineId = c.Int(nullable: false),
                        TmerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForeignLanguageDisciplines", t => t.ForeignLanguageDisciplineId, cascadeDelete: true)
                .ForeignKey("dbo.Tmer", t => t.TmerId)
                .Index(t => t.ForeignLanguageDisciplineId)
                .Index(t => t.TmerId);
            
            CreateTable(
                "dbo.ForeignLanguageSubgroups",
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
                .ForeignKey("dbo.ForeignLanguageDisciplineTmerPeriods", t => t.MetaSubgroupId, cascadeDelete: true)
                .ForeignKey("dbo.ForeignLanguageSubgroups", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.MetaSubgroupId);
            
            CreateTable(
                "dbo.ForeignLanguageSubgroupMemberships",
                c => new
                    {
                        SubgroupId = c.Int(nullable: false),
                        studentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SubgroupId, t.studentId })
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.ForeignLanguageSubgroups", t => t.SubgroupId, cascadeDelete: true)
                .Index(t => t.SubgroupId)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.ForeignLanguageDisciplineTmerPeriodDivision",
                c => new
                    {
                        ForeignLanguageDisciplineTmerPeriodId = c.Int(nullable: false),
                        DivisionId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.ForeignLanguageDisciplineTmerPeriodId, t.DivisionId })
                .ForeignKey("dbo.ForeignLanguageDisciplineTmerPeriods", t => t.ForeignLanguageDisciplineTmerPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.Divisions", t => t.DivisionId, cascadeDelete: true)
                .Index(t => t.ForeignLanguageDisciplineTmerPeriodId)
                .Index(t => t.DivisionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForeignLanguageDisciplineTmerPeriods", "ForeignLanguageDisciplineTmerId", "dbo.ForeignLanguageDisciplineTmers");
            DropForeignKey("dbo.ForeignLanguageSubgroupMemberships", "SubgroupId", "dbo.ForeignLanguageSubgroups");
            DropForeignKey("dbo.ForeignLanguageSubgroupMemberships", "studentId", "dbo.Students");
            DropForeignKey("dbo.ForeignLanguageSubgroups", "ParentId", "dbo.ForeignLanguageSubgroups");
            DropForeignKey("dbo.ForeignLanguageSubgroups", "MetaSubgroupId", "dbo.ForeignLanguageDisciplineTmerPeriods");
            DropForeignKey("dbo.ForeignLanguageDisciplineTmerPeriods", "ForeignLanguagePeriodId", "dbo.ForeignLanguagePeriods");
            DropForeignKey("dbo.ForeignLanguagePeriods", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.ForeignLanguagePeriods", "ForeignLanguageId", "dbo.ForeignLanguages");
            DropForeignKey("dbo.ForeignLanguages", "ModuleTechId", "dbo.ModuleTeches");
            DropForeignKey("dbo.ForeignLanguages", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.ForeignLanguageDisciplineTmers", "TmerId", "dbo.Tmer");
            DropForeignKey("dbo.ForeignLanguageDisciplineTmers", "ForeignLanguageDisciplineId", "dbo.ForeignLanguageDisciplines");
            DropForeignKey("dbo.ForeignLanguageDisciplines", "ForeignLanguageId", "dbo.ForeignLanguages");
            DropForeignKey("dbo.ForeignLanguageDisciplines", "DisciplineUid", "dbo.Disciplines");
            DropForeignKey("dbo.ForeignLanguageDisciplineTmerPeriodDivision", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.ForeignLanguageDisciplineTmerPeriodDivision", "ForeignLanguageDisciplineTmerPeriodId", "dbo.ForeignLanguageDisciplineTmerPeriods");
            DropIndex("dbo.ForeignLanguageDisciplineTmerPeriodDivision", new[] { "DivisionId" });
            DropIndex("dbo.ForeignLanguageDisciplineTmerPeriodDivision", new[] { "ForeignLanguageDisciplineTmerPeriodId" });
            DropIndex("dbo.ForeignLanguageSubgroupMemberships", new[] { "studentId" });
            DropIndex("dbo.ForeignLanguageSubgroupMemberships", new[] { "SubgroupId" });
            DropIndex("dbo.ForeignLanguageSubgroups", new[] { "MetaSubgroupId" });
            DropIndex("dbo.ForeignLanguageSubgroups", new[] { "ParentId" });
            DropIndex("dbo.ForeignLanguageDisciplineTmers", new[] { "TmerId" });
            DropIndex("dbo.ForeignLanguageDisciplineTmers", new[] { "ForeignLanguageDisciplineId" });
            DropIndex("dbo.ForeignLanguageDisciplines", new[] { "DisciplineUid" });
            DropIndex("dbo.ForeignLanguageDisciplines", new[] { "ForeignLanguageId" });
            DropIndex("dbo.ForeignLanguages", new[] { "ModuleTechId" });
            DropIndex("dbo.ForeignLanguages", new[] { "ModuleId" });
            DropIndex("dbo.ForeignLanguagePeriods", new[] { "SemesterId" });
            DropIndex("dbo.ForeignLanguagePeriods", new[] { "ForeignLanguageId" });
            DropIndex("dbo.ForeignLanguageDisciplineTmerPeriods", new[] { "ForeignLanguagePeriodId" });
            DropIndex("dbo.ForeignLanguageDisciplineTmerPeriods", new[] { "ForeignLanguageDisciplineTmerId" });
            DropTable("dbo.ForeignLanguageDisciplineTmerPeriodDivision");
            DropTable("dbo.ForeignLanguageSubgroupMemberships");
            DropTable("dbo.ForeignLanguageSubgroups");
            DropTable("dbo.ForeignLanguageDisciplineTmers");
            DropTable("dbo.ForeignLanguageDisciplines");
            DropTable("dbo.ForeignLanguages");
            DropTable("dbo.ForeignLanguagePeriods");
            DropTable("dbo.ForeignLanguageDisciplineTmerPeriods");
        }
    }
}
