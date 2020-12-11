namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateIndexForVariantContent : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.VariantContents", new[] { "moduleId" });
            CreateIndex("dbo.VariantContents", new[] { "moduleId", "Selected" }, name: "IX_VariantContent_auto1");
        }
        
        public override void Down()
        {
            DropIndex("dbo.VariantContents", "IX_VariantContent_auto1");
            CreateIndex("dbo.VariantContents", "moduleId");
        }
    }
}
