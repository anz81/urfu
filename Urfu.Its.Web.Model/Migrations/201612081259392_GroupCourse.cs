namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "Course", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "Course");
        }
    }
}
