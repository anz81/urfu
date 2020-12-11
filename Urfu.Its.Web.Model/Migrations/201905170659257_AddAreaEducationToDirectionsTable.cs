namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAreaEducationToDirectionsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Directions", "AreaEducationId", c => c.Int());
            CreateIndex("dbo.Directions", "AreaEducationId");
            AddForeignKey("dbo.Directions", "AreaEducationId", "dbo.AreaEducation", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Directions", "AreaEducationId", "dbo.AreaEducation");
            DropIndex("dbo.Directions", new[] { "AreaEducationId" });
            DropColumn("dbo.Directions", "AreaEducationId");
        }
    }
}
