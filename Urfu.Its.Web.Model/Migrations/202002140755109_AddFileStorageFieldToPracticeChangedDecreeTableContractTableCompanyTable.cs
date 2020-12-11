namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileStorageFieldToPracticeChangedDecreeTableContractTableCompanyTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "FileStorageId", c => c.Int());
            AddColumn("dbo.Companies", "FileStorageId", c => c.Int());
            AddColumn("dbo.PracticeChangedDecrees", "FileStorageId", c => c.Int());
            CreateIndex("dbo.Contracts", "FileStorageId");
            CreateIndex("dbo.Companies", "FileStorageId");
            CreateIndex("dbo.PracticeChangedDecrees", "FileStorageId");
            AddForeignKey("dbo.Companies", "FileStorageId", "dbo.FileStorage", "Id");
            AddForeignKey("dbo.Contracts", "FileStorageId", "dbo.FileStorage", "Id");
            AddForeignKey("dbo.PracticeChangedDecrees", "FileStorageId", "dbo.FileStorage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeChangedDecrees", "FileStorageId", "dbo.FileStorage");
            DropForeignKey("dbo.Contracts", "FileStorageId", "dbo.FileStorage");
            DropForeignKey("dbo.Companies", "FileStorageId", "dbo.FileStorage");
            DropIndex("dbo.PracticeChangedDecrees", new[] { "FileStorageId" });
            DropIndex("dbo.Companies", new[] { "FileStorageId" });
            DropIndex("dbo.Contracts", new[] { "FileStorageId" });
            DropColumn("dbo.PracticeChangedDecrees", "FileStorageId");
            DropColumn("dbo.Companies", "FileStorageId");
            DropColumn("dbo.Contracts", "FileStorageId");
        }
    }
}
