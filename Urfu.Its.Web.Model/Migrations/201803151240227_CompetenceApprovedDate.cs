namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompetenceApprovedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competences", "ApprovedDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Competences", "ApprovedDate");
        }
    }
}
