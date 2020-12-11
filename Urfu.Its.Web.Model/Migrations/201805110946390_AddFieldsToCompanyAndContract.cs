namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsToCompanyAndContract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "PostOfDirector", c => c.String(maxLength: 255));
            AddColumn("dbo.Companies", "PostOfDirectorGenitive", c => c.String(maxLength: 255));
            AddColumn("dbo.Contracts", "PostOfDirector", c => c.String(maxLength: 255));
            AddColumn("dbo.Contracts", "PostOfDirectorGenitive", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "PostOfDirectorGenitive");
            DropColumn("dbo.Contracts", "PostOfDirector");
            DropColumn("dbo.Companies", "PostOfDirectorGenitive");
            DropColumn("dbo.Companies", "PostOfDirector");
        }
    }
}
