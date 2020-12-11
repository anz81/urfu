namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveYearFromEduProgramLimits : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.EduProgramLimits", "IX_EduProgramLimit_Unique");
            CreateIndex("dbo.EduProgramLimits", new[] { "EduProgramId", "ModuleId" }, unique: true, name: "IX_EduProgramLimit_Unique");
            DropColumn("dbo.EduProgramLimits", "Year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EduProgramLimits", "Year", c => c.Int(nullable: false));
            DropIndex("dbo.EduProgramLimits", "IX_EduProgramLimit_Unique");
            CreateIndex("dbo.EduProgramLimits", new[] { "EduProgramId", "ModuleId", "Year" }, unique: true, name: "IX_EduProgramLimit_Unique");
        }
    }
}
