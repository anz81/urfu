namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentPlanVersions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "planVerion", c => c.Int());
            AddColumn("dbo.Students", "versionNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "versionNumber");
            DropColumn("dbo.Students", "planVerion");
        }
    }
}
