namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRoleSets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleSetContents",
                c => new
                    {
                        RoleSetId = c.Int(nullable: false),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleSetId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.RoleSets", t => t.RoleSetId, cascadeDelete: true)
                .Index(t => t.RoleSetId)
                .Index(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleSetContents", "RoleSetId", "dbo.RoleSets");
            DropForeignKey("dbo.RoleSetContents", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.RoleSetContents", new[] { "RoleId" });
            DropIndex("dbo.RoleSetContents", new[] { "RoleSetId" });
            DropTable("dbo.RoleSetContents");
            DropTable("dbo.RoleSets");
        }
    }
}
