namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameCoefficientFieldInRaitingCoefficientsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RatingCoefficients", "Coefficient", c => c.Double(nullable: false));
            DropColumn("dbo.RatingCoefficients", "Сoefficient");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RatingCoefficients", "Сoefficient", c => c.Double(nullable: false));
            DropColumn("dbo.RatingCoefficients", "Coefficient");
        }
    }
}
