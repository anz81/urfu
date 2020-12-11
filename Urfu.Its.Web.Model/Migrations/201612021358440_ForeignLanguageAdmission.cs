namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignLanguageAdmission : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForeignLanguageAdmissions",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        ForeignLanguagePeriodId = c.Int(nullable: false),
                        Published = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.ForeignLanguagePeriodId })
                .ForeignKey("dbo.ForeignLanguagePeriods", t => t.ForeignLanguagePeriodId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.ForeignLanguagePeriodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForeignLanguageAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.ForeignLanguageAdmissions", "ForeignLanguagePeriodId", "dbo.ForeignLanguagePeriods");
            DropIndex("dbo.ForeignLanguageAdmissions", new[] { "ForeignLanguagePeriodId" });
            DropIndex("dbo.ForeignLanguageAdmissions", new[] { "studentId" });
            DropTable("dbo.ForeignLanguageAdmissions");
        }
    }
}
