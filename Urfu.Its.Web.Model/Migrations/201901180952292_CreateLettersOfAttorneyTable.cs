namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateLettersOfAttorneyTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LettersOfAttorney",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        Number = c.String(maxLength: 20),
                        Date = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LettersOfAttorney");
        }
    }
}
