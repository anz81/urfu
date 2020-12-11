namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInitialsToCompaniesAndContracts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "DirectorInitials", c => c.String(maxLength: 40));
            AddColumn("dbo.Companies", "PersonInChargeInitials", c => c.String(maxLength: 40));
            AddColumn("dbo.Contracts", "DirectorInitials", c => c.String(maxLength: 40));
            AddColumn("dbo.Contracts", "PersonInChargeInitials", c => c.String(maxLength: 40));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "PersonInChargeInitials");
            DropColumn("dbo.Contracts", "DirectorInitials");
            DropColumn("dbo.Companies", "PersonInChargeInitials");
            DropColumn("dbo.Companies", "DirectorInitials");
        }
    }
}
