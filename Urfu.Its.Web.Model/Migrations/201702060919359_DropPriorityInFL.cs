namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropPriorityInFL : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId", "dbo.ForeignLanguages");
            DropIndex("dbo.ForeignLanguageStudentSelectionPriorities", new[] { "sectionId" });
            DropPrimaryKey("dbo.ForeignLanguageStudentSelectionPriorities");
            AlterColumn("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.ForeignLanguageStudentSelectionPriorities", new[] { "studentId", "competitionGroupId" });
            CreateIndex("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId");
            AddForeignKey("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId", "dbo.ForeignLanguages", "ModuleId");
            DropColumn("dbo.ForeignLanguageStudentSelectionPriorities", "priority");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForeignLanguageStudentSelectionPriorities", "priority", c => c.Int());
            DropForeignKey("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId", "dbo.ForeignLanguages");
            DropIndex("dbo.ForeignLanguageStudentSelectionPriorities", new[] { "sectionId" });
            DropPrimaryKey("dbo.ForeignLanguageStudentSelectionPriorities");
            AlterColumn("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.ForeignLanguageStudentSelectionPriorities", new[] { "studentId", "competitionGroupId", "sectionId" });
            CreateIndex("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId");
            AddForeignKey("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId", "dbo.ForeignLanguages", "ModuleId", cascadeDelete: true);
        }
    }
}
