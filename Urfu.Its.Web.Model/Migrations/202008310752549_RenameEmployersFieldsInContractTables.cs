namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameEmployersFieldsInContractTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "PartnerSiteId", c => c.String());
            AddColumn("dbo.Companies", "PartnerSiteId", c => c.String());
            AddColumn("dbo.ContractLimits", "PartnerSiteId", c => c.Int());
            DropColumn("dbo.Contracts", "EmployersId");
            DropColumn("dbo.Companies", "EmployersId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "EmployersId", c => c.String());
            AddColumn("dbo.Contracts", "EmployersId", c => c.String());
            DropColumn("dbo.ContractLimits", "PartnerSiteId");
            DropColumn("dbo.Companies", "PartnerSiteId");
            DropColumn("dbo.Contracts", "PartnerSiteId");
        }
    }
}
