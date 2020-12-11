namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MetaSubgroup_Year : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaSubgroups", "Year", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaSubgroups", "Year");
        }
    }
}
