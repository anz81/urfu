namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateAreaEducationOrdersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AreaEducationOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AreaEducationId = c.Int(nullable: false),
                        Number = c.String(),
                        Date = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AreaEducation", t => t.AreaEducationId, cascadeDelete: true)
                .Index(t => t.AreaEducationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AreaEducationOrders", "AreaEducationId", "dbo.AreaEducation");
            DropIndex("dbo.AreaEducationOrders", new[] { "AreaEducationId" });
            DropTable("dbo.AreaEducationOrders");
        }
    }
}
