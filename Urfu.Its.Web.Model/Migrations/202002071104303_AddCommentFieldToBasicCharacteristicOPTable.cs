namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentFieldToBasicCharacteristicOPTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BasicCharacteristicOP", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BasicCharacteristicOP", "Comment");
        }
    }
}
