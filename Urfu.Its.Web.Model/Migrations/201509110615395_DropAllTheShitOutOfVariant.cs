namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropAllTheShitOutOfVariant : DbMigration
    {
        public override void Up()
        {
            Sql("delete from eduprograms where not exists (select 1 from variants where variants.eduprogramid=eduprograms.id)");
            DropColumn("dbo.Variants", "familirizationType");
            DropColumn("dbo.Variants", "familirizationTech");
            DropColumn("dbo.Variants", "familirizationCondition");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Variants", "familirizationCondition", c => c.String());
            AddColumn("dbo.Variants", "familirizationTech", c => c.String());
            AddColumn("dbo.Variants", "familirizationType", c => c.String());
        }
    }
}
