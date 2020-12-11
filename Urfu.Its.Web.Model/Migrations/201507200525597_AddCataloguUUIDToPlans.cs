namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCataloguUUIDToPlans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "catalogDisciplineUUID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "catalogDisciplineUUID");
        }
    }
}
