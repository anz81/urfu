namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RebiuldFLModel2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForeignLanguageStudentSelectionPriorities",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        competitionGroupId = c.Int(nullable: false),
                        sectionId = c.String(nullable: false, maxLength: 128),
                        priority = c.Int(),
                        modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.competitionGroupId, t.sectionId })
                .ForeignKey("dbo.ForeignLanguageCompetitionGroups", t => t.competitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.ForeignLanguages", t => t.sectionId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.competitionGroupId)
                .Index(t => t.sectionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForeignLanguageStudentSelectionPriorities", "studentId", "dbo.Students");
            DropForeignKey("dbo.ForeignLanguageStudentSelectionPriorities", "sectionId", "dbo.ForeignLanguages");
            DropForeignKey("dbo.ForeignLanguageStudentSelectionPriorities", "competitionGroupId", "dbo.ForeignLanguageCompetitionGroups");
            DropIndex("dbo.ForeignLanguageStudentSelectionPriorities", new[] { "sectionId" });
            DropIndex("dbo.ForeignLanguageStudentSelectionPriorities", new[] { "competitionGroupId" });
            DropIndex("dbo.ForeignLanguageStudentSelectionPriorities", new[] { "studentId" });
            DropTable("dbo.ForeignLanguageStudentSelectionPriorities");
        }
    }
}
