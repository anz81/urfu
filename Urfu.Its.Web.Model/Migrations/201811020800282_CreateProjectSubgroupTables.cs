namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateProjectSubgroupTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectSubgroupCounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectDisciplineTmerPeriodId = c.Int(nullable: false),
                        CompetitionGroupId = c.Int(nullable: false),
                        GroupCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectCompetitionGroups", t => t.CompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.ProjectDisciplineTmerPeriods", t => t.ProjectDisciplineTmerPeriodId)
                .Index(t => t.ProjectDisciplineTmerPeriodId)
                .Index(t => t.CompetitionGroupId);
            
            CreateTable(
                "dbo.ProjectSubgroups",
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
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectSubgroupCounts", t => t.SubgroupCountId, cascadeDelete: true)
                .ForeignKey("dbo.ProjectSubgroups", t => t.ParentId)
                .ForeignKey("dbo.Teachers", t => t.TeacherId)
                .Index(t => t.ParentId)
                .Index(t => t.SubgroupCountId)
                .Index(t => t.TeacherId);
            
            CreateTable(
                "dbo.ProjectSubgroupMemberships",
                c => new
                    {
                        SubgroupId = c.Int(nullable: false),
                        studentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SubgroupId, t.studentId })
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.ProjectSubgroups", t => t.SubgroupId, cascadeDelete: true)
                .Index(t => t.SubgroupId)
                .Index(t => t.studentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectSubgroups", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.ProjectSubgroupMemberships", "SubgroupId", "dbo.ProjectSubgroups");
            DropForeignKey("dbo.ProjectSubgroupMemberships", "studentId", "dbo.Students");
            DropForeignKey("dbo.ProjectSubgroups", "ParentId", "dbo.ProjectSubgroups");
            DropForeignKey("dbo.ProjectSubgroups", "SubgroupCountId", "dbo.ProjectSubgroupCounts");
            DropForeignKey("dbo.ProjectSubgroupCounts", "ProjectDisciplineTmerPeriodId", "dbo.ProjectDisciplineTmerPeriods");
            DropForeignKey("dbo.ProjectSubgroupCounts", "CompetitionGroupId", "dbo.ProjectCompetitionGroups");
            DropIndex("dbo.ProjectSubgroupMemberships", new[] { "studentId" });
            DropIndex("dbo.ProjectSubgroupMemberships", new[] { "SubgroupId" });
            DropIndex("dbo.ProjectSubgroups", new[] { "TeacherId" });
            DropIndex("dbo.ProjectSubgroups", new[] { "SubgroupCountId" });
            DropIndex("dbo.ProjectSubgroups", new[] { "ParentId" });
            DropIndex("dbo.ProjectSubgroupCounts", new[] { "CompetitionGroupId" });
            DropIndex("dbo.ProjectSubgroupCounts", new[] { "ProjectDisciplineTmerPeriodId" });
            DropTable("dbo.ProjectSubgroupMemberships");
            DropTable("dbo.ProjectSubgroups");
            DropTable("dbo.ProjectSubgroupCounts");
        }
    }
}
