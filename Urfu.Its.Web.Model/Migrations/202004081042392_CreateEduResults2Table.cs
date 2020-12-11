namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateEduResults2Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EduResults2",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        SerialNumber = c.Int(nullable: false),
                        Description = c.String(),
                        EduResultTypeId = c.Int(nullable: false),
                        CompetenceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Competences", t => t.CompetenceId, cascadeDelete: true)
                .ForeignKey("dbo.EduResultTypes", t => t.EduResultTypeId, cascadeDelete: true)
                .Index(t => t.EduResultTypeId)
                .Index(t => t.CompetenceId);
            
            CreateTable(
                "dbo.EduResultTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduResults2", "EduResultTypeId", "dbo.EduResultTypes");
            DropForeignKey("dbo.EduResults2", "CompetenceId", "dbo.Competences");
            DropIndex("dbo.EduResults2", new[] { "CompetenceId" });
            DropIndex("dbo.EduResults2", new[] { "EduResultTypeId" });
            DropTable("dbo.EduResultTypes");
            DropTable("dbo.EduResults2");
        }
    }
}
