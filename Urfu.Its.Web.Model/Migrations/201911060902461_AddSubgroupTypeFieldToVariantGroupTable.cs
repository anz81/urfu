namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubgroupTypeFieldToVariantGroupTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VariantGroups", "SubgroupType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VariantGroups", "SubgroupType");
        }
    }
}
