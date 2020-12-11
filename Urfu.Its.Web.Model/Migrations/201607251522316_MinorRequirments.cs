namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorRequirments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinorRequirements",
                c => new
                    {
                        RequiredForId = c.String(nullable: false, maxLength: 128),
                        RequirementId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RequiredForId, t.RequirementId })
                .ForeignKey("dbo.Minors", t => t.RequiredForId, cascadeDelete: true)
                .ForeignKey("dbo.Modules", t => t.RequirementId, cascadeDelete: true)
                .Index(t => t.RequiredForId)
                .Index(t => t.RequirementId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinorRequirements", "RequirementId", "dbo.Modules");
            DropForeignKey("dbo.MinorRequirements", "RequiredForId", "dbo.Minors");
            DropIndex("dbo.MinorRequirements", new[] { "RequirementId" });
            DropIndex("dbo.MinorRequirements", new[] { "RequiredForId" });
            DropTable("dbo.MinorRequirements");
        }
    }
}
