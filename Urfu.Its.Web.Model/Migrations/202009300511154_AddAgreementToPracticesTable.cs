namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgreementToPracticesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Practices", "Agreement", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Practices", "Agreement");
        }
    }
}
