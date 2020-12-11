namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RebuildVariantAdmission : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.StudentSelectionPriorities", "variantId");
            AddForeignKey("dbo.StudentSelectionPriorities", "variantId", "dbo.Variants", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentSelectionPriorities", "variantId", "dbo.Variants");
            DropIndex("dbo.StudentSelectionPriorities", new[] { "variantId" });
        }
    }
}
