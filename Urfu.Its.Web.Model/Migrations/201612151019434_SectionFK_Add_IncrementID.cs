namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFK_Add_IncrementID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SectionFKProperties", "SectionFKId", "dbo.SectionFKs");
            DropIndex("dbo.SectionFKProperties", new[] { "SectionFKId" });
            DropPrimaryKey("dbo.SectionFKProperties");
            AddColumn("dbo.SectionFKProperties", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.SectionFKProperties", "SectionFKId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.SectionFKProperties", "Id");
            CreateIndex("dbo.SectionFKProperties", "SectionFKId");
            AddForeignKey("dbo.SectionFKProperties", "SectionFKId", "dbo.SectionFKs", "ModuleId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKProperties", "SectionFKId", "dbo.SectionFKs");
            DropIndex("dbo.SectionFKProperties", new[] { "SectionFKId" });
            DropPrimaryKey("dbo.SectionFKProperties");
            AlterColumn("dbo.SectionFKProperties", "SectionFKId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.SectionFKProperties", "Id");
            AddPrimaryKey("dbo.SectionFKProperties", new[] { "SectionFKCompetitionGroupId", "SectionFKId" });
            CreateIndex("dbo.SectionFKProperties", "SectionFKId");
            AddForeignKey("dbo.SectionFKProperties", "SectionFKId", "dbo.SectionFKs", "ModuleId", cascadeDelete: true);
        }
    }
}
