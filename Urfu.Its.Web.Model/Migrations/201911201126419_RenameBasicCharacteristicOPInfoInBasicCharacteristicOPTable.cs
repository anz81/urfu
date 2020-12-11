namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameBasicCharacteristicOPInfoInBasicCharacteristicOPTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.BasicCharacteristicOP", name: "BasicCharacteristicOPInfoId", newName: "InfoId");
            RenameIndex(table: "dbo.BasicCharacteristicOP", name: "IX_BasicCharacteristicOPInfoId", newName: "IX_InfoId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.BasicCharacteristicOP", name: "IX_InfoId", newName: "IX_BasicCharacteristicOPInfoId");
            RenameColumn(table: "dbo.BasicCharacteristicOP", name: "InfoId", newName: "BasicCharacteristicOPInfoId");
        }
    }
}
