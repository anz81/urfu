namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveQualificationForeignKeyFromCompetencesTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Competences", "QualificationName", "dbo.Qualifications");
            DropIndex("dbo.Competences", new[] { "QualificationName" });
            AlterColumn("dbo.Competences", "QualificationName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Competences", "QualificationName", c => c.String(maxLength: 127));
            CreateIndex("dbo.Competences", "QualificationName");
            AddForeignKey("dbo.Competences", "QualificationName", "dbo.Qualifications", "Name");
        }
    }
}
