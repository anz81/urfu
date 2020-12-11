namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParcticeDocumentAddComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDocuments", "Comment", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeDocuments", "Comment");
        }
    }
}
