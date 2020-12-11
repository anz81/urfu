namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequisitesOrdersFGOS_CreateTable1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequisitesOrdersFGOS",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DirectionId = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Order = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.DirectionId)
                .Index(t => t.DirectionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequisitesOrdersFGOS", "DirectionId", "dbo.Directions");
            DropIndex("dbo.RequisitesOrdersFGOS", new[] { "DirectionId" });
            DropTable("dbo.RequisitesOrdersFGOS");
        }
    }
}
