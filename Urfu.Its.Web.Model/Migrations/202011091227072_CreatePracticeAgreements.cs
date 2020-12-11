namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePracticeAgreements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeAgreements",
                c => new
                    {
                        FileStorageId = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileStorageId)
                .ForeignKey("dbo.FileStorage", t => t.FileStorageId)
                .Index(t => t.FileStorageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeAgreements", "FileStorageId", "dbo.FileStorage");
            DropIndex("dbo.PracticeAgreements", new[] { "FileStorageId" });
            DropTable("dbo.PracticeAgreements");
        }
    }
}
