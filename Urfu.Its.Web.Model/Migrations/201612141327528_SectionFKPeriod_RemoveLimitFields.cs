namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKPeriod_RemoveLimitFields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SectionFKPeriods", "MinStudentsCount");
            DropColumn("dbo.SectionFKPeriods", "MaxStudentsCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SectionFKPeriods", "MaxStudentsCount", c => c.Int(nullable: false));
            AddColumn("dbo.SectionFKPeriods", "MinStudentsCount", c => c.Int(nullable: false));
        }
    }
}
