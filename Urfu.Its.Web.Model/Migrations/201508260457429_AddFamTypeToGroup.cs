namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFamTypeToGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "FamType", c => c.String());
            AddColumn("dbo.Groups", "FamTech", c => c.String());
            AddColumn("dbo.Groups", "FamCond", c => c.String());
            AddColumn("dbo.Groups", "FamPeriod", c => c.String());
            AddColumn("dbo.Groups", "Qual", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "Qual");
            DropColumn("dbo.Groups", "FamPeriod");
            DropColumn("dbo.Groups", "FamCond");
            DropColumn("dbo.Groups", "FamTech");
            DropColumn("dbo.Groups", "FamType");
        }
    }
}
