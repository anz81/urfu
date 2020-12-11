namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactorSubgroupMeta : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Subgroups", "groupId", "dbo.Groups");
            DropForeignKey("dbo.Subgroups", "moduleId", "dbo.Modules");
            DropForeignKey("dbo.Subgroups", "programId", "dbo.EduPrograms");
            DropForeignKey("dbo.Subgroups", "kmer", "dbo.Tmer");
            DropIndex("dbo.Subgroups", new[] { "groupId" });
            DropIndex("dbo.Subgroups", new[] { "moduleId" });
            DropIndex("dbo.Subgroups", new[] { "programId" });
            DropIndex("dbo.Subgroups", new[] { "kmer" });
            AddColumn("dbo.Subgroups", "MetaSubgroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Subgroups", "MetaSubgroupId");
            AddForeignKey("dbo.Subgroups", "MetaSubgroupId", "dbo.MetaSubgroups", "Id", cascadeDelete: true);
            DropColumn("dbo.Subgroups", "groupId");
            DropColumn("dbo.Subgroups", "moduleId");
            DropColumn("dbo.Subgroups", "Term");
            DropColumn("dbo.Subgroups", "programId");
            DropColumn("dbo.Subgroups", "kmer");
            DropColumn("dbo.Subgroups", "catalogDisciplineUuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subgroups", "catalogDisciplineUuid", c => c.String(maxLength: 127));
            AddColumn("dbo.Subgroups", "kmer", c => c.String(maxLength: 128));
            AddColumn("dbo.Subgroups", "programId", c => c.Int(nullable: false));
            AddColumn("dbo.Subgroups", "Term", c => c.Int(nullable: false));
            AddColumn("dbo.Subgroups", "moduleId", c => c.String(maxLength: 128));
            AddColumn("dbo.Subgroups", "groupId", c => c.String(maxLength: 128));
            DropForeignKey("dbo.Subgroups", "MetaSubgroupId", "dbo.MetaSubgroups");
            DropIndex("dbo.Subgroups", new[] { "MetaSubgroupId" });
            DropColumn("dbo.Subgroups", "MetaSubgroupId");
            CreateIndex("dbo.Subgroups", "kmer");
            CreateIndex("dbo.Subgroups", "programId");
            CreateIndex("dbo.Subgroups", "moduleId");
            CreateIndex("dbo.Subgroups", "groupId");
            AddForeignKey("dbo.Subgroups", "kmer", "dbo.Tmer", "kmer");
            AddForeignKey("dbo.Subgroups", "programId", "dbo.EduPrograms", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Subgroups", "moduleId", "dbo.Modules", "uuid");
            AddForeignKey("dbo.Subgroups", "groupId", "dbo.Groups", "Id");
        }
    }
}
