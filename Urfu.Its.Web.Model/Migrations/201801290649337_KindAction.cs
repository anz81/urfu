namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KindAction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KindAction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Okso = c.String(nullable: false),
                        DirectionId = c.String(maxLength: 128),
                        Name = c.String(),
                        ExternalId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.DirectionId)
                .Index(t => t.DirectionId);
            
            AddColumn("dbo.Competences", "KindActionId", c => c.Int());
            CreateIndex("dbo.Competences", "KindActionId");
            AddForeignKey("dbo.Competences", "KindActionId", "dbo.KindAction", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Competences", "KindActionId", "dbo.KindAction");
            DropForeignKey("dbo.KindAction", "DirectionId", "dbo.Directions");
            DropIndex("dbo.KindAction", new[] { "DirectionId" });
            DropIndex("dbo.Competences", new[] { "KindActionId" });
            DropColumn("dbo.Competences", "KindActionId");
            DropTable("dbo.KindAction");
        }
    }
}
