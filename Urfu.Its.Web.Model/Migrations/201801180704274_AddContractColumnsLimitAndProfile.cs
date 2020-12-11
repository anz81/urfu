namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContractColumnsLimitAndProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "Limit", c => c.Int());
            AddColumn("dbo.ContractLimits", "ProfileId", c => c.String(maxLength: 127));
            CreateIndex("dbo.ContractLimits", "ProfileId");
            AddForeignKey("dbo.ContractLimits", "ProfileId", "dbo.Profiles", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractLimits", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.ContractLimits", new[] { "ProfileId" });
            DropColumn("dbo.ContractLimits", "ProfileId");
            DropColumn("dbo.Contracts", "Limit");
        }
    }
}
