namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RevertNameOfVersionedDocumentTemplatePK1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropForeignKey("dbo.VersionedDocumentTemplates", "PreviousTemplateId", "dbo.VersionedDocumentTemplates");
            DropPrimaryKey("dbo.VersionedDocumentTemplates");
            AddColumn("dbo.VersionedDocumentTemplates", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.VersionedDocumentTemplates", "Id");
            AddForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates", "Id", cascadeDelete: true);
            AddForeignKey("dbo.VersionedDocumentTemplates", "PreviousTemplateId", "dbo.VersionedDocumentTemplates", "Id");
            DropColumn("dbo.VersionedDocumentTemplates", "TemplateId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VersionedDocumentTemplates", "TemplateId", c => c.Int(nullable: false));
            DropForeignKey("dbo.VersionedDocumentTemplates", "PreviousTemplateId", "dbo.VersionedDocumentTemplates");
            DropForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropPrimaryKey("dbo.VersionedDocumentTemplates");
            DropColumn("dbo.VersionedDocumentTemplates", "Id");
            AddPrimaryKey("dbo.VersionedDocumentTemplates", "TemplateId");
            AddForeignKey("dbo.VersionedDocumentTemplates", "PreviousTemplateId", "dbo.VersionedDocumentTemplates", "TemplateId");
            AddForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates", "TemplateId", cascadeDelete: true);
        }
    }
}
