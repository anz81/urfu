namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorSubgroupFrozenProp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorSubgroups", "MarksFrozen", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MinorSubgroups", "MarksFrozen");
        }
    }
}
