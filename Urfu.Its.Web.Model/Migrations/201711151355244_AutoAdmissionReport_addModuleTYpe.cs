namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutoAdmissionReport_addModuleTYpe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorAutoAdmissionReports", "ModuleType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MinorAutoAdmissionReports", "ModuleType");
        }
    }
}
