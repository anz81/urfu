namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsToCompetencePassportTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompetencePassports", "Year", c => c.Int(nullable: false));
            AddColumn("dbo.CompetencePassports", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.CompetencePassports", "BasedOnId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompetencePassports", "BasedOnId");
            DropColumn("dbo.CompetencePassports", "Version");
            DropColumn("dbo.CompetencePassports", "Year");
        }
    }
}
