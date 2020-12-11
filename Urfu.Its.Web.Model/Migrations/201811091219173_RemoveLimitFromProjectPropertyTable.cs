namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLimitFromProjectPropertyTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectProperties", "LimitId", "dbo.ContractLimits");
            DropIndex("dbo.ProjectProperties", new[] { "LimitId" });
            DropColumn("dbo.ProjectProperties", "LimitId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectProperties", "LimitId", c => c.Int());
            CreateIndex("dbo.ProjectProperties", "LimitId");
            AddForeignKey("dbo.ProjectProperties", "LimitId", "dbo.ContractLimits", "Id");
        }
    }
}
