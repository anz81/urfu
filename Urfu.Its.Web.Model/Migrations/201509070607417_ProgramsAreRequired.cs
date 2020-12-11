namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProgramsAreRequired : DbMigration
    {
        public override void Up()
        {
            Sql("insert into eduprograms(Name,directionId)  select distinct N'Программа №1',directionid from variants");
            Sql("update variants set EduProgramId = (select top 1 id from EduPrograms ep where ep.directionId=directionId)");
            DropIndex("dbo.Variants", new[] { "EduProgramId" });
            AlterColumn("dbo.Variants", "EduProgramId", c => c.Int(nullable: false));
            CreateIndex("dbo.Variants", "EduProgramId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Variants", new[] { "EduProgramId" });
            AlterColumn("dbo.Variants", "EduProgramId", c => c.Int());
            CreateIndex("dbo.Variants", "EduProgramId");
        }
    }
}
