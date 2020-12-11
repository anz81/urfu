namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModuleIdToRatingCoefficientTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RatingCoefficients", "ModuleId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RatingCoefficients", "ModuleId");
        }
    }
}
