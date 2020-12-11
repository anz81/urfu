namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeDecreeAddFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDecrees", "FileName", c => c.String(maxLength: 250));
            AddColumn("dbo.PracticeDecrees", "FileData", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeDecrees", "FileData");
            DropColumn("dbo.PracticeDecrees", "FileName");
        }
    }
}
