namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteFieldsFileInfoFromCompaniesContractsPracticeDocumentsPracticeDecreesPracticeChangedDecrees : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Contracts", "Scan");
            DropColumn("dbo.Contracts", "ScanName");
            DropColumn("dbo.Companies", "Document");
            DropColumn("dbo.Companies", "DocumentName");
            DropColumn("dbo.PracticeDocuments", "FileName");
            DropColumn("dbo.PracticeDocuments", "FileData");
            DropColumn("dbo.PracticeDocuments", "FileDate");
            DropColumn("dbo.PracticeChangedDecrees", "FileName");
            DropColumn("dbo.PracticeChangedDecrees", "FileData");
            DropColumn("dbo.PracticeDecrees", "FileName");
            DropColumn("dbo.PracticeDecrees", "FileData");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PracticeDecrees", "FileData", c => c.Binary());
            AddColumn("dbo.PracticeDecrees", "FileName", c => c.String(maxLength: 250));
            AddColumn("dbo.PracticeChangedDecrees", "FileData", c => c.Binary());
            AddColumn("dbo.PracticeChangedDecrees", "FileName", c => c.String(maxLength: 250));
            AddColumn("dbo.PracticeDocuments", "FileDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PracticeDocuments", "FileData", c => c.Binary());
            AddColumn("dbo.PracticeDocuments", "FileName", c => c.String());
            AddColumn("dbo.Companies", "DocumentName", c => c.String(maxLength: 255));
            AddColumn("dbo.Companies", "Document", c => c.Binary());
            AddColumn("dbo.Contracts", "ScanName", c => c.String());
            AddColumn("dbo.Contracts", "Scan", c => c.Binary());
        }
    }
}
