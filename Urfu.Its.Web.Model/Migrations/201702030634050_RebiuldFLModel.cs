namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RebiuldFLModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ForeignLanguageAdmissions", "ForeignLanguagePeriodId", "dbo.ForeignLanguagePeriods");
            DropForeignKey("dbo.ForeignLanguageLimits", "ForeignLanguageId", "dbo.ForeignLanguages");
            DropForeignKey("dbo.ForeignLanguageLimits", "ForeignLanguageCompetitionGroupId", "dbo.ForeignLanguageCompetitionGroups");
            DropForeignKey("dbo.ForeignLanguageSubgroups", "MetaSubgroupId", "dbo.ForeignLanguageDisciplineTmerPeriods");
            DropIndex("dbo.ForeignLanguageAdmissions", new[] { "ForeignLanguagePeriodId" });
            DropIndex("dbo.ForeignLanguageLimits", new[] { "ForeignLanguageCompetitionGroupId" });
            DropIndex("dbo.ForeignLanguageLimits", new[] { "ForeignLanguageId" });
            RenameColumn(table: "dbo.ForeignLanguageSubgroups", name: "MetaSubgroupId", newName: "SubgroupCountId");
            RenameIndex(table: "dbo.ForeignLanguageSubgroups", name: "IX_MetaSubgroupId", newName: "IX_SubgroupCountId");
            DropPrimaryKey("dbo.ForeignLanguageAdmissions");
            CreateTable(
                "dbo.ForeignLanguageProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForeignLanguageCompetitionGroupId = c.Int(nullable: false),
                        ForeignLanguageId = c.String(maxLength: 128),
                        Limit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForeignLanguages", t => t.ForeignLanguageId)
                .ForeignKey("dbo.ForeignLanguageCompetitionGroups", t => t.ForeignLanguageCompetitionGroupId, cascadeDelete: true)
                .Index(t => t.ForeignLanguageCompetitionGroupId)
                .Index(t => t.ForeignLanguageId);
            
            CreateTable(
                "dbo.ForeignLanguageSubgroupCounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForeignLanguageDisciplineTmerPeriodId = c.Int(nullable: false),
                        CompetitionGroupId = c.Int(nullable: false),
                        GroupCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForeignLanguageCompetitionGroups", t => t.CompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.ForeignLanguageDisciplineTmerPeriods", t => t.ForeignLanguageDisciplineTmerPeriodId)
                .Index(t => t.ForeignLanguageDisciplineTmerPeriodId)
                .Index(t => t.CompetitionGroupId);
            
            CreateTable(
                "dbo.ForeignLanguageTeachers",
                c => new
                    {
                        ForeignLanguagePropertyId = c.Int(nullable: false),
                        TeacherId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.ForeignLanguagePropertyId, t.TeacherId })
                .ForeignKey("dbo.ForeignLanguageProperties", t => t.ForeignLanguagePropertyId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.ForeignLanguagePropertyId)
                .Index(t => t.TeacherId);
            
            AddColumn("dbo.ForeignLanguageAdmissions", "ForeignLanguageCompetitionGroupId", c => c.Int(nullable: false));
            AddColumn("dbo.ForeignLanguageAdmissions", "ForeignLanguageId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.ForeignLanguagePeriods", "Male", c => c.Boolean());
            AddColumn("dbo.ForeignLanguageSubgroups", "TeacherId", c => c.String(maxLength: 127));
            AlterColumn("dbo.ForeignLanguageCompetitionGroups", "Name", c => c.String(nullable: false));
            AddPrimaryKey("dbo.ForeignLanguageAdmissions", new[] { "studentId", "ForeignLanguageCompetitionGroupId", "ForeignLanguageId" });
            CreateIndex("dbo.ForeignLanguageAdmissions", new[] { "ForeignLanguageId", "ForeignLanguageCompetitionGroupId", "Status" }, name: "IX_ForeignLanguageAdmission_Count");
            CreateIndex("dbo.ForeignLanguageAdmissions", "ForeignLanguageCompetitionGroupId", name: "IX_ForeignLanguageAdmission_ForeignLanguageCompetitionGroupId");
            CreateIndex("dbo.ForeignLanguageSubgroups", "TeacherId");
            AddForeignKey("dbo.ForeignLanguageAdmissions", "ForeignLanguageId", "dbo.ForeignLanguages", "ModuleId", cascadeDelete: true);
            AddForeignKey("dbo.ForeignLanguageAdmissions", "ForeignLanguageCompetitionGroupId", "dbo.ForeignLanguageCompetitionGroups", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForeignLanguageSubgroups", "TeacherId", "dbo.Teachers", "pkey");
            AddForeignKey("dbo.ForeignLanguageSubgroups", "SubgroupCountId", "dbo.ForeignLanguageSubgroupCounts", "Id");
            DropColumn("dbo.ForeignLanguageDisciplineTmerPeriods", "GroupCount");
            DropColumn("dbo.ForeignLanguageAdmissions", "ForeignLanguagePeriodId");
            DropColumn("dbo.ForeignLanguagePeriods", "MinStudentsCount");
            DropColumn("dbo.ForeignLanguagePeriods", "MaxStudentsCount");
            DropTable("dbo.ForeignLanguageLimits");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ForeignLanguageLimits",
                c => new
                    {
                        ForeignLanguageCompetitionGroupId = c.Int(nullable: false),
                        ForeignLanguageId = c.String(nullable: false, maxLength: 128),
                        Limit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ForeignLanguageCompetitionGroupId, t.ForeignLanguageId });
            
            AddColumn("dbo.ForeignLanguagePeriods", "MaxStudentsCount", c => c.Int(nullable: false));
            AddColumn("dbo.ForeignLanguagePeriods", "MinStudentsCount", c => c.Int(nullable: false));
            AddColumn("dbo.ForeignLanguageAdmissions", "ForeignLanguagePeriodId", c => c.Int(nullable: false));
            AddColumn("dbo.ForeignLanguageDisciplineTmerPeriods", "GroupCount", c => c.Int(nullable: false));
            DropForeignKey("dbo.ForeignLanguageSubgroups", "SubgroupCountId", "dbo.ForeignLanguageSubgroupCounts");
            DropForeignKey("dbo.ForeignLanguageSubgroups", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.ForeignLanguageSubgroupCounts", "ForeignLanguageDisciplineTmerPeriodId", "dbo.ForeignLanguageDisciplineTmerPeriods");
            DropForeignKey("dbo.ForeignLanguageSubgroupCounts", "CompetitionGroupId", "dbo.ForeignLanguageCompetitionGroups");
            DropForeignKey("dbo.ForeignLanguageAdmissions", "ForeignLanguageCompetitionGroupId", "dbo.ForeignLanguageCompetitionGroups");
            DropForeignKey("dbo.ForeignLanguageTeachers", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.ForeignLanguageTeachers", "ForeignLanguagePropertyId", "dbo.ForeignLanguageProperties");
            DropForeignKey("dbo.ForeignLanguageProperties", "ForeignLanguageCompetitionGroupId", "dbo.ForeignLanguageCompetitionGroups");
            DropForeignKey("dbo.ForeignLanguageProperties", "ForeignLanguageId", "dbo.ForeignLanguages");
            DropForeignKey("dbo.ForeignLanguageAdmissions", "ForeignLanguageId", "dbo.ForeignLanguages");
            DropIndex("dbo.ForeignLanguageTeachers", new[] { "TeacherId" });
            DropIndex("dbo.ForeignLanguageTeachers", new[] { "ForeignLanguagePropertyId" });
            DropIndex("dbo.ForeignLanguageSubgroups", new[] { "TeacherId" });
            DropIndex("dbo.ForeignLanguageSubgroupCounts", new[] { "CompetitionGroupId" });
            DropIndex("dbo.ForeignLanguageSubgroupCounts", new[] { "ForeignLanguageDisciplineTmerPeriodId" });
            DropIndex("dbo.ForeignLanguageProperties", new[] { "ForeignLanguageId" });
            DropIndex("dbo.ForeignLanguageProperties", new[] { "ForeignLanguageCompetitionGroupId" });
            DropIndex("dbo.ForeignLanguageAdmissions", "IX_ForeignLanguageAdmission_ForeignLanguageCompetitionGroupId");
            DropIndex("dbo.ForeignLanguageAdmissions", "IX_ForeignLanguageAdmission_Count");
            DropPrimaryKey("dbo.ForeignLanguageAdmissions");
            AlterColumn("dbo.ForeignLanguageCompetitionGroups", "Name", c => c.String());
            DropColumn("dbo.ForeignLanguageSubgroups", "TeacherId");
            DropColumn("dbo.ForeignLanguagePeriods", "Male");
            DropColumn("dbo.ForeignLanguageAdmissions", "ForeignLanguageId");
            DropColumn("dbo.ForeignLanguageAdmissions", "ForeignLanguageCompetitionGroupId");
            DropTable("dbo.ForeignLanguageTeachers");
            DropTable("dbo.ForeignLanguageSubgroupCounts");
            DropTable("dbo.ForeignLanguageProperties");
            AddPrimaryKey("dbo.ForeignLanguageAdmissions", new[] { "studentId", "ForeignLanguagePeriodId" });
            RenameIndex(table: "dbo.ForeignLanguageSubgroups", name: "IX_SubgroupCountId", newName: "IX_MetaSubgroupId");
            RenameColumn(table: "dbo.ForeignLanguageSubgroups", name: "SubgroupCountId", newName: "MetaSubgroupId");
            CreateIndex("dbo.ForeignLanguageLimits", "ForeignLanguageId");
            CreateIndex("dbo.ForeignLanguageLimits", "ForeignLanguageCompetitionGroupId");
            CreateIndex("dbo.ForeignLanguageAdmissions", "ForeignLanguagePeriodId");
            AddForeignKey("dbo.ForeignLanguageSubgroups", "MetaSubgroupId", "dbo.ForeignLanguageDisciplineTmerPeriods", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForeignLanguageLimits", "ForeignLanguageCompetitionGroupId", "dbo.ForeignLanguageCompetitionGroups", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForeignLanguageLimits", "ForeignLanguageId", "dbo.ForeignLanguages", "ModuleId", cascadeDelete: true);
            AddForeignKey("dbo.ForeignLanguageAdmissions", "ForeignLanguagePeriodId", "dbo.ForeignLanguagePeriods", "Id", cascadeDelete: true);
        }
    }
}
