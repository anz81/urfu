namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersionUidToPlan : DbMigration
    {
        public override void Up()
        {
            Sql("delete plans");
            DropPrimaryKey("dbo.Plans");
            AddColumn("dbo.Plans", "versionUUID", c => c.String(nullable: false, maxLength: 127));
            AddPrimaryKey("dbo.Plans", new[] { "disciplineUUID", "moduleUUID", "eduplanUUID", "versionUUID" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Plans");
            DropColumn("dbo.Plans", "versionUUID");
            AddPrimaryKey("dbo.Plans", new[] { "disciplineUUID", "moduleUUID", "eduplanUUID" });
        }
    }
}
