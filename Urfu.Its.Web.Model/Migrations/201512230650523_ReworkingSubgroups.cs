namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReworkingSubgroups : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Subgroups", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Subgroups", "EduProgramId", "dbo.EduPrograms");
            DropForeignKey("dbo.Subgroups", "VariantId", "dbo.Variants");
            DropIndex("dbo.Subgroups", new[] { "EduProgramId" });
            DropIndex("dbo.Subgroups", new[] { "VariantId" });
            DropIndex("dbo.Subgroups", new[] { "ModuleId" });
            AddColumn("dbo.Subgroups", "VariantContentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Subgroups", "VariantContentId");
            AddForeignKey("dbo.Subgroups", "VariantContentId", "dbo.VariantContents", "Id");
            DropColumn("dbo.Subgroups", "EduProgramId");
            DropColumn("dbo.Subgroups", "VariantId");
            DropColumn("dbo.Subgroups", "ModuleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subgroups", "ModuleId", c => c.String(maxLength: 128));
            AddColumn("dbo.Subgroups", "VariantId", c => c.Int());
            AddColumn("dbo.Subgroups", "EduProgramId", c => c.Int());
            DropForeignKey("dbo.Subgroups", "VariantContentId", "dbo.VariantContents");
            DropIndex("dbo.Subgroups", new[] { "VariantContentId" });
            DropColumn("dbo.Subgroups", "VariantContentId");
            CreateIndex("dbo.Subgroups", "ModuleId");
            CreateIndex("dbo.Subgroups", "VariantId");
            CreateIndex("dbo.Subgroups", "EduProgramId");
            AddForeignKey("dbo.Subgroups", "VariantId", "dbo.Variants", "Id");
            AddForeignKey("dbo.Subgroups", "EduProgramId", "dbo.EduPrograms", "Id");
            AddForeignKey("dbo.Subgroups", "ModuleId", "dbo.Modules", "uuid");
        }
    }
}
