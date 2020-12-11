namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FKAddTech : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SectionFKs", "ModuleTechId", c => c.Int(nullable: false));
            CreateIndex("dbo.SectionFKs", "ModuleTechId");
            AddForeignKey("dbo.SectionFKs", "ModuleTechId", "dbo.ModuleTeches", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKs", "ModuleTechId", "dbo.ModuleTeches");
            DropIndex("dbo.SectionFKs", new[] { "ModuleTechId" });
            DropColumn("dbo.SectionFKs", "ModuleTechId");
        }
    }
}
