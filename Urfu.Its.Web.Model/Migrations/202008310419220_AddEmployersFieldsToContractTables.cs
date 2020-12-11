namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployersFieldsToContractTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "EmployersId", c => c.String());
            AddColumn("dbo.Companies", "EmployersId", c => c.String());
            AddColumn("dbo.ContractPeriods", "RequestId", c => c.Int());
            AddColumn("dbo.ContractPeriods", "RequestNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContractPeriods", "RequestNumber");
            DropColumn("dbo.ContractPeriods", "RequestId");
            DropColumn("dbo.Companies", "EmployersId");
            DropColumn("dbo.Contracts", "EmployersId");
        }
    }
}
