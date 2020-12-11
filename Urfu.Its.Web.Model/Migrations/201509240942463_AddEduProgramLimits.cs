namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEduProgramLimits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EduProgramLimits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EduProgramId = c.Int(nullable: false),
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        Year = c.Int(nullable: false),
                        StudentsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EduPrograms", t => t.EduProgramId, cascadeDelete: true)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => new { t.EduProgramId, t.ModuleId, t.Year }, unique: true, name: "IX_EduProgramLimit_Unique");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduProgramLimits", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.EduProgramLimits", "EduProgramId", "dbo.EduPrograms");
            DropIndex("dbo.EduProgramLimits", "IX_EduProgramLimit_Unique");
            DropTable("dbo.EduProgramLimits");
        }
    }
}
