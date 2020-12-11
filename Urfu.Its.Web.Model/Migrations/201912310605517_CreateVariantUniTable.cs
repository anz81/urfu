namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateVariantUniTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VariantsUnis",
                c => new
                    {
                        TrajectoryUuid = c.String(nullable: false, maxLength: 128),
                        VariantId = c.Int(),
                        ProfileId = c.String(maxLength: 127),
                        DocumentName = c.String(),
                    })
                .PrimaryKey(t => t.TrajectoryUuid)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .ForeignKey("dbo.Variants", t => t.VariantId)
                .Index(t => t.VariantId)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VariantsUnis", "VariantId", "dbo.Variants");
            DropForeignKey("dbo.VariantsUnis", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.VariantsUnis", new[] { "ProfileId" });
            DropIndex("dbo.VariantsUnis", new[] { "VariantId" });
            DropTable("dbo.VariantsUnis");
        }
    }
}
