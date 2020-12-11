namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SportsmanStudent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "Sportsman", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "Sportsman");
        }
    }
}
