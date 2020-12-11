namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentSelectionMinorRenameField : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudentMinorSelectionPriorities", "minorPeriodId", "dbo.MinorPeriods");
            DropIndex("dbo.StudentMinorSelectionPriorities", new[] { "minorPeriodId" });
            CreateTable(
                "dbo.StudentSelectionMinorPriorities",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 127),
                        minorPeriodId = c.Int(nullable: false),
                        minornumber = c.Int(nullable: false),
                        priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.minorPeriodId })
                .ForeignKey("dbo.MinorPeriods", t => t.minorPeriodId)
                .Index(t => t.minorPeriodId);
            
            DropTable("dbo.StudentMinorSelectionPriorities");
        }
        
        public override void Down()
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
                .PrimaryKey(t => new { t.studentId, t.minorPeriodId });
            
            DropForeignKey("dbo.StudentSelectionMinorPriorities", "minorPeriodId", "dbo.MinorPeriods");
            DropIndex("dbo.StudentSelectionMinorPriorities", new[] { "minorPeriodId" });
            DropTable("dbo.StudentSelectionMinorPriorities");
            CreateIndex("dbo.StudentMinorSelectionPriorities", "minorPeriodId");
            AddForeignKey("dbo.StudentMinorSelectionPriorities", "minorPeriodId", "dbo.MinorPeriods", "Id");
        }
    }
}
