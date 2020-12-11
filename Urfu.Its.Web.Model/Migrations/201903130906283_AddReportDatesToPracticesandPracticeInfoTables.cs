namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportDatesToPracticesandPracticeInfoTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Practices", "ReportBeginDate", c => c.DateTime());
            AddColumn("dbo.Practices", "ReportEndDate", c => c.DateTime());
            AddColumn("dbo.PracticeInfo", "ReportBeginDate", c => c.DateTime());
            AddColumn("dbo.PracticeInfo", "ReportEndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeInfo", "ReportEndDate");
            DropColumn("dbo.PracticeInfo", "ReportBeginDate");
            DropColumn("dbo.Practices", "ReportEndDate");
            DropColumn("dbo.Practices", "ReportBeginDate");
        }
    }
}
