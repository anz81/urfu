namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModuleGroupTypeToPlansTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "moduleGroupType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "moduleGroupType");
        }
    }
}
