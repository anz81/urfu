namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddROPInitialsToPracticeInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeInfo", "ROPInitials", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeInfo", "ROPInitials");
        }
    }
}
