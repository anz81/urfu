namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentMinorLink : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.StudentSelectionMinorPriorities");
            AlterColumn("dbo.StudentSelectionMinorPriorities", "studentId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.StudentSelectionMinorPriorities", new[] { "studentId", "minorPeriodId" });
            CreateIndex("dbo.StudentSelectionMinorPriorities", "studentId");
            AddForeignKey("dbo.StudentSelectionMinorPriorities", "studentId", "dbo.Students", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentSelectionMinorPriorities", "studentId", "dbo.Students");
            DropIndex("dbo.StudentSelectionMinorPriorities", new[] { "studentId" });
            DropPrimaryKey("dbo.StudentSelectionMinorPriorities");
            AlterColumn("dbo.StudentSelectionMinorPriorities", "studentId", c => c.String(nullable: false, maxLength: 127));
            AddPrimaryKey("dbo.StudentSelectionMinorPriorities", new[] { "studentId", "minorPeriodId" });
        }
    }
}
