namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveVersionedDocumentBlockTemplates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VersionedDocumentBlocks", "TemplateId", "dbo.VersionedDocumentBlockTemplates");
            DropForeignKey("dbo.VersionedDocumentBlockTemplates", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropIndex("dbo.VersionedDocumentBlocks", new[] { "TemplateId" });
            DropIndex("dbo.VersionedDocumentBlockTemplates", new[] { "TemplateId" });
            AddColumn("dbo.ModuleWorkingProgram", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.ModuleWorkingProgram", "StatusChangeTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.VersionedDocumentBlocks", "SchemaSections", c => c.String(nullable: false));
            AddColumn("dbo.VersionedDocumentBlocks", "DocumentTemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.VersionedDocumentTemplates", "Schema", c => c.String(nullable: false));
            CreateIndex("dbo.VersionedDocumentBlocks", "DocumentTemplateId");
            AddForeignKey("dbo.VersionedDocumentBlocks", "DocumentTemplateId", "dbo.VersionedDocumentTemplates", "Id");
            DropColumn("dbo.VersionedDocumentBlocks", "TemplateId");
            DropTable("dbo.VersionedDocumentBlockTemplates");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.VersionedDocumentBlockTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Schema = c.String(nullable: false),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.VersionedDocumentBlocks", "TemplateId", c => c.Int(nullable: false));
            DropForeignKey("dbo.VersionedDocumentBlocks", "DocumentTemplateId", "dbo.VersionedDocumentTemplates");
            DropIndex("dbo.VersionedDocumentBlocks", new[] { "DocumentTemplateId" });
            DropColumn("dbo.VersionedDocumentTemplates", "Schema");
            DropColumn("dbo.VersionedDocumentBlocks", "DocumentTemplateId");
            DropColumn("dbo.VersionedDocumentBlocks", "SchemaSections");
            DropColumn("dbo.ModuleWorkingProgram", "StatusChangeTime");
            DropColumn("dbo.ModuleWorkingProgram", "Status");
            CreateIndex("dbo.VersionedDocumentBlockTemplates", "TemplateId");
            CreateIndex("dbo.VersionedDocumentBlocks", "TemplateId");
            AddForeignKey("dbo.VersionedDocumentBlockTemplates", "TemplateId", "dbo.VersionedDocumentTemplates", "Id");
            AddForeignKey("dbo.VersionedDocumentBlocks", "TemplateId", "dbo.VersionedDocumentBlockTemplates", "Id", cascadeDelete: true);
        }
    }
}
