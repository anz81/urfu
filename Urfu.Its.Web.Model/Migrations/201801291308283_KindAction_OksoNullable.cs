namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KindAction_OksoNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KindAction", "Okso", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KindAction", "Okso", c => c.String(nullable: false));
        }
    }
}
