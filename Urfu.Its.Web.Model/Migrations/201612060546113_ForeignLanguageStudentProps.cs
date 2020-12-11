namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignLanguageStudentProps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "ForeignLanguageRating", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Students", "ForeignLanguageLevel", c => c.String());
            AddColumn("dbo.Students", "ForeignLanguageTargetLevel", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "ForeignLanguageTargetLevel");
            DropColumn("dbo.Students", "ForeignLanguageLevel");
            DropColumn("dbo.Students", "ForeignLanguageRating");
        }
    }
}
