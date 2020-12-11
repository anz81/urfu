namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvanceFKAdmissions : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.SectionFKAdmissions");
            AddColumn("dbo.SectionFKAdmissions", "SectionFKId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.SectionFKAdmissions", new[] { "studentId", "SectionFKCompetitionGroupId", "SectionFKId" });
            CreateIndex("dbo.SectionFKAdmissions", "SectionFKId");
            AddForeignKey("dbo.SectionFKAdmissions", "SectionFKId", "dbo.SectionFKs", "ModuleId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKAdmissions", "SectionFKId", "dbo.SectionFKs");
            DropIndex("dbo.SectionFKAdmissions", new[] { "SectionFKId" });
            DropPrimaryKey("dbo.SectionFKAdmissions");
            DropColumn("dbo.SectionFKAdmissions", "SectionFKId");
            AddPrimaryKey("dbo.SectionFKAdmissions", new[] { "studentId", "SectionFKCompetitionGroupId" });
        }
    }
}
