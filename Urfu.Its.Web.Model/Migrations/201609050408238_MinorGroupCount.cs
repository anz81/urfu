namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorGroupCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorDisciplineTmerPeriods", "GroupCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MinorDisciplineTmerPeriods", "GroupCount");
        }
    }
}
