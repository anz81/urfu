namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPartnerSiteIdFieldToCompanyLocationsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyLocations", "PartnerSiteId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompanyLocations", "PartnerSiteId");
        }
    }
}
