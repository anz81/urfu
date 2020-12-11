namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDistributionToSubgroupMeta : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaSubgroups", "Distribution", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaSubgroups", "Distribution");
        }
    }
}
