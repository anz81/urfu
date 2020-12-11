namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatesToPracticeAdmissions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeAdmissionCompanys", "Dates", c => c.String());
            AddColumn("dbo.PracticeAdmissions", "Dates", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeAdmissions", "Dates");
            DropColumn("dbo.PracticeAdmissionCompanys", "Dates");
        }
    }
}
