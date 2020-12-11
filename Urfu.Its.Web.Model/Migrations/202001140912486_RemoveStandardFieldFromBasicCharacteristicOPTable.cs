namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveStandardFieldFromBasicCharacteristicOPTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BasicCharacteristicOP", "StandardName", "dbo.Standards");
            DropIndex("dbo.BasicCharacteristicOP", new[] { "StandardName" });
            DropColumn("dbo.BasicCharacteristicOP", "StandardName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BasicCharacteristicOP", "StandardName", c => c.String(nullable: false, maxLength: 20));
            CreateIndex("dbo.BasicCharacteristicOP", "StandardName");
            AddForeignKey("dbo.BasicCharacteristicOP", "StandardName", "dbo.Standards", "Name", cascadeDelete: true);
        }
    }
}
