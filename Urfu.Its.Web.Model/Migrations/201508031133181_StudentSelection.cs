namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentSelection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "SelectionJson", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "SelectionJson");
        }
    }
}
