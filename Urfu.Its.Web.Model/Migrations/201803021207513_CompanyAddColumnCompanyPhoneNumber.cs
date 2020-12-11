namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyAddColumnCompanyPhoneNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "CompanyPhoneNumber", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "CompanyPhoneNumber");
        }
    }
}
