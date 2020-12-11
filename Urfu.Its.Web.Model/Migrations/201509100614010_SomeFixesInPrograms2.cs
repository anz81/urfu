namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeFixesInPrograms2 : DbMigration
    {
        public override void Up()
        {
            Sql("update EduPrograms set divisionId = (select top 1 uuid from divisions) where divisionId is null");
            DropForeignKey("dbo.EduPrograms", "divisionId", "dbo.Divisions");
            DropForeignKey("dbo.EduPrograms", "profileId", "dbo.Profiles");
            DropIndex("dbo.EduPrograms", new[] { "divisionId" });
            DropIndex("dbo.EduPrograms", new[] { "profileId" });
            AlterColumn("dbo.EduPrograms", "divisionId", c => c.String(nullable: false, maxLength: 127));
            AlterColumn("dbo.EduPrograms", "profileId", c => c.String(nullable: false, maxLength: 127));
            CreateIndex("dbo.EduPrograms", "divisionId");
            CreateIndex("dbo.EduPrograms", "profileId");
            AddForeignKey("dbo.EduPrograms", "divisionId", "dbo.Divisions", "uuid", cascadeDelete: true);
            AddForeignKey("dbo.EduPrograms", "profileId", "dbo.Profiles", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduPrograms", "profileId", "dbo.Profiles");
            DropForeignKey("dbo.EduPrograms", "divisionId", "dbo.Divisions");
            DropIndex("dbo.EduPrograms", new[] { "profileId" });
            DropIndex("dbo.EduPrograms", new[] { "divisionId" });
            AlterColumn("dbo.EduPrograms", "profileId", c => c.String(maxLength: 127));
            AlterColumn("dbo.EduPrograms", "divisionId", c => c.String(maxLength: 127));
            CreateIndex("dbo.EduPrograms", "profileId");
            CreateIndex("dbo.EduPrograms", "divisionId");
            AddForeignKey("dbo.EduPrograms", "profileId", "dbo.Profiles", "ID");
            AddForeignKey("dbo.EduPrograms", "divisionId", "dbo.Divisions", "uuid");
        }
    }
}
