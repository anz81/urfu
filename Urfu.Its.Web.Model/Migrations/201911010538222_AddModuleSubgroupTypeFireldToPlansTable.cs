namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModuleSubgroupTypeFireldToPlansTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "moduleSubgroupType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "moduleSubgroupType");
        }
    }
}
