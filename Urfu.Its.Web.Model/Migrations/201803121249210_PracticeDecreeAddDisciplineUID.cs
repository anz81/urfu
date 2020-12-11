namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeDecreeAddDisciplineUID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDecrees", "DisciplineUUID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeDecrees", "DisciplineUUID");
        }
    }
}
