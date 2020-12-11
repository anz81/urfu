namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePrograms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EduPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        directionId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.directionId, cascadeDelete: true)
                .Index(t => t.directionId);
            
            AddColumn("dbo.Variants", "EduProgramId", c => c.Int());
            CreateIndex("dbo.Variants", "EduProgramId");
            AddForeignKey("dbo.Variants", "EduProgramId", "dbo.EduPrograms", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Variants", "EduProgramId", "dbo.EduPrograms");
            DropForeignKey("dbo.EduPrograms", "directionId", "dbo.Directions");
            DropIndex("dbo.EduPrograms", new[] { "directionId" });
            DropIndex("dbo.Variants", new[] { "EduProgramId" });
            DropColumn("dbo.Variants", "EduProgramId");
            DropTable("dbo.EduPrograms");
        }
    }
}
