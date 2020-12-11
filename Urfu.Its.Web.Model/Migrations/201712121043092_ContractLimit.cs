namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContractLimit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContractLimits", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.ContractStudents", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.ContractStudents", "StudentId", "dbo.Students");
            DropIndex("dbo.ContractLimits", new[] { "ContractId" });
            DropIndex("dbo.ContractStudents", new[] { "StudentId" });
            DropIndex("dbo.ContractStudents", new[] { "ContractId" });
            AddColumn("dbo.ContractLimits", "ContractPeriodId", c => c.Int(nullable: false));
            CreateIndex("dbo.ContractLimits", "ContractPeriodId");
            AddForeignKey("dbo.ContractLimits", "ContractPeriodId", "dbo.ContractPeriods", "Id", cascadeDelete: true);
            DropColumn("dbo.ContractLimits", "ContractId");
            DropTable("dbo.ContractStudents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ContractStudents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.String(maxLength: 128),
                        ContractId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ContractLimits", "ContractId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ContractLimits", "ContractPeriodId", "dbo.ContractPeriods");
            DropIndex("dbo.ContractLimits", new[] { "ContractPeriodId" });
            DropColumn("dbo.ContractLimits", "ContractPeriodId");
            CreateIndex("dbo.ContractStudents", "ContractId");
            CreateIndex("dbo.ContractStudents", "StudentId");
            CreateIndex("dbo.ContractLimits", "ContractId");
            AddForeignKey("dbo.ContractStudents", "StudentId", "dbo.Students", "Id");
            AddForeignKey("dbo.ContractStudents", "ContractId", "dbo.Contracts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ContractLimits", "ContractId", "dbo.Contracts", "Id", cascadeDelete: true);
        }
    }
}
