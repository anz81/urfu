namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueConstraitToLimits : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Limits", new[] { "ModuleId", "Year" }, unique: true, name: "IX_Limit_Unique");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Limits", "IX_Limit_Unique");
        }
    }
}
