namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEduResultsAndChangeCompetences : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EduResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10),
                        Description = c.String(nullable: false, maxLength: 1000),
                        DirectionId = c.String(maxLength: 128),
                        DivisionId = c.String(maxLength: 127),
                        IsDeleted = c.Boolean(nullable: false),
                        EduProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.DirectionId)
                .ForeignKey("dbo.Divisions", t => t.DivisionId)
                .ForeignKey("dbo.EduPrograms", t => t.EduProgramId, cascadeDelete: true)
                .Index(t => t.DirectionId)
                .Index(t => t.DivisionId)
                .Index(t => t.EduProgramId);
            
            AddColumn("dbo.Competences", "ExternalId", c => c.Int());
            AddColumn("dbo.Competences", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Competences", "Type", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Competences", "Standard", c => c.String(maxLength: 20));
            CreateIndex("dbo.Competences", "Type");
            CreateIndex("dbo.Competences", "Standard");
            AddForeignKey("dbo.Competences", "Standard", "dbo.Standards", "Name");
            AddForeignKey("dbo.Competences", "Type", "dbo.CompetenceTypes", "Name", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduResults", "EduProgramId", "dbo.EduPrograms");
            DropForeignKey("dbo.EduResults", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.EduResults", "DirectionId", "dbo.Directions");
            DropForeignKey("dbo.Competences", "Type", "dbo.CompetenceTypes");
            DropForeignKey("dbo.Competences", "Standard", "dbo.Standards");
            DropIndex("dbo.EduResults", new[] { "EduProgramId" });
            DropIndex("dbo.EduResults", new[] { "DivisionId" });
            DropIndex("dbo.EduResults", new[] { "DirectionId" });
            DropIndex("dbo.Competences", new[] { "Standard" });
            DropIndex("dbo.Competences", new[] { "Type" });
            AlterColumn("dbo.Competences", "Standard", c => c.String(maxLength: 200));
            AlterColumn("dbo.Competences", "Type", c => c.String(nullable: false));
            DropColumn("dbo.Competences", "IsDeleted");
            DropColumn("dbo.Competences", "ExternalId");
            DropTable("dbo.EduResults");
        }
    }
}
