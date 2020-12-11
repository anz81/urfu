namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStandardsAndCompetenceTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompetenceTypes",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false, maxLength: 200),
                        IsStandard = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Standards",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Name);
            
            Sql(@"
INSERT INTO Standards (Name) VALUES
	('ФГОС ВО'),
	('ФГОС ВО 3++')

INSERT INTO CompetenceTypes (Name, Description, IsStandard) VALUES
	('ОК', 'Общекультурные компетенции', 1),
	('ОПК', 'Общепрофессиональные компетенции', 1),
	('ПК', 'Профессиональные компетенции', 1),
	('УК', 'УК', 1),
	('ПСК', 'Профессионально-специализированные компетенции', 1),
	('ДОК', 'Дополнительные общекультурные компетенции', 0),
	('ДОПК', 'Дополнительные общепрофессиональные компетенции', 0),
	('ДПК', 'Дополнительные профессиональные компетенции', 0)
            ");
        }
        
        public override void Down()
        {
            DropTable("dbo.Standards");
            DropTable("dbo.CompetenceTypes");
        }
    }
}
