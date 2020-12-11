namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHeadToPrograms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "HeadFullName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EduPrograms", "HeadFullName");
        }
    }
}
