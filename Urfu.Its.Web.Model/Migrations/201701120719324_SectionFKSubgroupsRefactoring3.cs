namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKSubgroupsRefactoring3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SectionFKSubgroups", "MetaSubgroupId", "dbo.SectionFKDisciplineTmerPeriods");
            DropIndex("dbo.SectionFKSubgroups", new[] { "MetaSubgroupId" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKSubgroups", "Meta_Id", "dbo.SectionFKSubgroupCounts");
            RenameIndex(table: "dbo.SectionFKSubgroups", name: "IX_Meta_Id", newName: "IX_SectionFKSubgroupCount_Id");
            RenameColumn(table: "dbo.SectionFKSubgroups", name: "Meta_Id", newName: "SectionFKSubgroupCount_Id");
            CreateIndex("dbo.SectionFKSubgroups", "MetaSubgroupId");
            AddForeignKey("dbo.SectionFKSubgroups", "MetaSubgroupId", "dbo.SectionFKSubgroupCounts", "Id");
        }
    }
}
