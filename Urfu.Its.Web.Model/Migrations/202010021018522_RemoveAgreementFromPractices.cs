namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAgreementFromPractices : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Practices", "Agreement");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Practices", "Agreement", c => c.Boolean(nullable: false));
        }
    }
}
