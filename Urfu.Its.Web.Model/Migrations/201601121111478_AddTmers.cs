namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTmers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tmer",
                c => new
                    {
                        kmer = c.String(nullable: false, maxLength: 128),
                        kgmer = c.Int(),
                        rmer = c.String(),
                        kunr = c.Int(nullable: false),
                        ktur = c.Int(),
                        kediz = c.Int(),
                        npp = c.Int(nullable: false),
                        lnormzd = c.Int(nullable: false),
                        techLoad = c.Boolean(nullable: false),
                        techControl = c.Boolean(nullable: false),
                        techSingle = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.kmer);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Tmer");
        }
    }
}
