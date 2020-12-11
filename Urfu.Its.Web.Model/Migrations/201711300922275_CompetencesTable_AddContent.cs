namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompetencesTable_AddContent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competences", "Content", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Competences", "Content");
        }
    }
}
