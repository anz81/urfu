namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChengedPracticeDecreeTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeChangedDecreeReasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PracticeChangedDecrees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MainDecreeId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        DecreeNumber = c.String(maxLength: 20),
                        DecreeDate = c.DateTime(),
                        SerialNumber = c.Int(nullable: false),
                        FileName = c.String(maxLength: 250),
                        FileData = c.Binary(),
                        SedId = c.Int(),
                        Comment = c.String(),
                        DateExportToSed = c.DateTime(),
                        ExecutorName = c.String(),
                        ExecutorPhone = c.String(),
                        ExecutorEmail = c.String(),
                        ROPInitials = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PracticeDecrees", t => t.MainDecreeId, cascadeDelete: true)
                .Index(t => t.MainDecreeId);
            
            CreateTable(
                "dbo.PracticeChangedDecreeStudents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.String(),
                        ChangedDecreeId = c.Int(nullable: false),
                        RecoveryDate = c.DateTime(),
                        ReasonId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PracticeChangedDecrees", t => t.ChangedDecreeId, cascadeDelete: true)
                .ForeignKey("dbo.PracticeChangedDecreeReasons", t => t.ReasonId)
                .Index(t => t.ChangedDecreeId)
                .Index(t => t.ReasonId);
            
            AddColumn("dbo.PracticeDecreeNumbers", "ChangedDecreeNumber", c => c.String());
            AddColumn("dbo.PracticeDecreeNumbers", "ChangedDecreeDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeChangedDecreeStudents", "ReasonId", "dbo.PracticeChangedDecreeReasons");
            DropForeignKey("dbo.PracticeChangedDecreeStudents", "ChangedDecreeId", "dbo.PracticeChangedDecrees");
            DropForeignKey("dbo.PracticeChangedDecrees", "MainDecreeId", "dbo.PracticeDecrees");
            DropIndex("dbo.PracticeChangedDecreeStudents", new[] { "ReasonId" });
            DropIndex("dbo.PracticeChangedDecreeStudents", new[] { "ChangedDecreeId" });
            DropIndex("dbo.PracticeChangedDecrees", new[] { "MainDecreeId" });
            DropColumn("dbo.PracticeDecreeNumbers", "ChangedDecreeDate");
            DropColumn("dbo.PracticeDecreeNumbers", "ChangedDecreeNumber");
            DropTable("dbo.PracticeChangedDecreeStudents");
            DropTable("dbo.PracticeChangedDecrees");
            DropTable("dbo.PracticeChangedDecreeReasons");
        }
    }
}
