namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeFieldsToMUPModeusTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MUPModeus", "Directions", c => c.String());
            AddColumn("dbo.MUPModeusTeachers", "EventTypes", c => c.String());
            AddColumn("dbo.MUPModeusTeams", "EventTypes", c => c.String());
            AddColumn("dbo.MUPModeusTeams", "Kmers", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MUPModeusTeams", "Kmers");
            DropColumn("dbo.MUPModeusTeams", "EventTypes");
            DropColumn("dbo.MUPModeusTeachers", "EventTypes");
            DropColumn("dbo.MUPModeus", "Directions");
        }
    }
}
