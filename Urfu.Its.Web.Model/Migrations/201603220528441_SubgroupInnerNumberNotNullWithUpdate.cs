namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubgroupInnerNumberNotNullWithUpdate : DbMigration
    {
        public override void Up()
        {
            Sql("update Subgroups set InnerNumber = coalesce(TRY_PARSE(right(Name,2) as int),TRY_PARSE(right(Name,1) as int))");
            AlterColumn("dbo.Subgroups", "InnerNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Subgroups", "InnerNumber", c => c.Int());
        }
    }
}
