namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDiplomaQualificationFieldToDirectionsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Directions", "diplomaQualification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Directions", "diplomaQualification");
        }
    }
}
