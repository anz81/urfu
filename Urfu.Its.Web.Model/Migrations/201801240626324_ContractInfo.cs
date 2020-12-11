namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContractInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "IsEndless", c => c.Boolean(nullable: false));
            AddColumn("dbo.Contracts", "PersonalComment", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "PersonalComment");
            DropColumn("dbo.Contracts", "IsEndless");
        }
    }
}
