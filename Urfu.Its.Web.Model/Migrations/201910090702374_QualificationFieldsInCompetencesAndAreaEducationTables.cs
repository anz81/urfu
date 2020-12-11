namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QualificationFieldsInCompetencesAndAreaEducationTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competences", "QualificationName", c => c.String(maxLength: 127));
            CreateIndex("dbo.Competences", "QualificationName");
            AddForeignKey("dbo.Competences", "QualificationName", "dbo.Qualifications", "Name");
            DropColumn("dbo.AreaEducation", "Qualifications");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AreaEducation", "Qualifications", c => c.String());
            DropForeignKey("dbo.Competences", "QualificationName", "dbo.Qualifications");
            DropIndex("dbo.Competences", new[] { "QualificationName" });
            DropColumn("dbo.Competences", "QualificationName");
        }
    }
}
