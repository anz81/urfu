namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VariantContentsCascadeDelte : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VariantGroups", "VariantId", "dbo.Variants");
            AddForeignKey("dbo.VariantGroups", "VariantId", "dbo.Variants", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VariantGroups", "VariantId", "dbo.Variants");
            AddForeignKey("dbo.VariantGroups", "VariantId", "dbo.Variants", "Id");
        }
    }
}
