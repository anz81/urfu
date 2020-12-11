namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTestUnitsByTermToPlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "testUnitsByTerm", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "testUnitsByTerm");
        }
    }
}
