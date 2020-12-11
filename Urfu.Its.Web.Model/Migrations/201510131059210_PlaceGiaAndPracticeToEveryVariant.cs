namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlaceGiaAndPracticeToEveryVariant : DbMigration
    {
        public override void Up()
        {
            Sql("insert into VariantGroups(GroupType,TestUnits,VariantId) select 4, 0, Id from variants");
            Sql("insert into VariantGroups(GroupType,TestUnits,VariantId) select 5, 0, Id from variants");
        }
        
        public override void Down()
        {
        }
    }
}
