namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentPlans : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentPlans",
                c => new
                    {
                        StudentId = c.String(nullable: false, maxLength: 128),
                        PlanNumber = c.Int(nullable: false),
                        VersionNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StudentId, t.PlanNumber, t.VersionNumber })
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.StudentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentPlans", "StudentId", "dbo.Students");
            DropIndex("dbo.StudentPlans", new[] { "StudentId" });
            DropTable("dbo.StudentPlans");
        }
    }
}
