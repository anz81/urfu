namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CraeteUserDivision : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserDivisions",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        DivisionId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.UserName, t.DivisionId })
                .ForeignKey("dbo.Divisions", t => t.DivisionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserName, cascadeDelete: true)
                .Index(t => t.UserName)
                .Index(t => t.DivisionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserDivisions", "UserName", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserDivisions", "DivisionId", "dbo.Divisions");
            DropIndex("dbo.UserDivisions", new[] { "DivisionId" });
            DropIndex("dbo.UserDivisions", new[] { "UserName" });
            DropTable("dbo.UserDivisions");
        }
    }
}
