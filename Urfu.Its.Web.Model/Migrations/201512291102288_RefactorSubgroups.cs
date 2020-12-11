namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactorSubgroups : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Subgroups", new[] { "VariantContentId" });
            RenameColumn(table: "dbo.Subgroups", name: "VariantContentId", newName: "VariantContent_Id");
            RenameColumn(table: "dbo.SubgroupMemberships", name: "stidentId", newName: "studentId");
            RenameIndex(table: "dbo.SubgroupMemberships", name: "IX_stidentId", newName: "IX_studentId");
            AddColumn("dbo.Subgroups", "groupId", c => c.String(maxLength: 128));
            AddColumn("dbo.Subgroups", "moduleId", c => c.String(maxLength: 128));
            AddColumn("dbo.Subgroups", "Term", c => c.Int(nullable: false));
            AddColumn("dbo.Subgroups", "programId", c => c.Int(nullable: false));
            AlterColumn("dbo.Subgroups", "VariantContent_Id", c => c.Int());
            CreateIndex("dbo.Subgroups", "groupId");
            CreateIndex("dbo.Subgroups", "moduleId");
            CreateIndex("dbo.Subgroups", "programId");
            CreateIndex("dbo.Subgroups", "VariantContent_Id");
            AddForeignKey("dbo.Subgroups", "groupId", "dbo.Groups", "Id");
            AddForeignKey("dbo.Subgroups", "moduleId", "dbo.Modules", "uuid");
            AddForeignKey("dbo.Subgroups", "programId", "dbo.EduPrograms", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subgroups", "programId", "dbo.EduPrograms");
            DropForeignKey("dbo.Subgroups", "moduleId", "dbo.Modules");
            DropForeignKey("dbo.Subgroups", "groupId", "dbo.Groups");
            DropIndex("dbo.Subgroups", new[] { "VariantContent_Id" });
            DropIndex("dbo.Subgroups", new[] { "programId" });
            DropIndex("dbo.Subgroups", new[] { "moduleId" });
            DropIndex("dbo.Subgroups", new[] { "groupId" });
            AlterColumn("dbo.Subgroups", "VariantContent_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Subgroups", "programId");
            DropColumn("dbo.Subgroups", "Term");
            DropColumn("dbo.Subgroups", "moduleId");
            DropColumn("dbo.Subgroups", "groupId");
            RenameIndex(table: "dbo.SubgroupMemberships", name: "IX_studentId", newName: "IX_stidentId");
            RenameColumn(table: "dbo.SubgroupMemberships", name: "studentId", newName: "stidentId");
            RenameColumn(table: "dbo.Subgroups", name: "VariantContent_Id", newName: "VariantContentId");
            CreateIndex("dbo.Subgroups", "VariantContentId");
        }
    }
}
