namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScanNameToContracts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "ScanName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "ScanName");
        }
    }
}
