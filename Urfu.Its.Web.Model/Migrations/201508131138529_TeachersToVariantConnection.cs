namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TeachersToVariantConnection : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.PlanTeachers", "variantId", "dbo.Variants", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlanTeachers", "variantId", "dbo.Variants");
        }
    }
}
