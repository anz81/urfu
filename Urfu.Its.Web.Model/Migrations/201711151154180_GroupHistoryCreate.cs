namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupHistoryCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupsHistory",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        GroupId = c.String(maxLength: 128),
                        Name = c.String(maxLength: 255),
                        ProfileId = c.String(maxLength: 127),
                        YearHistory = c.String(),
                        Course = c.Int(nullable: false),
                        ChairId = c.String(maxLength: 32),
                        ManagingDivisionId = c.String(maxLength: 32),
                        FamType = c.String(),
                        FamTech = c.String(),
                        Qual = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .Index(t => t.GroupId)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupsHistory", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.GroupsHistory", "GroupId", "dbo.Groups");
            DropIndex("dbo.GroupsHistory", new[] { "ProfileId" });
            DropIndex("dbo.GroupsHistory", new[] { "GroupId" });
            DropTable("dbo.GroupsHistory");
        }
    }
}
