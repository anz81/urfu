namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemovedFieldsToMUPModeusTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MUPModeusTeams", "Removed", c => c.Boolean(nullable: false));
            AddColumn("dbo.MUPModeusRealizations", "Removed", c => c.Boolean(nullable: false));
            AddColumn("dbo.MUPModeusTeamStudents", "Removed", c => c.Boolean(nullable: false));
            AddColumn("dbo.MUPModeusTeachers", "Removed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MUPModeusTeachers", "Removed");
            DropColumn("dbo.MUPModeusTeamStudents", "Removed");
            DropColumn("dbo.MUPModeusRealizations", "Removed");
            DropColumn("dbo.MUPModeusTeams", "Removed");
        }
    }
}
