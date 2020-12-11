namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorSubgroupDistribution : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorDisciplineTmerPeriods", "Distribution", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MinorDisciplineTmerPeriods", "Distribution");
        }
    }
}
