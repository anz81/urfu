namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDdetailDiscipline : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Apploads", "detailDiscipline", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Apploads", "detailDiscipline");
        }
    }
}
