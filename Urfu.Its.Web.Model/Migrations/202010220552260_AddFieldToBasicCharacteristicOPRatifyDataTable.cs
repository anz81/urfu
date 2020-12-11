namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldToBasicCharacteristicOPRatifyDataTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BasicCharacteristicOPRatifyDatas", "RatifyingPersonName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BasicCharacteristicOPRatifyDatas", "RatifyingPersonName");
        }
    }
}
