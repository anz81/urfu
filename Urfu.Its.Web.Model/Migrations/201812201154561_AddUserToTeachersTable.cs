namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserToTeachersTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            AddColumn("dbo.Teachers", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.ProjectUsers", "TeacherId", c => c.String(nullable: false, maxLength: 127));
            CreateIndex("dbo.Teachers", "UserId");
            CreateIndex("dbo.ProjectUsers", "TeacherId");
            AddForeignKey("dbo.ProjectUsers", "TeacherId", "dbo.Teachers", "pkey", cascadeDelete: false);
            AddForeignKey("dbo.Teachers", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.ProjectUsers", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectUsers", "UserId", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.Teachers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "TeacherId", "dbo.Teachers");
            DropIndex("dbo.ProjectUsers", new[] { "TeacherId" });
            DropIndex("dbo.Teachers", new[] { "UserId" });
            DropColumn("dbo.ProjectUsers", "TeacherId");
            DropColumn("dbo.Teachers", "UserId");
            CreateIndex("dbo.ProjectUsers", "UserId");
            AddForeignKey("dbo.ProjectUsers", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: false);
        }
    }
}
