namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKPropertyTrainingPlaces : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                   "dbo.SectionFKTrainingPlace",
                   c => new
                   {
                       SectionFKPropertyId = c.Int(nullable: false),
                       TrainingPlaceId = c.Int(nullable: false),
                   })
                   .PrimaryKey(t => new { t.SectionFKPropertyId, t.TrainingPlaceId })
                   .ForeignKey("dbo.SectionFKProperties", t => t.SectionFKPropertyId, cascadeDelete: true)
                   .ForeignKey("dbo.FirstTrainingPlaceFKs", t => t.TrainingPlaceId, cascadeDelete: true)
                   .Index(t => t.SectionFKPropertyId)
                   .Index(t => t.TrainingPlaceId);
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SectionFKTrainingPlace");
            AddPrimaryKey("dbo.SectionFKTrainingPlace", new[] { "FirstTrainingPlaceFK_Id", "SectionFKProperty_Id" });
            RenameIndex(table: "dbo.SectionFKTrainingPlace", name: "IX_TrainingPlaceId", newName: "IX_FirstTrainingPlaceFK_Id");
            RenameIndex(table: "dbo.SectionFKTrainingPlace", name: "IX_SectionFKPropertyId", newName: "IX_SectionFKProperty_Id");
            RenameColumn(table: "dbo.SectionFKTrainingPlace", name: "SectionFKPropertyId", newName: "SectionFKProperty_Id");
            RenameColumn(table: "dbo.SectionFKTrainingPlace", name: "TrainingPlaceId", newName: "FirstTrainingPlaceFK_Id");
            RenameTable(name: "dbo.SectionFKTrainingPlace", newName: "FirstTrainingPlaceFKSectionFKProperties");
        }
    }
}
