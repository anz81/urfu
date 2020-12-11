namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMinorTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Minors",
                c => new
                    {
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        MinorTechId = c.Int(nullable: false),
                        MinStudentsCount = c.Int(nullable: false),
                        MaxStudentsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.MinorTeches", t => t.MinorTechId, cascadeDelete: false)
                .Index(t => t.ModuleId)
                .Index(t => t.MinorTechId);
            
            CreateTable(
                "dbo.MinorPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleId = c.String(maxLength: 128),
                        Year = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Minors", t => t.ModuleId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: false)
                .Index(t => t.ModuleId)
                .Index(t => t.SemesterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Minors", "MinorTechId", "dbo.MinorTeches");
            DropForeignKey("dbo.MinorPeriods", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.MinorPeriods", "ModuleId", "dbo.Minors");
            DropForeignKey("dbo.Minors", "ModuleId", "dbo.Modules");
            DropIndex("dbo.MinorPeriods", new[] { "SemesterId" });
            DropIndex("dbo.MinorPeriods", new[] { "ModuleId" });
            DropIndex("dbo.Minors", new[] { "MinorTechId" });
            DropIndex("dbo.Minors", new[] { "ModuleId" });
            DropTable("dbo.MinorPeriods");
            DropTable("dbo.Minors");
        }
    }
}
