namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateFileStorageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileStorage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ip = c.String(),
                        HttpUser = c.String(),
                        FileNameForUser = c.String(),
                        FileName = c.String(),
                        Path = c.String(),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileStorage");
        }
    }
}
