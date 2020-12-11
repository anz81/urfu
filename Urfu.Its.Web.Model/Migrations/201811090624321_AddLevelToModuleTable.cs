namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLevelToModuleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Modules", "Level", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Modules", "Level");
        }
    }
}
