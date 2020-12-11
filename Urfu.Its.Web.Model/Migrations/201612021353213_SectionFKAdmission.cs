namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKAdmission : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SectionFKAdmissions",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        SectionFKPeriodId = c.Int(nullable: false),
                        Published = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.SectionFKPeriodId })
                .ForeignKey("dbo.SectionFKPeriods", t => t.SectionFKPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.SectionFKPeriodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.SectionFKAdmissions", "SectionFKPeriodId", "dbo.SectionFKPeriods");
            DropIndex("dbo.SectionFKAdmissions", new[] { "SectionFKPeriodId" });
            DropIndex("dbo.SectionFKAdmissions", new[] { "studentId" });
            DropTable("dbo.SectionFKAdmissions");
        }
    }
}
