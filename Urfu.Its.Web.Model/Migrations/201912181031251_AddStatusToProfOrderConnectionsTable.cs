namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusToProfOrderConnectionsTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProfOrderConnections", "ProfOrderId", "dbo.ProfOrders");
            DropForeignKey("dbo.ProfOrderConnections", "ProfOrderChangeId", "dbo.ProfOrderChanges");
            DropTable("dbo.ProfOrderConnections");

            CreateTable(
                "dbo.ProfOrderConnections",
                c => new
                    {
                        ProfOrderId = c.Int(nullable: false),
                        ProfOrderChangeId = c.Int(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => new { t.ProfOrderId, t.ProfOrderChangeId })
                .ForeignKey("dbo.ProfOrders", t => t.ProfOrderId, cascadeDelete: true)
                .ForeignKey("dbo.ProfOrderChanges", t => t.ProfOrderChangeId, cascadeDelete: true);
            
            DropColumn("dbo.ProfOrderChanges", "Status");

        }
        
        public override void Down()
        {
            DropTable("dbo.ProfOrderConnections");
            CreateTable(
                "dbo.ProfOrderConnections",
                c => new
                    {
                        ProfOrderId = c.Int(nullable: false),
                        ProfOrderChangeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProfOrderId, t.ProfOrderChangeId });
            
            AddColumn("dbo.ProfOrderChanges", "Status", c => c.String());
            DropForeignKey("dbo.ProfOrderConnections", "ProfOrderChangeId", "dbo.ProfOrderChanges");
            DropForeignKey("dbo.ProfOrderConnections", "ProfOrderId", "dbo.ProfOrders");
            
            AddForeignKey("dbo.ProfOrderConnections", "ProfOrderChangeId", "dbo.ProfOrderChanges", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ProfOrderConnections", "ProfOrderId", "dbo.ProfOrders", "Id", cascadeDelete: true);
        }
    }
}
