namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeEduResult : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EduResults", "DirectionId", "dbo.Directions");
            DropForeignKey("dbo.EduResults", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.EduResults", "EduProgramId", "dbo.EduPrograms");
            DropIndex("dbo.EduResults", new[] { "DirectionId" });
            DropIndex("dbo.EduResults", new[] { "DivisionId" });
            DropIndex("dbo.EduResults", new[] { "EduProgramId" });
            AddColumn("dbo.EduResults", "CodeNumber", c => c.Int(nullable: false));
            AddColumn("dbo.EduResults", "ProfileId", c => c.String(maxLength: 127));
            CreateIndex("dbo.EduResults", "ProfileId");
            AddForeignKey("dbo.EduResults", "ProfileId", "dbo.Profiles", "ID");
            DropColumn("dbo.EduResults", "Code");
            DropColumn("dbo.EduResults", "DirectionId");
            DropColumn("dbo.EduResults", "DivisionId");
            DropColumn("dbo.EduResults", "EduProgramId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EduResults", "EduProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.EduResults", "DivisionId", c => c.String(maxLength: 127));
            AddColumn("dbo.EduResults", "DirectionId", c => c.String(maxLength: 128));
            AddColumn("dbo.EduResults", "Code", c => c.String(nullable: false, maxLength: 10));
            DropForeignKey("dbo.EduResults", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.EduResults", new[] { "ProfileId" });
            DropColumn("dbo.EduResults", "ProfileId");
            DropColumn("dbo.EduResults", "CodeNumber");
            CreateIndex("dbo.EduResults", "EduProgramId");
            CreateIndex("dbo.EduResults", "DivisionId");
            CreateIndex("dbo.EduResults", "DirectionId");
            AddForeignKey("dbo.EduResults", "EduProgramId", "dbo.EduPrograms", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EduResults", "DivisionId", "dbo.Divisions", "uuid");
            AddForeignKey("dbo.EduResults", "DirectionId", "dbo.Directions", "uid");
        }
    }
}
