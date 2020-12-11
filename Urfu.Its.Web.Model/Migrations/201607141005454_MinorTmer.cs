namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorTmer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinorDisciplines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinorId = c.String(maxLength: 128),
                        DisciplineUid = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineUid)
                .ForeignKey("dbo.Minors", t => t.MinorId)
                .Index(t => t.MinorId)
                .Index(t => t.DisciplineUid);
            
            CreateTable(
                "dbo.MinorDisciplineTmers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinorDisciplineId = c.Int(nullable: false),
                        TmerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MinorDisciplines", t => t.MinorDisciplineId, cascadeDelete: true)
                .ForeignKey("dbo.Tmer", t => t.TmerId)
                .Index(t => t.MinorDisciplineId)
                .Index(t => t.TmerId);
            
            CreateTable(
                "dbo.MinorDisciplineTmerPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinorDisciplineTmerId = c.Int(nullable: false),
                        MinorPeriodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MinorPeriods", t => t.MinorPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.MinorDisciplineTmers", t => t.MinorDisciplineTmerId, cascadeDelete: true)
                .Index(t => t.MinorDisciplineTmerId)
                .Index(t => t.MinorPeriodId);
            
            CreateTable(
                "dbo.MinorDisciplineTmerPeriodDivision",
                c => new
                    {
                        DisciplineTmerPeriodId = c.Int(nullable: false),
                        DivisionId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.DisciplineTmerPeriodId, t.DivisionId })
                .ForeignKey("dbo.MinorDisciplineTmerPeriods", t => t.DisciplineTmerPeriodId, cascadeDelete: true)
                .ForeignKey("dbo.Divisions", t => t.DivisionId, cascadeDelete: true)
                .Index(t => t.DisciplineTmerPeriodId)
                .Index(t => t.DivisionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinorDisciplineTmers", "TmerId", "dbo.Tmer");
            DropForeignKey("dbo.MinorDisciplineTmerPeriods", "MinorDisciplineTmerId", "dbo.MinorDisciplineTmers");
            DropForeignKey("dbo.MinorDisciplineTmerPeriods", "MinorPeriodId", "dbo.MinorPeriods");
            DropForeignKey("dbo.MinorDisciplineTmerPeriodDivision", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.MinorDisciplineTmerPeriodDivision", "DisciplineTmerPeriodId", "dbo.MinorDisciplineTmerPeriods");
            DropForeignKey("dbo.MinorDisciplineTmers", "MinorDisciplineId", "dbo.MinorDisciplines");
            DropForeignKey("dbo.MinorDisciplines", "MinorId", "dbo.Minors");
            DropForeignKey("dbo.MinorDisciplines", "DisciplineUid", "dbo.Disciplines");
            DropIndex("dbo.MinorDisciplineTmerPeriodDivision", new[] { "DivisionId" });
            DropIndex("dbo.MinorDisciplineTmerPeriodDivision", new[] { "DisciplineTmerPeriodId" });
            DropIndex("dbo.MinorDisciplineTmerPeriods", new[] { "MinorPeriodId" });
            DropIndex("dbo.MinorDisciplineTmerPeriods", new[] { "MinorDisciplineTmerId" });
            DropIndex("dbo.MinorDisciplineTmers", new[] { "TmerId" });
            DropIndex("dbo.MinorDisciplineTmers", new[] { "MinorDisciplineId" });
            DropIndex("dbo.MinorDisciplines", new[] { "DisciplineUid" });
            DropIndex("dbo.MinorDisciplines", new[] { "MinorId" });
            DropTable("dbo.MinorDisciplineTmerPeriodDivision");
            DropTable("dbo.MinorDisciplineTmerPeriods");
            DropTable("dbo.MinorDisciplineTmers");
            DropTable("dbo.MinorDisciplines");
        }
    }
}
