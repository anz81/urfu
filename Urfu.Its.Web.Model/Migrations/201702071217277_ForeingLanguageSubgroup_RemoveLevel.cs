namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeingLanguageSubgroup_RemoveLevel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ForeignLanguageSubgroups", "Level");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForeignLanguageSubgroups", "Level", c => c.String());
        }
    }
}
