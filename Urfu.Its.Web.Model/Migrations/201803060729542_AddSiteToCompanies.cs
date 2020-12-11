namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteToCompanies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Site", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "Site");
        }
    }
}
