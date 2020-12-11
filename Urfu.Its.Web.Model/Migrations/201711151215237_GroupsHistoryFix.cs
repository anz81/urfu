namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupsHistoryFix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GroupsHistory", "YearHistory", c => c.Int(nullable: false));
            AlterColumn("dbo.GroupsHistory", "FamType", c => c.String(maxLength: 32));
            AlterColumn("dbo.GroupsHistory", "FamTech", c => c.String(maxLength: 100));
            AlterColumn("dbo.GroupsHistory", "Qual", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GroupsHistory", "Qual", c => c.String());
            AlterColumn("dbo.GroupsHistory", "FamTech", c => c.String());
            AlterColumn("dbo.GroupsHistory", "FamType", c => c.String());
            AlterColumn("dbo.GroupsHistory", "YearHistory", c => c.String());
        }
    }
}
