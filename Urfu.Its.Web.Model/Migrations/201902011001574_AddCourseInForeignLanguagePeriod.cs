namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCourseInForeignLanguagePeriod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForeignLanguagePeriods", "Course", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForeignLanguagePeriods", "Course");
        }
    }
}
