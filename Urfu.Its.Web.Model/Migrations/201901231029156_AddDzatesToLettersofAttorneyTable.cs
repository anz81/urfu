namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDzatesToLettersofAttorneyTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LettersOfAttorney", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.LettersOfAttorney", "EndDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.LettersOfAttorney", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LettersOfAttorney", "Date", c => c.DateTime());
            DropColumn("dbo.LettersOfAttorney", "EndDate");
            DropColumn("dbo.LettersOfAttorney", "StartDate");
        }
    }
}
