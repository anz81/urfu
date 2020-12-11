namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseVariantForProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "VariantId", c => c.Int());
            CreateIndex("dbo.EduPrograms", "VariantId");
            AddForeignKey("dbo.EduPrograms", "VariantId", "dbo.Variants", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduPrograms", "VariantId", "dbo.Variants");
            DropIndex("dbo.EduPrograms", new[] { "VariantId" });
            DropColumn("dbo.EduPrograms", "VariantId");
        }
    }
}
