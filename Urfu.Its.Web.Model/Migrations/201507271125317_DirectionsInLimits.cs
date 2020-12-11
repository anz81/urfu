namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DirectionsInLimits : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Limits", "IX_Limit_Module");
            DropIndex("dbo.Limits", "IX_Limit_Unique");
            AddColumn("dbo.Limits", "directionId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Limits", new[] { "directionId", "ModuleId", "Year" }, unique: true, name: "IX_Limit_Unique");
            AddForeignKey("dbo.Limits", "directionId", "dbo.Directions", "uid", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Limits", "directionId", "dbo.Directions");
            DropIndex("dbo.Limits", "IX_Limit_Unique");
            DropColumn("dbo.Limits", "directionId");
            CreateIndex("dbo.Limits", new[] { "ModuleId", "Year" }, unique: true, name: "IX_Limit_Unique");
            CreateIndex("dbo.Limits", "ModuleId", name: "IX_Limit_Module");
        }
    }
}
