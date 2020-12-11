namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TmerMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subgroups", "kmer", c => c.String(maxLength: 128));
            CreateIndex("dbo.Subgroups", "kmer");
            AddForeignKey("dbo.Subgroups", "kmer", "dbo.Tmer", "kmer");
            DropColumn("dbo.Subgroups", "SubgroupType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subgroups", "SubgroupType", c => c.Int(nullable: false));
            DropForeignKey("dbo.Subgroups", "kmer", "dbo.Tmer");
            DropIndex("dbo.Subgroups", new[] { "kmer" });
            DropColumn("dbo.Subgroups", "kmer");
        }
    }
}
