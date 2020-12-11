namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateFolderNumberOfContract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "FolderNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "FolderNumber");
        }
    }
}
