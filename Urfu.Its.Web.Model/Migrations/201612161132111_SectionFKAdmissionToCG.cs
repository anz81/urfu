namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKAdmissionToCG : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SectionFKAdmissions", "SectionFKPeriodId", "dbo.SectionFKPeriods");
            DropIndex("dbo.SectionFKAdmissions", new[] { "SectionFKPeriodId" });
            DropPrimaryKey("dbo.SectionFKAdmissions");
            AddColumn("dbo.SectionFKAdmissions", "SectionFKCompetitionGroupId", c => c.Int(nullable: false));
            AddColumn("dbo.SectionFKStudentSelectionPriorities", "modified", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.SectionFKAdmissions", new[] { "studentId", "SectionFKCompetitionGroupId" });
            CreateIndex("dbo.SectionFKAdmissions", "SectionFKCompetitionGroupId");
            AddForeignKey("dbo.SectionFKAdmissions", "SectionFKCompetitionGroupId", "dbo.SectionFKCompetitionGroups", "Id", cascadeDelete: true);
            DropColumn("dbo.SectionFKAdmissions", "SectionFKPeriodId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SectionFKAdmissions", "SectionFKPeriodId", c => c.Int(nullable: false));
            DropForeignKey("dbo.SectionFKAdmissions", "SectionFKCompetitionGroupId", "dbo.SectionFKCompetitionGroups");
            DropIndex("dbo.SectionFKAdmissions", new[] { "SectionFKCompetitionGroupId" });
            DropPrimaryKey("dbo.SectionFKAdmissions");
            DropColumn("dbo.SectionFKStudentSelectionPriorities", "modified");
            DropColumn("dbo.SectionFKAdmissions", "SectionFKCompetitionGroupId");
            AddPrimaryKey("dbo.SectionFKAdmissions", new[] { "studentId", "SectionFKPeriodId" });
            CreateIndex("dbo.SectionFKAdmissions", "SectionFKPeriodId");
            AddForeignKey("dbo.SectionFKAdmissions", "SectionFKPeriodId", "dbo.SectionFKPeriods", "Id", cascadeDelete: true);
        }
    }
}
