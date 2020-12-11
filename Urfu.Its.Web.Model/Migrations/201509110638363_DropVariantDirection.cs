namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropVariantDirection : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Variants", "directionId", "dbo.Directions");
            DropForeignKey("dbo.EduPrograms", "directionId", "dbo.Directions");
            DropIndex("dbo.Variants", "IX_Variant_directionId");
            AddForeignKey("dbo.EduPrograms", "directionId", "dbo.Directions", "uid");
            DropColumn("dbo.Variants", "directionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Variants", "directionId", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.EduPrograms", "directionId", "dbo.Directions");
            CreateIndex("dbo.Variants", "directionId", name: "IX_Variant_directionId");
            AddForeignKey("dbo.EduPrograms", "directionId", "dbo.Directions", "uid", cascadeDelete: true);
            AddForeignKey("dbo.Variants", "directionId", "dbo.Directions", "uid");
        }
    }
}
