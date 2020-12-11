namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModuleRelationsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModuleRelations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MainModuleUUID = c.String(maxLength: 128),
                        PairedModuleUUID = c.String(maxLength: 128),
                        Group = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.MainModuleUUID)
                .ForeignKey("dbo.Modules", t => t.PairedModuleUUID)
                .Index(t => t.MainModuleUUID)
                .Index(t => t.PairedModuleUUID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleRelations", "PairedModuleUUID", "dbo.Modules");
            DropForeignKey("dbo.ModuleRelations", "MainModuleUUID", "dbo.Modules");
            DropIndex("dbo.ModuleRelations", new[] { "PairedModuleUUID" });
            DropIndex("dbo.ModuleRelations", new[] { "MainModuleUUID" });
            DropTable("dbo.ModuleRelations");
        }
    }
}
