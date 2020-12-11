namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveYearAndQualToProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "qualification", c => c.String());
            AddColumn("dbo.EduPrograms", "Year", c => c.Int(nullable: false));
            Sql("update EduPrograms set qualification = (select top 1 qualification from Variants where Variants.EduProgramId=Id)");
            Sql("update EduPrograms set Year = (select top 1 Year from Variants where Variants.EduProgramId=Id)");
            DropColumn("dbo.Variants", "qualification");
            DropColumn("dbo.Variants", "Year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Variants", "Year", c => c.Int(nullable: false));
            AddColumn("dbo.Variants", "qualification", c => c.String());
            DropColumn("dbo.EduPrograms", "Year");
            DropColumn("dbo.EduPrograms", "qualification");
        }
    }
}
