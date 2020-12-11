namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDirectorsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Directors",
                c => new
                    {
                        DivisionUuid = c.String(nullable: false, maxLength: 127),
                        Surname = c.String(),
                        Name = c.String(),
                        PatronymicName = c.String(),
                    })
                .PrimaryKey(t => t.DivisionUuid)
                .ForeignKey("dbo.Divisions", t => t.DivisionUuid)
                .Index(t => t.DivisionUuid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Directors", "DivisionUuid", "dbo.Divisions");
            DropIndex("dbo.Directors", new[] { "DivisionUuid" });
            DropTable("dbo.Directors");
        }
    }
}
