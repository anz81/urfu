namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteYearFromLettersofAttorneyTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LettersOfAttorney", "Year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LettersOfAttorney", "Year", c => c.Int(nullable: false));
        }
    }
}
