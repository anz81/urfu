namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLimitsFromVariant : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.VariantContents", "Limits");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VariantContents", "Limits", c => c.Int());
        }
    }
}
