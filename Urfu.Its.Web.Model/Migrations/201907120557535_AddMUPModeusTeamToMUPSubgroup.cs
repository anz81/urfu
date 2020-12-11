namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMUPModeusTeamToMUPSubgroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MUPSubgroups", "MUPModeusTeamId", c => c.String(maxLength: 128));
            CreateIndex("dbo.MUPSubgroups", "MUPModeusTeamId");
            AddForeignKey("dbo.MUPSubgroups", "MUPModeusTeamId", "dbo.MUPModeusTeams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MUPSubgroups", "MUPModeusTeamId", "dbo.MUPModeusTeams");
            DropIndex("dbo.MUPSubgroups", new[] { "MUPModeusTeamId" });
            DropColumn("dbo.MUPSubgroups", "MUPModeusTeamId");
        }
    }
}
