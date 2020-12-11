namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeTables_Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        INN = c.String(maxLength: 20),
                        Director = c.String(maxLength: 255),
                        PersonInCharge = c.String(maxLength: 255),
                        Adddress = c.String(maxLength: 255),
                        PhoneNumber = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ContractLimits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContractId = c.Int(nullable: false),
                        QualificationName = c.String(maxLength: 127),
                        Course = c.Int(nullable: false),
                        Limit = c.Int(nullable: false),
                        DirectionId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.DirectionId)
                .ForeignKey("dbo.Qualifications", t => t.QualificationName)
                .ForeignKey("dbo.Contracts", t => t.ContractId, cascadeDelete: true)
                .Index(t => t.ContractId)
                .Index(t => t.QualificationName)
                .Index(t => t.DirectionId);
            
            CreateTable(
                "dbo.ContractPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContractId = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId, cascadeDelete: true)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.ContractId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(nullable: false),
                        Director = c.String(maxLength: 255),
                        PersonInCharge = c.String(maxLength: 255),
                        IsShortDated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ContractStudents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.String(maxLength: 128),
                        ContractId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId)
                .Index(t => t.StudentId)
                .Index(t => t.ContractId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractStudents", "StudentId", "dbo.Students");
            DropForeignKey("dbo.ContractStudents", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.ContractPeriods", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.ContractPeriods", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.ContractLimits", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.Contracts", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ContractLimits", "QualificationName", "dbo.Qualifications");
            DropForeignKey("dbo.ContractLimits", "DirectionId", "dbo.Directions");
            DropIndex("dbo.ContractStudents", new[] { "ContractId" });
            DropIndex("dbo.ContractStudents", new[] { "StudentId" });
            DropIndex("dbo.Contracts", new[] { "CompanyId" });
            DropIndex("dbo.ContractPeriods", new[] { "SemesterId" });
            DropIndex("dbo.ContractPeriods", new[] { "ContractId" });
            DropIndex("dbo.ContractLimits", new[] { "DirectionId" });
            DropIndex("dbo.ContractLimits", new[] { "QualificationName" });
            DropIndex("dbo.ContractLimits", new[] { "ContractId" });
            DropTable("dbo.ContractStudents");
            DropTable("dbo.Contracts");
            DropTable("dbo.ContractPeriods");
            DropTable("dbo.ContractLimits");
            DropTable("dbo.Companies");
        }
    }
}
