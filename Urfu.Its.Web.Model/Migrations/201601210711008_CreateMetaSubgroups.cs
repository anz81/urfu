namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMetaSubgroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MetaSubgroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        groupId = c.String(maxLength: 128),
                        moduleId = c.String(maxLength: 128),
                        Term = c.Int(nullable: false),
                        programId = c.Int(nullable: false),
                        kmer = c.String(maxLength: 128),
                        catalogDisciplineUuid = c.String(maxLength: 127),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.groupId)
                .ForeignKey("dbo.Modules", t => t.moduleId)
                .ForeignKey("dbo.EduPrograms", t => t.programId, cascadeDelete: true)
                .ForeignKey("dbo.Tmer", t => t.kmer)
                .Index(t => t.groupId)
                .Index(t => t.moduleId)
                .Index(t => t.programId)
                .Index(t => t.kmer);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MetaSubgroups", "kmer", "dbo.Tmer");
            DropForeignKey("dbo.MetaSubgroups", "programId", "dbo.EduPrograms");
            DropForeignKey("dbo.MetaSubgroups", "moduleId", "dbo.Modules");
            DropForeignKey("dbo.MetaSubgroups", "groupId", "dbo.Groups");
            DropIndex("dbo.MetaSubgroups", new[] { "kmer" });
            DropIndex("dbo.MetaSubgroups", new[] { "programId" });
            DropIndex("dbo.MetaSubgroups", new[] { "moduleId" });
            DropIndex("dbo.MetaSubgroups", new[] { "groupId" });
            DropTable("dbo.MetaSubgroups");
        }
    }
}
