namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateProfOrderChangeAndConnectionTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProfOrders", "BaseOrderId", "dbo.ProfOrders");
            DropIndex("dbo.ProfOrders", new[] { "BaseOrderId" });
            CreateTable(
                "dbo.ProfOrderChanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfStandardCode = c.String(maxLength: 20),
                        NumberOfMintrud = c.String(),
                        DateOfMintrud = c.DateTime(),
                        RegNumberOfMinust = c.String(),
                        RegNumberDateOfMinust = c.DateTime(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProfStandards", t => t.ProfStandardCode)
                .Index(t => t.ProfStandardCode);
            
            CreateTable(
                "dbo.ProfOrderConnections",
                c => new
                    {
                        ProfOrderId = c.Int(nullable: false),
                        ProfOrderChangeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProfOrderId, t.ProfOrderChangeId })
                .ForeignKey("dbo.ProfOrders", t => t.ProfOrderId, cascadeDelete: true)
                .ForeignKey("dbo.ProfOrderChanges", t => t.ProfOrderChangeId, cascadeDelete: true)
                .Index(t => t.ProfOrderId)
                .Index(t => t.ProfOrderChangeId);
            
            DropColumn("dbo.ProfOrders", "BaseOrderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProfOrders", "BaseOrderId", c => c.Int());
            DropForeignKey("dbo.ProfOrderChanges", "ProfStandardCode", "dbo.ProfStandards");
            DropForeignKey("dbo.ProfOrderConnections", "ProfOrderChangeId", "dbo.ProfOrderChanges");
            DropForeignKey("dbo.ProfOrderConnections", "ProfOrderId", "dbo.ProfOrders");
            DropIndex("dbo.ProfOrderConnections", new[] { "ProfOrderChangeId" });
            DropIndex("dbo.ProfOrderConnections", new[] { "ProfOrderId" });
            DropIndex("dbo.ProfOrderChanges", new[] { "ProfStandardCode" });
            DropTable("dbo.ProfOrderConnections");
            DropTable("dbo.ProfOrderChanges");
            CreateIndex("dbo.ProfOrders", "BaseOrderId");
            AddForeignKey("dbo.ProfOrders", "BaseOrderId", "dbo.ProfOrders", "Id");
        }
    }
}
