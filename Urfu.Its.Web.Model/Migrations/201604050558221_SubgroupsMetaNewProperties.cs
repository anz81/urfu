namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubgroupsMetaNewProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaSubgroups", "disciplineUUID", c => c.String(maxLength: 127));
            AddColumn("dbo.MetaSubgroups", "additionalUUID", c => c.String(maxLength: 127));

            /*Sql(
@"
Update MetaSubgroups set disciplineUUID=  (select [disciplineUUID] from plans p inner join EduPrograms pg on pg.Id=programId 
where p.catalogDisciplineUUID = catalogDisciplineUUID and 
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
where p.catalogDisciplineUUID = catalogDisciplineUUID and 
p.moduleUUID = moduleId and 
active =1  and 
versionStatus= N'Утверждено' and
p.[familirizationType]=pg.[familirizationType] and
p.[familirizationTech]=pg.[familirizationTech] and
p.[familirizationCondition] =pg.[familirizationCondition]and
p.[qualification] = pg.[qualification] and
p.versionNumber = pg.PlanVersionNumber and
p.[eduplanNumber] = pg.PlanNumber)
");*/
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaSubgroups", "additionalUUID");
            DropColumn("dbo.MetaSubgroups", "disciplineUUID");
        }
    }
}
