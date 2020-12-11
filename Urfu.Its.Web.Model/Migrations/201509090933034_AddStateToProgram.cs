namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStateToProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "State", c => c.Int(nullable: false));
            DropColumn("dbo.Variants", "IsBase");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Variants", "IsBase", c => c.Boolean(nullable: false));
            DropColumn("dbo.EduPrograms", "State");
        }
    }
}
