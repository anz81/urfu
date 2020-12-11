namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubgroupInnerNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subgroups", "InnerNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subgroups", "InnerNumber");
        }
    }
}
