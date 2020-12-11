namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSectionFKPeriodStartDateAndCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SectionFKPeriods", "SelectionBegin", c => c.DateTime());
            AddColumn("dbo.SectionFKPeriods", "Course", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SectionFKPeriods", "Course");
            DropColumn("dbo.SectionFKPeriods", "SelectionBegin");
        }
    }
}
