namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOldLimits : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Limits", "directionId", "dbo.Directions");
            DropForeignKey("dbo.VariantLimits", "LimitId", "dbo.Limits");
            DropForeignKey("dbo.Limits", "ModuleId", "dbo.Modules");
            DropIndex("dbo.Limits", "IX_Limit_Unique");
            DropIndex("dbo.VariantLimits", new[] { "LimitId" });
            DropTable("dbo.Limits");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Limits",
                c => new
                    {
                        LimitId = c.Int(nullable: false, identity: true),
                        directionId = c.String(nullable: false, maxLength: 128),
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        Year = c.Int(nullable: false),
                        StudentsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LimitId);
            
            CreateIndex("dbo.VariantLimits", "LimitId");
            CreateIndex("dbo.Limits", new[] { "directionId", "ModuleId", "Year" }, unique: true, name: "IX_Limit_Unique");
            AddForeignKey("dbo.Limits", "ModuleId", "dbo.Modules", "uuid", cascadeDelete: true);
            AddForeignKey("dbo.VariantLimits", "LimitId", "dbo.Limits", "LimitId", cascadeDelete: true);
            AddForeignKey("dbo.Limits", "directionId", "dbo.Directions", "uid", cascadeDelete: true);
        }
    }
}
