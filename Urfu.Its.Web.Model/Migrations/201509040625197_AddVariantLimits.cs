namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVariantLimits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VariantLimits",
                c => new
                    {
                        LimitId = c.Int(nullable: false),
                        VariantId = c.Int(nullable: false),
                        StudentsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LimitId, t.VariantId })
                .ForeignKey("dbo.Limits", t => t.LimitId, cascadeDelete: true)
                .ForeignKey("dbo.Variants", t => t.VariantId, cascadeDelete: true)
                .Index(t => t.LimitId)
                .Index(t => t.VariantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VariantLimits", "VariantId", "dbo.Variants");
            DropForeignKey("dbo.VariantLimits", "LimitId", "dbo.Limits");
            DropIndex("dbo.VariantLimits", new[] { "VariantId" });
            DropIndex("dbo.VariantLimits", new[] { "LimitId" });
            DropTable("dbo.VariantLimits");
        }
    }
}
