namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsToContractPeriodTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContractPeriods", "DivisionDescription", c => c.String());
            AddColumn("dbo.ContractPeriods", "AdditionalTerms", c => c.String());
            AddColumn("dbo.ContractPeriods", "FileStorageId", c => c.Int());
            CreateIndex("dbo.ContractPeriods", "FileStorageId");
            AddForeignKey("dbo.ContractPeriods", "FileStorageId", "dbo.FileStorage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractPeriods", "FileStorageId", "dbo.FileStorage");
            DropIndex("dbo.ContractPeriods", new[] { "FileStorageId" });
            DropColumn("dbo.ContractPeriods", "FileStorageId");
            DropColumn("dbo.ContractPeriods", "AdditionalTerms");
            DropColumn("dbo.ContractPeriods", "DivisionDescription");
        }
    }
}
