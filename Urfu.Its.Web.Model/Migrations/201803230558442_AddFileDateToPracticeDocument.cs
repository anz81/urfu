namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileDateToPracticeDocument : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDocuments", "FileDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeDocuments", "FileDate");
        }
    }
}
