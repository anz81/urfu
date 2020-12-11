namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropObsoleFieldInSectionFKSubgroups : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE SectionFKSubgroups DROP CONSTRAINT [FK_dbo.SectionFKSubgroups_dbo.SectionFKSubgroupCounts_MetaSubgroupId]");
            DropColumn("dbo.SectionFKSubgroups", "MetaSubgroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SectionFKSubgroups", "MetaSubgroupId", c => c.Int(nullable: false));
        }
    }
}
