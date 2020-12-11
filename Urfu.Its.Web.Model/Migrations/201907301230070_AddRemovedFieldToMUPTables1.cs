namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemovedFieldToMUPTables1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MUPDisciplineTmers", "Removed", c => c.Boolean(nullable: false));
            AddColumn("dbo.MUPDisciplineTmerPeriods", "Removed", c => c.Boolean(nullable: false));
            AddColumn("dbo.MUPPeriods", "Removed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MUPPeriods", "Removed");
            DropColumn("dbo.MUPDisciplineTmerPeriods", "Removed");
            DropColumn("dbo.MUPDisciplineTmers", "Removed");
        }
    }
}
