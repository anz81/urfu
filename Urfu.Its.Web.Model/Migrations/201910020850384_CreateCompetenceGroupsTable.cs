namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCompetenceGroupsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompetenceGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AreaEducationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AreaEducation", t => t.AreaEducationId)
                .Index(t => t.AreaEducationId);
            
            AddColumn("dbo.AreaEducation", "Qualifications", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompetenceGroups", "AreaEducationId", "dbo.AreaEducation");
            DropIndex("dbo.CompetenceGroups", new[] { "AreaEducationId" });
            DropColumn("dbo.AreaEducation", "Qualifications");
            DropTable("dbo.CompetenceGroups");
        }
    }
}
