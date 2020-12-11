namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKSubgroupCount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SectionFKSubgroupCounts",
                c => new
                    {
                        SectionFKDisciplineTmerPeriodId = c.Int(nullable: false),
                        CompetitionGroupId = c.Int(nullable: false),
                        GroupCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SectionFKDisciplineTmerPeriodId, t.CompetitionGroupId })
                .ForeignKey("dbo.SectionFKCompetitionGroups", t => t.CompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.SectionFKDisciplineTmerPeriods", t => t.SectionFKDisciplineTmerPeriodId)
                .Index(t => t.SectionFKDisciplineTmerPeriodId)
                .Index(t => t.CompetitionGroupId);
            
            DropColumn("dbo.SectionFKDisciplineTmerPeriods", "GroupCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SectionFKDisciplineTmerPeriods", "GroupCount", c => c.Int(nullable: false));
            DropForeignKey("dbo.SectionFKSubgroupCounts", "SectionFKDisciplineTmerPeriodId", "dbo.SectionFKDisciplineTmerPeriods");
            DropForeignKey("dbo.SectionFKSubgroupCounts", "CompetitionGroupId", "dbo.SectionFKCompetitionGroups");
            DropIndex("dbo.SectionFKSubgroupCounts", new[] { "CompetitionGroupId" });
            DropIndex("dbo.SectionFKSubgroupCounts", new[] { "SectionFKDisciplineTmerPeriodId" });
            DropTable("dbo.SectionFKSubgroupCounts");
        }
    }
}
