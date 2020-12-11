namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveTheShitToProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Variants", "IsBase", c => c.Boolean(nullable: false));
            AddColumn("dbo.EduPrograms", "familirizationType", c => c.String());
            AddColumn("dbo.EduPrograms", "familirizationTech", c => c.String());
            AddColumn("dbo.EduPrograms", "familirizationCondition", c => c.String());
            Sql("update EduPrograms set familirizationType = (select top 1 familirizationType from variants where EduProgramId=Id)");
            Sql("update EduPrograms set familirizationTech = (select top 1 familirizationTech from variants where EduProgramId=Id)");
            Sql("update EduPrograms set familirizationCondition = (select top 1 familirizationCondition from variants where EduProgramId=Id)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.EduPrograms", "familirizationCondition");
            DropColumn("dbo.EduPrograms", "familirizationTech");
            DropColumn("dbo.EduPrograms", "familirizationType");
            DropColumn("dbo.Variants", "IsBase");
        }
    }
}
