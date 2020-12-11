namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateYearInContractsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "Year", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "Year");
        }
    }
}
