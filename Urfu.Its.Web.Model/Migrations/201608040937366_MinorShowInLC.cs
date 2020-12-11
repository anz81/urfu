namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorShowInLC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Minors", "ShowInLC", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Minors", "ShowInLC");
        }
    }
}
