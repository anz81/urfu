namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompetenceTable_Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Competences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        Type = c.String(nullable: false),
                        DirectionId = c.String(maxLength: 128),
                        DivisionId = c.String(maxLength: 127),
                        Standard = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.DirectionId)
                .ForeignKey("dbo.Divisions", t => t.DivisionId)
                .Index(t => t.DirectionId)
                .Index(t => t.DivisionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Competences", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.Competences", "DirectionId", "dbo.Directions");
            DropIndex("dbo.Competences", new[] { "DivisionId" });
            DropIndex("dbo.Competences", new[] { "DirectionId" });
            DropTable("dbo.Competences");
        }
    }
}
