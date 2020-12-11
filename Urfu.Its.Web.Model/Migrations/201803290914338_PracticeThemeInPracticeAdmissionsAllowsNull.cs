namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeThemeInPracticeAdmissionsAllowsNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PracticeAdmissions", "PracticeThemeId", "dbo.PracticeThemes");
            DropIndex("dbo.PracticeAdmissions", new[] { "PracticeThemeId" });
            AlterColumn("dbo.PracticeAdmissions", "PracticeThemeId", c => c.Int());
            CreateIndex("dbo.PracticeAdmissions", "PracticeThemeId");
            AddForeignKey("dbo.PracticeAdmissions", "PracticeThemeId", "dbo.PracticeThemes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeAdmissions", "PracticeThemeId", "dbo.PracticeThemes");
            DropIndex("dbo.PracticeAdmissions", new[] { "PracticeThemeId" });
            AlterColumn("dbo.PracticeAdmissions", "PracticeThemeId", c => c.Int(nullable: false));
            CreateIndex("dbo.PracticeAdmissions", "PracticeThemeId");
            AddForeignKey("dbo.PracticeAdmissions", "PracticeThemeId", "dbo.PracticeThemes", "Id", cascadeDelete: true);
        }
    }
}
