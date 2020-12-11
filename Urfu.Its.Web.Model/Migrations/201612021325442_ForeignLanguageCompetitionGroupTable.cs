namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignLanguageCompetitionGroupTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForeignLanguageCompetitionGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StudentCourse = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.ForeignLanguageLimits",
                c => new
                    {
                        ForeignLanguageCompetitionGroupId = c.Int(nullable: false),
                        ForeignLanguageId = c.String(nullable: false, maxLength: 128),
                        Limit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ForeignLanguageCompetitionGroupId, t.ForeignLanguageId })
                .ForeignKey("dbo.ForeignLanguages", t => t.ForeignLanguageId, cascadeDelete: true)
                .ForeignKey("dbo.ForeignLanguageCompetitionGroups", t => t.ForeignLanguageCompetitionGroupId, cascadeDelete: true)
                .Index(t => t.ForeignLanguageCompetitionGroupId)
                .Index(t => t.ForeignLanguageId);
            
            CreateTable(
                "dbo.ForeignLanguageCompetitionGroupContents",
                c => new
                    {
                        ForeignLanguageCompetitionGroupId = c.Int(nullable: false),
                        GroupId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ForeignLanguageCompetitionGroupId, t.GroupId })
                .ForeignKey("dbo.ForeignLanguageCompetitionGroups", t => t.ForeignLanguageCompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.ForeignLanguageCompetitionGroupId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForeignLanguageCompetitionGroups", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.ForeignLanguageCompetitionGroupContents", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.ForeignLanguageCompetitionGroupContents", "ForeignLanguageCompetitionGroupId", "dbo.ForeignLanguageCompetitionGroups");
            DropForeignKey("dbo.ForeignLanguageLimits", "ForeignLanguageCompetitionGroupId", "dbo.ForeignLanguageCompetitionGroups");
            DropForeignKey("dbo.ForeignLanguageLimits", "ForeignLanguageId", "dbo.ForeignLanguages");
            DropIndex("dbo.ForeignLanguageCompetitionGroupContents", new[] { "GroupId" });
            DropIndex("dbo.ForeignLanguageCompetitionGroupContents", new[] { "ForeignLanguageCompetitionGroupId" });
            DropIndex("dbo.ForeignLanguageLimits", new[] { "ForeignLanguageId" });
            DropIndex("dbo.ForeignLanguageLimits", new[] { "ForeignLanguageCompetitionGroupId" });
            DropIndex("dbo.ForeignLanguageCompetitionGroups", new[] { "SemesterId" });
            DropTable("dbo.ForeignLanguageCompetitionGroupContents");
            DropTable("dbo.ForeignLanguageLimits");
            DropTable("dbo.ForeignLanguageCompetitionGroups");
        }
    }
}
