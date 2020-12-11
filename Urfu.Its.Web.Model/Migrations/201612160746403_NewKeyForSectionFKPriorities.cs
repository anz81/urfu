namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewKeyForSectionFKPriorities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SectionFKStudentSelectionPriorities", "sectionId", "dbo.SectionFKs");
            DropIndex("dbo.SectionFKStudentSelectionPriorities", new[] { "sectionId" });
            DropPrimaryKey("dbo.SectionFKStudentSelectionPriorities");
            AlterColumn("dbo.SectionFKStudentSelectionPriorities", "sectionId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.SectionFKStudentSelectionPriorities", new[] { "studentId", "competitionGroupId", "sectionId" });
            CreateIndex("dbo.SectionFKStudentSelectionPriorities", "sectionId");
            AddForeignKey("dbo.SectionFKStudentSelectionPriorities", "sectionId", "dbo.SectionFKs", "ModuleId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKStudentSelectionPriorities", "sectionId", "dbo.SectionFKs");
            DropIndex("dbo.SectionFKStudentSelectionPriorities", new[] { "sectionId" });
            DropPrimaryKey("dbo.SectionFKStudentSelectionPriorities");
            AlterColumn("dbo.SectionFKStudentSelectionPriorities", "sectionId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.SectionFKStudentSelectionPriorities", new[] { "studentId", "competitionGroupId" });
            CreateIndex("dbo.SectionFKStudentSelectionPriorities", "sectionId");
            AddForeignKey("dbo.SectionFKStudentSelectionPriorities", "sectionId", "dbo.SectionFKs", "ModuleId");
        }
    }
}
