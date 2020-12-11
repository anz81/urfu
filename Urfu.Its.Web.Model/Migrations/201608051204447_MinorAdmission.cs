namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorAdmission : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinorAdmissions",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        minorPeriodId = c.Int(nullable: false),
                        Published = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.minorPeriodId })
                .ForeignKey("dbo.MinorPeriods", t => t.minorPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.minorPeriodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinorAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.MinorAdmissions", "minorPeriodId", "dbo.MinorPeriods");
            DropIndex("dbo.MinorAdmissions", new[] { "minorPeriodId" });
            DropIndex("dbo.MinorAdmissions", new[] { "studentId" });
            DropTable("dbo.MinorAdmissions");
        }
    }
}
