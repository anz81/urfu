namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateFieldsForProjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectROPProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectUserId = c.Int(nullable: false),
                        ProfileId = c.String(maxLength: 127),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .ForeignKey("dbo.ProjectUsers", t => t.ProjectUserId, cascadeDelete: true)
                .Index(t => t.ProjectUserId)
                .Index(t => t.ProfileId);
            
            AddColumn("dbo.Projects", "Summary", c => c.String());
            AddColumn("dbo.Projects", "EmployersId", c => c.Int());
            AddColumn("dbo.Contracts", "Division", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectROPProfiles", "ProjectUserId", "dbo.ProjectUsers");
            DropForeignKey("dbo.ProjectROPProfiles", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.ProjectROPProfiles", new[] { "ProfileId" });
            DropIndex("dbo.ProjectROPProfiles", new[] { "ProjectUserId" });
            DropColumn("dbo.Contracts", "Division");
            DropColumn("dbo.Projects", "EmployersId");
            DropColumn("dbo.Projects", "Summary");
            DropTable("dbo.ProjectROPProfiles");
        }
    }
}
