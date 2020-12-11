namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProfStandardFieldFromProfOrderChangesTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProfOrderChanges", "ProfStandardCode", "dbo.ProfStandards");
            DropIndex("dbo.ProfOrderChanges", new[] { "ProfStandardCode" });
            DropColumn("dbo.ProfOrderChanges", "ProfStandardCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProfOrderChanges", "ProfStandardCode", c => c.String(maxLength: 20));
            CreateIndex("dbo.ProfOrderChanges", "ProfStandardCode");
            AddForeignKey("dbo.ProfOrderChanges", "ProfStandardCode", "dbo.ProfStandards", "Code");
        }
    }
}
