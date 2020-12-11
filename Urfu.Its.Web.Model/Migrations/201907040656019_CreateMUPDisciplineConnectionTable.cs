namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMUPDisciplineConnectionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MUPDisciplineConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPModeusId = c.String(maxLength: 128),
                        ModuleId = c.String(maxLength: 128),
                        DisciplineId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.MUPModeus", t => t.MUPModeusId)
                .Index(t => t.MUPModeusId)
                .Index(t => t.ModuleId)
                .Index(t => t.DisciplineId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MUPDisciplineConnections", "MUPModeusId", "dbo.MUPModeus");
            DropForeignKey("dbo.MUPDisciplineConnections", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.MUPDisciplineConnections", "DisciplineId", "dbo.Disciplines");
            DropIndex("dbo.MUPDisciplineConnections", new[] { "DisciplineId" });
            DropIndex("dbo.MUPDisciplineConnections", new[] { "ModuleId" });
            DropIndex("dbo.MUPDisciplineConnections", new[] { "MUPModeusId" });
            DropTable("dbo.MUPDisciplineConnections");
        }
    }
}
