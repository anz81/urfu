namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMinor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserMinors",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        ModuleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserName, t.ModuleId })
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserName, cascadeDelete: true)
                .Index(t => t.UserName)
                .Index(t => t.ModuleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMinors", "UserName", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserMinors", "ModuleId", "dbo.Modules");
            DropIndex("dbo.UserMinors", new[] { "ModuleId" });
            DropIndex("dbo.UserMinors", new[] { "UserName" });
            DropTable("dbo.UserMinors");
        }
    }
}
