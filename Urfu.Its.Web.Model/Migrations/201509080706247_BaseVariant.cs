namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseVariant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Variants", "IsBase", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Variants", "IsBase");
        }
    }
}
