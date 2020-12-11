namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 127),
                        CODE = c.String(),
                        NAME = c.String(),
                        CHAIR_ID = c.String(),
                        QUALIFICATION = c.String(),
                        DIRECTION_ID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Directions", t => t.DIRECTION_ID)
                .Index(t => t.DIRECTION_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "DIRECTION_ID", "dbo.Directions");
            DropIndex("dbo.Profiles", new[] { "DIRECTION_ID" });
            DropTable("dbo.Profiles");
        }
    }
}
