namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgreementToPracticeAdmissionCompanys : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeAdmissionCompanys", "Agreement", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeAdmissionCompanys", "Agreement");
        }
    }
}
