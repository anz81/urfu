namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKSubgroupsRefactoring : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.SectionFKSubgroupCounts");
            AddColumn("dbo.SectionFKSubgroupCounts", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.SectionFKSubgroupCounts", "Id");
            AddForeignKey("dbo.SectionFKSubgroups", "MetaSubgroupId", "dbo.SectionFKSubgroupCounts", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKSubgroups", "MetaSubgroupId", "dbo.SectionFKSubgroupCounts");
            DropPrimaryKey("dbo.SectionFKSubgroupCounts");
            DropColumn("dbo.SectionFKSubgroupCounts", "Id");
            AddPrimaryKey("dbo.SectionFKSubgroupCounts", new[] { "SectionFKDisciplineTmerPeriodId", "CompetitionGroupId" });
        }
    }
}
