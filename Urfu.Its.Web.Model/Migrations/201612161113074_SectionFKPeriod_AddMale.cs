namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKPeriod_AddMale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SectionFKPeriods", "Male", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SectionFKPeriods", "Male");
        }
    }
}
