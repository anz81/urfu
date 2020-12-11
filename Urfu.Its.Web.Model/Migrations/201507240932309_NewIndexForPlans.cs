namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewIndexForPlans : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentRatings",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.Plans", "familirizationType", c => c.String(maxLength: 127));
            AddColumn("dbo.Plans", "familirizationTech", c => c.String(maxLength: 127));
            AddColumn("dbo.Plans", "familirizationCondition", c => c.String(maxLength: 127));
            AddColumn("dbo.Plans", "qualification", c => c.String(maxLength: 127));
            AlterColumn("dbo.Plans", "directionId", c => c.String(maxLength: 127));
            CreateIndex("dbo.Plans", new[] { "moduleUUID", "directionId", "qualification", "familirizationType", "familirizationTech", "familirizationCondition" }, name: "IX_Plan_QueryIndex");
            CreateIndex("dbo.Students", "Id");
            AddForeignKey("dbo.Students", "Id", "dbo.StudentRatings", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "Id", "dbo.StudentRatings");
            DropIndex("dbo.Students", new[] { "Id" });
            DropIndex("dbo.Plans", "IX_Plan_QueryIndex");
            AlterColumn("dbo.Plans", "directionId", c => c.String());
            DropColumn("dbo.Plans", "qualification");
            DropColumn("dbo.Plans", "familirizationCondition");
            DropColumn("dbo.Plans", "familirizationTech");
            DropColumn("dbo.Plans", "familirizationType");
            DropTable("dbo.StudentRatings");
        }
    }
}
