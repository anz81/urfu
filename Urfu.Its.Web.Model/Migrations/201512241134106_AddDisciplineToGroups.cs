namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDisciplineToGroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subgroups", "catalogDisciplineUuid", c => c.String(maxLength: 127));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subgroups", "catalogDisciplineUuid");
        }
    }
}
