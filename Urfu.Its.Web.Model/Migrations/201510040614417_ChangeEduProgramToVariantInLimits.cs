namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeEduProgramToVariantInLimits : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EduProgramLimits", "EduProgramId", "dbo.EduPrograms");
            DropIndex("dbo.EduProgramLimits", "IX_EduProgramLimit_Unique");
            AddColumn("dbo.EduProgramLimits", "VariantId", c => c.Int(nullable: false));

            Sql("UPDATE limits SET limits.VariantId = programs.VariantId FROM EduProgramLimits limits INNER JOIN EduPrograms programs ON limits.EduProgramId = programs.Id");
            
            CreateIndex("dbo.EduProgramLimits", new[] { "VariantId", "ModuleId" }, unique: true, name: "IX_EduProgramLimit_Unique");
            AddForeignKey("dbo.EduProgramLimits", "VariantId", "dbo.Variants", "Id", cascadeDelete: true);
            DropColumn("dbo.EduProgramLimits", "EduProgramId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EduProgramLimits", "EduProgramId", c => c.Int(nullable: false));
            DropForeignKey("dbo.EduProgramLimits", "VariantId", "dbo.Variants");
            DropIndex("dbo.EduProgramLimits", "IX_EduProgramLimit_Unique");
            DropColumn("dbo.EduProgramLimits", "VariantId");
            CreateIndex("dbo.EduProgramLimits", new[] { "EduProgramId", "ModuleId" }, unique: true, name: "IX_EduProgramLimit_Unique");
            AddForeignKey("dbo.EduProgramLimits", "EduProgramId", "dbo.EduPrograms", "Id", cascadeDelete: true);
        }
    }
}
