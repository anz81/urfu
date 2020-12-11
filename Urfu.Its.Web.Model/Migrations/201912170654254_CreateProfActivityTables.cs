namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateProfActivityTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProfActivityArea",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 10),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.ProfActivityKind",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 10),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.ProfOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfStandardCode = c.String(maxLength: 20),
                        NumberOfMintrud = c.String(),
                        DateOfMintrud = c.DateTime(),
                        RegNumberOfMinust = c.String(),
                        RegNumberDateOfMinust = c.DateTime(),
                        Status = c.String(),
                        BaseOrderId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProfOrders", t => t.BaseOrderId)
                .ForeignKey("dbo.ProfStandards", t => t.ProfStandardCode)
                .Index(t => t.ProfStandardCode)
                .Index(t => t.BaseOrderId);
            
            CreateTable(
                "dbo.ProfStandards",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 20),
                        Title = c.String(),
                        ProfActivityAreaCode = c.String(maxLength: 10),
                        ProfActivityKindCode = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Code)
                .ForeignKey("dbo.ProfActivityArea", t => t.ProfActivityAreaCode)
                .ForeignKey("dbo.ProfActivityKind", t => t.ProfActivityKindCode)
                .Index(t => t.ProfActivityAreaCode)
                .Index(t => t.ProfActivityKindCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProfOrders", "ProfStandardCode", "dbo.ProfStandards");
            DropForeignKey("dbo.ProfStandards", "ProfActivityKindCode", "dbo.ProfActivityKind");
            DropForeignKey("dbo.ProfStandards", "ProfActivityAreaCode", "dbo.ProfActivityArea");
            DropForeignKey("dbo.ProfOrders", "BaseOrderId", "dbo.ProfOrders");
            DropIndex("dbo.ProfStandards", new[] { "ProfActivityKindCode" });
            DropIndex("dbo.ProfStandards", new[] { "ProfActivityAreaCode" });
            DropIndex("dbo.ProfOrders", new[] { "BaseOrderId" });
            DropIndex("dbo.ProfOrders", new[] { "ProfStandardCode" });
            DropTable("dbo.ProfStandards");
            DropTable("dbo.ProfOrders");
            DropTable("dbo.ProfActivityKind");
            DropTable("dbo.ProfActivityArea");
        }
    }
}
