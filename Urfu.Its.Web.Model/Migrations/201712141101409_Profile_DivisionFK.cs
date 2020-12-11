namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Profile_DivisionFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Competences", "DivisionId", "dbo.Divisions");
            DropIndex("dbo.Competences", new[] { "DivisionId" });
            AlterColumn("dbo.Profiles", "CHAIR_ID", c => c.String(maxLength: 127));
            CreateIndex("dbo.Profiles", "CHAIR_ID");
            AddForeignKey("dbo.Profiles", "CHAIR_ID", "dbo.Divisions", "uuid");
            DropColumn("dbo.Competences", "DivisionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Competences", "DivisionId", c => c.String(maxLength: 127));
            DropForeignKey("dbo.Profiles", "CHAIR_ID", "dbo.Divisions");
            DropIndex("dbo.Profiles", new[] { "CHAIR_ID" });
            AlterColumn("dbo.Profiles", "CHAIR_ID", c => c.String());
            CreateIndex("dbo.Competences", "DivisionId");
            AddForeignKey("dbo.Competences", "DivisionId", "dbo.Divisions", "uuid");
        }
    }
}
