namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSerialNumberInContractTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "SerialNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "SerialNumber");
        }
    }
}
