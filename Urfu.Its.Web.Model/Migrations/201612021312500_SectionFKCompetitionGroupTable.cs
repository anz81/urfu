namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKCompetitionGroupTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SectionFKCompetitionGroups",
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
                "dbo.SectionFKLimits",
                c => new
                    {
                        SectionFKCompetitionGroupId = c.Int(nullable: false),
                        SectionFKId = c.String(nullable: false, maxLength: 128),
                        Limit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SectionFKCompetitionGroupId, t.SectionFKId })
                .ForeignKey("dbo.SectionFKs", t => t.SectionFKId, cascadeDelete: true)
                .ForeignKey("dbo.SectionFKCompetitionGroups", t => t.SectionFKCompetitionGroupId, cascadeDelete: true)
                .Index(t => t.SectionFKCompetitionGroupId)
                .Index(t => t.SectionFKId);
            
            CreateTable(
                "dbo.SectionFKCompetitionGroupContents",
                c => new
                    {
                        SectionFKCompetitionGroupId = c.Int(nullable: false),
                        GroupId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SectionFKCompetitionGroupId, t.GroupId })
                .ForeignKey("dbo.SectionFKCompetitionGroups", t => t.SectionFKCompetitionGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.SectionFKCompetitionGroupId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKCompetitionGroups", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.SectionFKLimits", "SectionFKCompetitionGroupId", "dbo.SectionFKCompetitionGroups");
            DropForeignKey("dbo.SectionFKLimits", "SectionFKId", "dbo.SectionFKs");
            DropForeignKey("dbo.SectionFKCompetitionGroupContents", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.SectionFKCompetitionGroupContents", "SectionFKCompetitionGroupId", "dbo.SectionFKCompetitionGroups");
            DropIndex("dbo.SectionFKCompetitionGroupContents", new[] { "GroupId" });
            DropIndex("dbo.SectionFKCompetitionGroupContents", new[] { "SectionFKCompetitionGroupId" });
            DropIndex("dbo.SectionFKLimits", new[] { "SectionFKId" });
            DropIndex("dbo.SectionFKLimits", new[] { "SectionFKCompetitionGroupId" });
            DropIndex("dbo.SectionFKCompetitionGroups", new[] { "SemesterId" });
            DropTable("dbo.SectionFKCompetitionGroupContents");
            DropTable("dbo.SectionFKLimits");
            DropTable("dbo.SectionFKCompetitionGroups");
        }
    }
}
