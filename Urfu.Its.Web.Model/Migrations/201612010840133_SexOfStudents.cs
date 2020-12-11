namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SexOfStudents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "Male", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "Male");
        }
    }
}
