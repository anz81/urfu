namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentMinorSelectionPriority : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentMinorSelectionPriorities",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 127),
                        minorPeriodId = c.Int(nullable: false),
                        minornumber = c.Int(nullable: false),
                        proprity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.minorPeriodId })
                .ForeignKey("dbo.MinorPeriods", t => t.minorPeriodId)
                .Index(t => t.minorPeriodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentMinorSelectionPriorities", "minorPeriodId", "dbo.MinorPeriods");
            DropIndex("dbo.StudentMinorSelectionPriorities", new[] { "minorPeriodId" });
            DropTable("dbo.StudentMinorSelectionPriorities");
        }
    }
}
