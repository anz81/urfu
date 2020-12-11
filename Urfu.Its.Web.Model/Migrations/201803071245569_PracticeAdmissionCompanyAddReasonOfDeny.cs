namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeAdmissionCompanyAddReasonOfDeny : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeAdmissionCompanys", "ReasonOfDeny", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeAdmissionCompanys", "ReasonOfDeny");
        }
    }
}
