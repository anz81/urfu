namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeVersionedDocumentTemplatePK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropPrimaryKey("dbo.VersionedDocumentTemplates");
            AddColumn("dbo.VersionedDocumentTemplates", "TemplateId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.VersionedDocumentTemplates", "TemplateId");
            AddForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates", "TemplateId", cascadeDelete: true);
            DropColumn("dbo.VersionedDocumentTemplates", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VersionedDocumentTemplates", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates");
            DropPrimaryKey("dbo.VersionedDocumentTemplates");
            DropColumn("dbo.VersionedDocumentTemplates", "TemplateId");
            AddPrimaryKey("dbo.VersionedDocumentTemplates", "Id");
            AddForeignKey("dbo.VersionedDocuments", "TemplateId", "dbo.VersionedDocumentTemplates", "Id", cascadeDelete: true);
        }
    }
}
