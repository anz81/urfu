namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersionToVersionedDocumentTemplateAndChangePK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropPrimaryKey("dbo.VersionedDocumentTemplates");
            AddColumn("dbo.VersionedDocumentTemplates", "TemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.VersionedDocumentTemplates", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.VersionedDocumentTemplates", "PreviousTemplateId", c => c.Int());
            AddPrimaryKey("dbo.VersionedDocumentTemplates", "TemplateId");
            CreateIndex("dbo.VersionedDocumentTemplates", "PreviousTemplateId");
            AddForeignKey("dbo.VersionedDocumentTemplates", "PreviousTemplateId", "dbo.VersionedDocumentTemplates", "TemplateId");
            AddForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates", "TemplateId", cascadeDelete: true);
            DropColumn("dbo.VersionedDocumentTemplates", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VersionedDocumentTemplates", "Id", c => c.Int(nullable: false));
            DropForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropForeignKey("dbo.VersionedDocumentTemplates", "PreviousTemplateId", "dbo.VersionedDocumentTemplates");
            DropIndex("dbo.VersionedDocumentTemplates", new[] { "PreviousTemplateId" });
            DropPrimaryKey("dbo.VersionedDocumentTemplates");
            DropColumn("dbo.VersionedDocumentTemplates", "PreviousTemplateId");
            DropColumn("dbo.VersionedDocumentTemplates", "Version");
            DropColumn("dbo.VersionedDocumentTemplates", "TemplateId");
            AddPrimaryKey("dbo.VersionedDocumentTemplates", "Id");
            AddForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates", "Id", cascadeDelete: true);
        }
    }
}
