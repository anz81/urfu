namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonalNumberToStudent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "PersonalNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "PersonalNumber");
        }
    }
}
