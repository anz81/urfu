namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Contract_AddContractDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "ContractDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "ContractDate");
        }
    }
}
