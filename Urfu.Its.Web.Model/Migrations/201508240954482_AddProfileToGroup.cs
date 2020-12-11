namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfileToGroup : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Groups", "ProfileId", c => c.String(maxLength: 127));
            CreateIndex("dbo.Groups", "ProfileId");
            AddForeignKey("dbo.Groups", "ProfileId", "dbo.Profiles", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Groups", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.Groups", new[] { "ProfileId" });
            AlterColumn("dbo.Groups", "ProfileId", c => c.String());
        }
    }
}
