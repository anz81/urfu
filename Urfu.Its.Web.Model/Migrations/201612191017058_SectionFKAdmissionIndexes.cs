namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKAdmissionIndexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SectionFKAdmissions", new[] { "SectionFKId" });
            RenameIndex(table: "dbo.SectionFKAdmissions", name: "IX_SectionFKCompetitionGroupId", newName: "IX_SectionFKAdmission_SectionFKCompetitionGroupId");
            CreateIndex("dbo.SectionFKAdmissions", new[] { "SectionFKId", "SectionFKCompetitionGroupId", "Status" }, name: "IX_SectionFKAdmission_Count");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SectionFKAdmissions", "IX_SectionFKAdmission_Count");
            RenameIndex(table: "dbo.SectionFKAdmissions", name: "IX_SectionFKAdmission_SectionFKCompetitionGroupId", newName: "IX_SectionFKCompetitionGroupId");
            CreateIndex("dbo.SectionFKAdmissions", "SectionFKId");
        }
    }
}
