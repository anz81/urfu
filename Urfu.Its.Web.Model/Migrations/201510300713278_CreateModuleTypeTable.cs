namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModuleTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VariantModuleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.VariantContents", "ModuleTypeId", c => c.Int(nullable: false));

            string[] types =
            {
                "Унифицированный" ,
                "Профессиональный",
                "По выбору (профессиональный)",
                "По выбору (майонор)",
                "По выбору (альтернативный)",
                "Факультатив"
            };
            foreach (var type in types)
            {
                Sql("Insert into VariantModuleTypes(Name)values(N'" + type + "')");
            }
            Sql("update VariantContents set ModuleTypeId = ContentType+1");

            CreateIndex("dbo.VariantContents", "ModuleTypeId");
            AddForeignKey("dbo.VariantContents", "ModuleTypeId", "dbo.VariantModuleTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VariantContents", "ModuleTypeId", "dbo.VariantModuleTypes");
            DropIndex("dbo.VariantContents", new[] { "ModuleTypeId" });
            DropColumn("dbo.VariantContents", "ModuleTypeId");
            DropTable("dbo.VariantModuleTypes");
        }
    }
}
