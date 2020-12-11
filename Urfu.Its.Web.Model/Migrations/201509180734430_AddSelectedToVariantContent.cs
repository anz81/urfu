using System.Runtime.InteropServices;

namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSelectedToVariantContent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VariantContents", "Selected", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VariantContents", "Selected");
        }
    }
}
