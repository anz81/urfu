namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmailToPracticeTeachersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeTeachers", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeTeachers", "Email");
        }
    }
}
