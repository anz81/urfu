namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLimits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Limits",
                c => new
                    {
                        LimitId = c.Int(nullable: false, identity: true),
                        ModuleId = c.String(nullable: false, maxLength: 128),
                        Year = c.Int(nullable: false),
                        StudentsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LimitId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId, name: "IX_Limit_Module");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Limits", "ModuleId", "dbo.Modules");
            DropIndex("dbo.Limits", "IX_Limit_Module");
            DropTable("dbo.Limits");
        }
    }
}
