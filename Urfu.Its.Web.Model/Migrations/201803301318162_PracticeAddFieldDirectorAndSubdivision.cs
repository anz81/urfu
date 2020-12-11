namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeAddFieldDirectorAndSubdivision : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "DirectorGenitive", c => c.String(maxLength: 255));
            AddColumn("dbo.Contracts", "DirectorGenitive", c => c.String(maxLength: 255));
            AddColumn("dbo.PracticeAdmissions", "Subdivision", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeAdmissions", "Subdivision");
            DropColumn("dbo.Contracts", "DirectorGenitive");
            DropColumn("dbo.Companies", "DirectorGenitive");
        }
    }
}
