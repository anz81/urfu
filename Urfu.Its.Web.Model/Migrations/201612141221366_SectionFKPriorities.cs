namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKPriorities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SectionFKStudentSelectionPriorities",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        competitionGroupId = c.Int(nullable: false),
                        sectionId = c.String(maxLength: 128),
                        priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.competitionGroupId })
                .ForeignKey("dbo.SectionFKCompetitionGroups", t => t.competitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.SectionFKs", t => t.sectionId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.competitionGroupId)
                .Index(t => t.sectionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKStudentSelectionPriorities", "studentId", "dbo.Students");
            DropForeignKey("dbo.SectionFKStudentSelectionPriorities", "sectionId", "dbo.SectionFKs");
            DropForeignKey("dbo.SectionFKStudentSelectionPriorities", "competitionGroupId", "dbo.SectionFKCompetitionGroups");
            DropIndex("dbo.SectionFKStudentSelectionPriorities", new[] { "sectionId" });
            DropIndex("dbo.SectionFKStudentSelectionPriorities", new[] { "competitionGroupId" });
            DropIndex("dbo.SectionFKStudentSelectionPriorities", new[] { "studentId" });
            DropTable("dbo.SectionFKStudentSelectionPriorities");
        }
    }
}
