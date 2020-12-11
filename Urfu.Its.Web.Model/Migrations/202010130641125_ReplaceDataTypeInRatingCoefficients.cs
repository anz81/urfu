namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplaceDataTypeInRatingCoefficients : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE dbo.RatingCoefficients DROP CONSTRAINT DF__RatingCoe__Coeff__54B751CD");
            AlterColumn("dbo.RatingCoefficients", "Coefficient", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RatingCoefficients", "Coefficient", c => c.Double(nullable: false));
        }
    }
}
