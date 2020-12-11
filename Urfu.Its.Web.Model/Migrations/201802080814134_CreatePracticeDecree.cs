namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePracticeDecree : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeDecrees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.String(maxLength: 128),
                        Status = c.Int(nullable: false),
                        Number = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GroupsHistory", t => t.GroupId)
                .Index(t => t.GroupId);
            
            AddColumn("dbo.PracticeInfo", "GroupId", c => c.String(maxLength: 128));
            AddColumn("dbo.PracticeInfo", "SemesterId", c => c.Int(nullable: false));
            CreateIndex("dbo.PracticeInfo", "GroupId");
            CreateIndex("dbo.PracticeInfo", "SemesterId");
            AddForeignKey("dbo.PracticeInfo", "GroupId", "dbo.GroupsHistory", "Id");
            AddForeignKey("dbo.PracticeInfo", "SemesterId", "dbo.Semesters", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeInfo", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.PracticeInfo", "GroupId", "dbo.GroupsHistory");
            DropForeignKey("dbo.PracticeDecrees", "GroupId", "dbo.GroupsHistory");
            DropIndex("dbo.PracticeInfo", new[] { "SemesterId" });
            DropIndex("dbo.PracticeInfo", new[] { "GroupId" });
            DropIndex("dbo.PracticeDecrees", new[] { "GroupId" });
            DropColumn("dbo.PracticeInfo", "SemesterId");
            DropColumn("dbo.PracticeInfo", "GroupId");
            DropTable("dbo.PracticeDecrees");
        }
    }
}
