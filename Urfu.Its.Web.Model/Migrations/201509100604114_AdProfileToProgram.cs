namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdProfileToProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "profileId", c => c.String(maxLength: 127));
            CreateIndex("dbo.EduPrograms", "profileId");
            AddForeignKey("dbo.EduPrograms", "profileId", "dbo.Profiles", "ID");
            Sql("update EduPrograms set profileId = (select top 1 ID from profiles where profiles.DIRECTION_ID = directionId)");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduPrograms", "profileId", "dbo.Profiles");
            DropIndex("dbo.EduPrograms", new[] { "profileId" });
            DropColumn("dbo.EduPrograms", "profileId");
        }
    }
}
