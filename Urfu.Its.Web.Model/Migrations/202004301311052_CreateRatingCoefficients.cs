namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRatingCoefficients : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RatingCoefficients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleType = c.Int(nullable: false),
                        Level = c.String(),
                        Сoefficient = c.Double(nullable: false),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RatingCoefficients");
        }
    }
}
