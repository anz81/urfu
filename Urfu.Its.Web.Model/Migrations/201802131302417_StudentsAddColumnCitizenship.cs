namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentsAddColumnCitizenship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "Citizenship", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "Citizenship");
        }
    }
}
