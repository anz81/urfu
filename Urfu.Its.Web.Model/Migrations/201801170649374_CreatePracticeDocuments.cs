namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePracticeDocuments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PracticeId = c.Int(nullable: false),
                        DocumentType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        FileName = c.String(),
                        FileData = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Practices", t => t.PracticeId, cascadeDelete: true)
                .Index(t => t.PracticeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeDocuments", "PracticeId", "dbo.Practices");
            DropIndex("dbo.PracticeDocuments", new[] { "PracticeId" });
            DropTable("dbo.PracticeDocuments");
        }
    }
}
