namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LimtNaPeriod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorPeriods", "MinStudentsCount", c => c.Int(nullable: false));
            AddColumn("dbo.MinorPeriods", "MaxStudentsCount", c => c.Int(nullable: false));
            DropColumn("dbo.Minors", "MinStudentsCount");
            DropColumn("dbo.Minors", "MaxStudentsCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Minors", "MaxStudentsCount", c => c.Int(nullable: false));
            AddColumn("dbo.Minors", "MinStudentsCount", c => c.Int(nullable: false));
            DropColumn("dbo.MinorPeriods", "MaxStudentsCount");
            DropColumn("dbo.MinorPeriods", "MinStudentsCount");
        }
    }
}
