namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWorkingProgramResponsiblePersons : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkingProgramResponseblePersons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisciplineId = c.String(maxLength: 128),
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.DisciplineId)
                .Index(t => t.ModuleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkingProgramResponseblePersons", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkingProgramResponseblePersons", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.WorkingProgramResponseblePersons", "DisciplineId", "dbo.Disciplines");
            DropIndex("dbo.WorkingProgramResponseblePersons", new[] { "UserId" });
            DropIndex("dbo.WorkingProgramResponseblePersons", new[] { "ModuleId" });
            DropIndex("dbo.WorkingProgramResponseblePersons", new[] { "DisciplineId" });
            DropTable("dbo.WorkingProgramResponseblePersons");
        }
    }
}
