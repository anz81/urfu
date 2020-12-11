namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResetMinorMarks : DbMigration
    {
        public override void Up()
        {
            Sql("update MinorSubgroupMemberships set mark = null");
        }
        
        public override void Down()
        {
        }
    }
}
