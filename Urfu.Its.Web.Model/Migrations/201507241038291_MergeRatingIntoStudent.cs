namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergeRatingIntoStudent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "Id", "dbo.StudentRatings");
            DropIndex("dbo.Students", new[] { "Id" });
            AddColumn("dbo.Students", "Rating", c => c.Decimal(precision: 18, scale: 2));
            DropTable("dbo.StudentRatings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StudentRatings",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id);
            
            DropColumn("dbo.Students", "Rating");
            CreateIndex("dbo.Students", "Id");
            AddForeignKey("dbo.Students", "Id", "dbo.StudentRatings", "id");
        }
    }
}
