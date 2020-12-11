namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSubgroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subgroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        EduProgramId = c.Int(),
                        ParentId = c.Int(),
                        VariantId = c.Int(),
                        ModuleId = c.String(maxLength: 128),
                        SubgroupType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.Subgroups", t => t.ParentId)
                .ForeignKey("dbo.EduPrograms", t => t.EduProgramId)
                .ForeignKey("dbo.Variants", t => t.VariantId)
                .Index(t => t.EduProgramId)
                .Index(t => t.ParentId)
                .Index(t => t.VariantId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.SubgroupMemberships",
                c => new
                    {
                        stidentId = c.String(nullable: false, maxLength: 128),
                        SubgroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.stidentId, t.SubgroupId })
                .ForeignKey("dbo.Students", t => t.stidentId, cascadeDelete: true)
                .ForeignKey("dbo.Subgroups", t => t.SubgroupId, cascadeDelete: true)
                .Index(t => t.stidentId)
                .Index(t => t.SubgroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subgroups", "VariantId", "dbo.Variants");
            DropForeignKey("dbo.SubgroupMemberships", "SubgroupId", "dbo.Subgroups");
            DropForeignKey("dbo.SubgroupMemberships", "stidentId", "dbo.Students");
            DropForeignKey("dbo.Subgroups", "EduProgramId", "dbo.EduPrograms");
            DropForeignKey("dbo.Subgroups", "ParentId", "dbo.Subgroups");
            DropForeignKey("dbo.Subgroups", "ModuleId", "dbo.Modules");
            DropIndex("dbo.SubgroupMemberships", new[] { "SubgroupId" });
            DropIndex("dbo.SubgroupMemberships", new[] { "stidentId" });
            DropIndex("dbo.Subgroups", new[] { "ModuleId" });
            DropIndex("dbo.Subgroups", new[] { "VariantId" });
            DropIndex("dbo.Subgroups", new[] { "ParentId" });
            DropIndex("dbo.Subgroups", new[] { "EduProgramId" });
            DropTable("dbo.SubgroupMemberships");
            DropTable("dbo.Subgroups");
        }
    }
}
