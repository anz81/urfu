namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Competence_AddProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competences", "ProfileId", c => c.String(maxLength: 127));
            CreateIndex("dbo.Competences", "ProfileId");
            AddForeignKey("dbo.Competences", "ProfileId", "dbo.Profiles", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Competences", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.Competences", new[] { "ProfileId" });
            DropColumn("dbo.Competences", "ProfileId");
        }
    }
}
