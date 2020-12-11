namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReorderGroupTypes : DbMigration
    {
        public override void Up()
        {
            Sql("update VariantGroups set GroupType = 6 where GroupType = 4");
        }
        
        public override void Down()
        {
        }
    }
}
