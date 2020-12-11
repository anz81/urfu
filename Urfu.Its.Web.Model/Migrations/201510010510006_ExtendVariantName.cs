namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendVariantName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Variants", "Name", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Variants", "Name", c => c.String(nullable: false));
        }
    }
}
