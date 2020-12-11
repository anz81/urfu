namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateExportToSed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDecrees", "DateExportToSed", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeDecrees", "DateExportToSed");
        }
    }
}
