namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompetencesTable_AddOkso : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competences", "Okso", c => c.String(maxLength: 8));
            AlterColumn("dbo.Competences", "Code", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Competences", "Content", c => c.String(maxLength: 4000));
            AlterColumn("dbo.Competences", "Standard", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Competences", "Standard", c => c.String());
            AlterColumn("dbo.Competences", "Content", c => c.String());
            AlterColumn("dbo.Competences", "Code", c => c.String(nullable: false));
            DropColumn("dbo.Competences", "Okso");
        }
    }
}
