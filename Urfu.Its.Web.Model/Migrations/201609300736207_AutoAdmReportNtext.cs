namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutoAdmReportNtext : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MinorAutoAdmissionReports", "Content", c => c.String(storeType: "ntext"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MinorAutoAdmissionReports", "Content", c => c.String());
        }
    }
}
