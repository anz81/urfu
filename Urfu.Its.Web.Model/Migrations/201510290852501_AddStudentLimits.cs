namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStudentLimits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Variants", "StudentsLimit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Variants", "StudentsLimit");
        }
    }
}
