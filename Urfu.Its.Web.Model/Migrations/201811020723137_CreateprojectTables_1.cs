namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateprojectTables_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectDisciplineTmerPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectDisciplineTmerId = c.Int(nullable: false),
                        ProjectPeriodId = c.Int(nullable: false),
                        Distribution = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectPeriods", t => t.ProjectPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.ProjectDisciplineTmers", t => t.ProjectDisciplineTmerId, cascadeDelete: true)
                .Index(t => t.ProjectDisciplineTmerId)
                .Index(t => t.ProjectPeriodId);
            
            CreateTable(
                "dbo.ProjectPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.String(maxLength: 128),
                        Year = c.Int(nullable: false),
                        Male = c.Boolean(),
                        SemesterId = c.Int(nullable: false),
                        SelectionBegin = c.DateTime(),
                        SelectionDeadline = c.DateTime(),
                        Course = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        ModuleTechId = c.Int(nullable: false),
                        ShowInLC = c.Boolean(nullable: false),
                        WithoutPriorities = c.Boolean(nullable: false),
                        ContractId = c.Int(),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.ModuleTeches", t => t.ModuleTechId, cascadeDelete: true)
                .Index(t => t.ModuleId)
                .Index(t => t.ModuleTechId)
                .Index(t => t.ContractId);
            
            CreateTable(
                "dbo.ProjectDisciplines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.String(maxLength: 128),
                        DisciplineUid = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineUid)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId)
                .Index(t => t.DisciplineUid);
            
            CreateTable(
                "dbo.ProjectDisciplineTmers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectDisciplineId = c.Int(nullable: false),
                        TmerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectDisciplines", t => t.ProjectDisciplineId, cascadeDelete: true)
                .ForeignKey("dbo.Tmer", t => t.TmerId)
                .Index(t => t.ProjectDisciplineId)
                .Index(t => t.TmerId);
            
            CreateTable(
                "dbo.ProjectAutoMoveReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        FileName = c.String(),
                        Content = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectDisciplineTmerPeriodDivisions",
                c => new
                    {
                        ProjectDisciplineTmerPeriod_Id = c.Int(nullable: false),
                        Division_uuid = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.ProjectDisciplineTmerPeriod_Id, t.Division_uuid })
                .ForeignKey("dbo.ProjectDisciplineTmerPeriods", t => t.ProjectDisciplineTmerPeriod_Id, cascadeDelete: true)
                .ForeignKey("dbo.Divisions", t => t.Division_uuid, cascadeDelete: true)
                .Index(t => t.ProjectDisciplineTmerPeriod_Id)
                .Index(t => t.Division_uuid);
            
            AddColumn("dbo.Companies", "Source", c => c.String(nullable: false, defaultValue: "practice"));
            AddColumn("dbo.Modules", "Source", c => c.String(nullable: false, defaultValue: "uni"));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectDisciplineTmerPeriods", "ProjectDisciplineTmerId", "dbo.ProjectDisciplineTmers");
            DropForeignKey("dbo.ProjectDisciplineTmerPeriods", "ProjectPeriodId", "dbo.ProjectPeriods");
            DropForeignKey("dbo.ProjectPeriods", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.ProjectPeriods", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "ModuleTechId", "dbo.ModuleTeches");
            DropForeignKey("dbo.Projects", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.ProjectDisciplineTmers", "TmerId", "dbo.Tmer");
            DropForeignKey("dbo.ProjectDisciplineTmers", "ProjectDisciplineId", "dbo.ProjectDisciplines");
            DropForeignKey("dbo.ProjectDisciplines", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectDisciplines", "DisciplineUid", "dbo.Disciplines");
            DropForeignKey("dbo.Projects", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.ProjectDisciplineTmerPeriodDivisions", "Division_uuid", "dbo.Divisions");
            DropForeignKey("dbo.ProjectDisciplineTmerPeriodDivisions", "ProjectDisciplineTmerPeriod_Id", "dbo.ProjectDisciplineTmerPeriods");
            DropIndex("dbo.ProjectDisciplineTmerPeriodDivisions", new[] { "Division_uuid" });
            DropIndex("dbo.ProjectDisciplineTmerPeriodDivisions", new[] { "ProjectDisciplineTmerPeriod_Id" });
            DropIndex("dbo.ProjectDisciplineTmers", new[] { "TmerId" });
            DropIndex("dbo.ProjectDisciplineTmers", new[] { "ProjectDisciplineId" });
            DropIndex("dbo.ProjectDisciplines", new[] { "DisciplineUid" });
            DropIndex("dbo.ProjectDisciplines", new[] { "ProjectId" });
            DropIndex("dbo.Projects", new[] { "ContractId" });
            DropIndex("dbo.Projects", new[] { "ModuleTechId" });
            DropIndex("dbo.Projects", new[] { "ModuleId" });
            DropIndex("dbo.ProjectPeriods", new[] { "SemesterId" });
            DropIndex("dbo.ProjectPeriods", new[] { "ProjectId" });
            DropIndex("dbo.ProjectDisciplineTmerPeriods", new[] { "ProjectPeriodId" });
            DropIndex("dbo.ProjectDisciplineTmerPeriods", new[] { "ProjectDisciplineTmerId" });
            DropColumn("dbo.Modules", "Source");
            DropColumn("dbo.Companies", "Source");
            DropTable("dbo.ProjectDisciplineTmerPeriodDivisions");
            DropTable("dbo.ProjectAutoMoveReports");
            DropTable("dbo.ProjectDisciplineTmers");
            DropTable("dbo.ProjectDisciplines");
            DropTable("dbo.Projects");
            DropTable("dbo.ProjectPeriods");
            DropTable("dbo.ProjectDisciplineTmerPeriods");
        }
    }
}
