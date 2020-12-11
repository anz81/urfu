namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersionedDocuments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModuleWorkingProgram",
                c => new
                    {
                        VersionedDocumentId = c.Int(nullable: false),
                        ModuleId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.VersionedDocumentId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.VersionedDocuments", t => t.VersionedDocumentId)
                .Index(t => t.VersionedDocumentId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.VersionedDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VersionedDocumentTemplates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);
            
            CreateTable(
                "dbo.VersionedDocumentBlockLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        DocumentBlockId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VersionedDocuments", t => t.DocumentId, cascadeDelete: true)
                .ForeignKey("dbo.VersionedDocumentBlocks", t => t.DocumentBlockId, cascadeDelete: true)
                .Index(t => t.DocumentId)
                .Index(t => t.DocumentBlockId);
            
            CreateTable(
                "dbo.VersionedDocumentBlocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.String(nullable: false),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VersionedDocumentBlockTemplates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);
            
            CreateTable(
                "dbo.VersionedDocumentBlockTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Schema = c.String(nullable: false),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VersionedDocumentTemplates", t => t.TemplateId)
                .Index(t => t.TemplateId);
            
            CreateTable(
                "dbo.VersionedDocumentTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.Binary(nullable: false),
                        DocumentType = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleWorkingProgram", "VersionedDocumentId", "dbo.VersionedDocuments");
            DropForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropForeignKey("dbo.VersionedDocumentBlockTemplates", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropForeignKey("dbo.VersionedDocumentBlocks", "TemplateId", "dbo.VersionedDocumentBlockTemplates");
            DropForeignKey("dbo.VersionedDocumentBlockLinks", "DocumentBlockId", "dbo.VersionedDocumentBlocks");
            DropForeignKey("dbo.VersionedDocumentBlockLinks", "DocumentId", "dbo.VersionedDocuments");
            DropForeignKey("dbo.ModuleWorkingProgram", "ModuleId", "dbo.Modules");
            DropIndex("dbo.VersionedDocumentBlockTemplates", new[] { "TemplateId" });
            DropIndex("dbo.VersionedDocumentBlocks", new[] { "TemplateId" });
            DropIndex("dbo.VersionedDocumentBlockLinks", new[] { "DocumentBlockId" });
            DropIndex("dbo.VersionedDocumentBlockLinks", new[] { "DocumentId" });
            DropIndex("dbo.VersionedDocuments", new[] { "TemplateId" });
            DropIndex("dbo.ModuleWorkingProgram", new[] { "ModuleId" });
            DropIndex("dbo.ModuleWorkingProgram", new[] { "VersionedDocumentId" });
            DropTable("dbo.VersionedDocumentTemplates");
            DropTable("dbo.VersionedDocumentBlockTemplates");
            DropTable("dbo.VersionedDocumentBlocks");
            DropTable("dbo.VersionedDocumentBlockLinks");
            DropTable("dbo.VersionedDocuments");
            DropTable("dbo.ModuleWorkingProgram");
        }
    }
}
