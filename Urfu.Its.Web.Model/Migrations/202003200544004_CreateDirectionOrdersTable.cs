namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDirectionOrdersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DirectionOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DirectionId = c.String(maxLength: 128),
                        Number = c.String(),
                        Date = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.DirectionId)
                .Index(t => t.DirectionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DirectionOrders", "DirectionId", "dbo.Directions");
            DropIndex("dbo.DirectionOrders", new[] { "DirectionId" });
            DropTable("dbo.DirectionOrders");
        }
    }
}
