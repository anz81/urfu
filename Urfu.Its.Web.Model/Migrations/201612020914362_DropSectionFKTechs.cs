namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropSectionFKTechs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SectionFKs", "SectionFKTechId", "dbo.SectionFKTeches");
            DropIndex("dbo.SectionFKs", new[] { "SectionFKTechId" });
            DropColumn("dbo.SectionFKs", "SectionFKTechId");
            DropTable("dbo.SectionFKTeches");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SectionFKTeches",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 127),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.SectionFKs", "SectionFKTechId", c => c.Int(nullable: false));
            CreateIndex("dbo.SectionFKs", "SectionFKTechId");
            AddForeignKey("dbo.SectionFKs", "SectionFKTechId", "dbo.SectionFKTeches", "Id", cascadeDelete: true);
        }
    }
}
