namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountancyGuidFieldToTeachersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teachers", "AccountancyGuid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teachers", "AccountancyGuid");
        }
    }
}
