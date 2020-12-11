namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCompanyDocument : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Document", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "Document");
        }
    }
}
