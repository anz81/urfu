namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContractAddColumnPhoneEMail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "PhoneNumber", c => c.String(maxLength: 255));
            AddColumn("dbo.Contracts", "Email", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "Email");
            DropColumn("dbo.Contracts", "PhoneNumber");
        }
    }
}
