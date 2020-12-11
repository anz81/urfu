namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateExistContractFieldInPractices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Practices", "ExistContract", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Practices", "ExistContract");
        }
    }
}
