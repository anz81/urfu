namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsConfirmedInCompanies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "IsConfirmed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "IsConfirmed");
        }
    }
}
