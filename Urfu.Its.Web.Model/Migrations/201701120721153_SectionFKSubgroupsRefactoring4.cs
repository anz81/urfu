namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKSubgroupsRefactoring4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SectionFKSubgroups", "SubgroupCountId", c => c.Int(nullable: false));
            AddForeignKey("dbo.SectionFKSubgroups", "SubgroupCountId", "dbo.SectionFKSubgroupCounts", "Id");
            CreateIndex("dbo.SectionFKSubgroups", "SubgroupCountId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SectionFKSubgroups", new[] { "SubgroupCountId" });
            AlterColumn("dbo.SectionFKSubgroups", "SubgroupCountId", c => c.Int());
            RenameColumn(table: "dbo.SectionFKSubgroups", name: "SubgroupCountId", newName: "Meta_Id");
            CreateIndex("dbo.SectionFKSubgroups", "Meta_Id");
        }
    }
}
