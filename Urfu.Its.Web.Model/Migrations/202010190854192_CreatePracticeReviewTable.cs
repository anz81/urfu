namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePracticeReviewTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeReviews",
                c => new
                    {
                        PracticeId = c.Int(nullable: false),
                        Events = c.String(),
                        Description = c.String(),
                        Employment = c.Boolean(nullable: false),
                        FuturePractice = c.Boolean(nullable: false),
                        FutureEmployment = c.Boolean(nullable: false),
                        Suggestions = c.String(),
                        Score = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PracticeId)
                .ForeignKey("dbo.Practices", t => t.PracticeId)
                .Index(t => t.PracticeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeReviews", "PracticeId", "dbo.Practices");
            DropIndex("dbo.PracticeReviews", new[] { "PracticeId" });
            DropTable("dbo.PracticeReviews");
        }
    }
}
