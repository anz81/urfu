namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeOfPartnerSiteIdFieldInCompanyLocationsTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompanyLocations", "PartnerSiteId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompanyLocations", "PartnerSiteId", c => c.String());
        }
    }
}
