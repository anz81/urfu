namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetupNewFields : DbMigration
    {
        public override void Up()
        {
            Sql(
   @"
Update MetaSubgroups set disciplineUUID=  (select [disciplineUUID] from plans p inner join EduPrograms pg on pg.Id=programId 
where p.catalogDisciplineUUID = MetaSubgroups.catalogDisciplineUUID and 
p.moduleUUID = moduleId and 
active =1  and 
versionStatus= N'Утверждено' and
p.[familirizationType]=pg.[familirizationType] and
p.[familirizationTech]=pg.[familirizationTech] and
p.[familirizationCondition] =pg.[familirizationCondition]and
p.[qualification] = pg.[qualification] and
p.versionNumber = pg.PlanVersionNumber and
p.[eduplanNumber] = pg.PlanNumber)
");
            Sql(
@"
Update MetaSubgroups set additionalUUID=  (select [additionalUUID] from plans p inner join EduPrograms pg on pg.Id=programId 
where p.catalogDisciplineUUID = MetaSubgroups.catalogDisciplineUUID and 
p.moduleUUID = moduleId and 
active =1  and 
versionStatus= N'Утверждено' and
p.[familirizationType]=pg.[familirizationType] and
p.[familirizationTech]=pg.[familirizationTech] and
p.[familirizationCondition] =pg.[familirizationCondition]and
p.[qualification] = pg.[qualification] and
p.versionNumber = pg.PlanVersionNumber and
p.[eduplanNumber] = pg.PlanNumber)
");
        }
        
        public override void Down()
        {
        }
    }
}
