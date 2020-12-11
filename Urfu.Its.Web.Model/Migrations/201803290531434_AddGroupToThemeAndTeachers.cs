namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupToThemeAndTeachers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeThemes", "GroupHistoryId", c => c.String(maxLength: 128));
            AddColumn("dbo.PracticeTeachers", "GroupHistoryId", c => c.String(maxLength: 128));
            CreateIndex("dbo.PracticeThemes", "GroupHistoryId");
            CreateIndex("dbo.PracticeTeachers", "GroupHistoryId");
            AddForeignKey("dbo.PracticeThemes", "GroupHistoryId", "dbo.GroupsHistory", "Id");
            AddForeignKey("dbo.PracticeTeachers", "GroupHistoryId", "dbo.GroupsHistory", "Id");

            Sql(
@"
insert into PracticeThemes(DisciplineUUID, Theme, Year, SemesterId, GroupHistoryId)
select t.DisciplineUUID, t.Theme, t.Year, t.SemesterId , gg.Id from PracticeThemes t
left join 
(
  select g.groupID, h.Id, h.YearHistory,  t.DisciplineUUID, t.Term, t.Course, t.SemesterID from Plans p
   join PlanDisciplineTerms t on t.DisciplineUUID = p.[disciplineUUID]
   join 
   (
     select distinct groupID, versionNumber, planVerion from  Students s where s.Status='Активный'
   ) g on g.versionNumber = p.versionNumber and g.planVerion = p.eduplanNumber
   join groupsHistory h on  h.GroupId = g.GroupId and h.Course = t.course
   where p.additionalType in ('Производственная практика', 'Учебная практика') 
)  gg on t.DisciplineUUID = gg.DisciplineUUID and t.Year = gg.YearHistory  and t.SemesterId = gg.SemesterID

update PracticeAdmissions
set PracticeThemeId  = t2.Id
from PracticeAdmissions a 
join Practices p on a.PracticeId = p.Id
join PracticeThemes t on t.Id = a.PracticeThemeId
join PracticeThemes t2 on t2.DisciplineUUID = t.DisciplineUUID
 and t2.SemesterId = t.SemesterId 
 and t2.Year = t.Year 
 and t2.Theme = t.Theme
 and t2.GroupHistoryId = p.GroupHistoryId 

insert into PracticeTeachers([DisciplineUUID],[TeacherPKey],[Year] ,[SemesterId], GroupHistoryId)
select t.DisciplineUUID, t.TeacherPKey, t.Year, t.SemesterId , gg.Id from PracticeTeachers t
left join 
(
  select g.groupID, h.Id, h.YearHistory,  t.DisciplineUUID, t.Term, t.Course, t.SemesterID from Plans p
   join PlanDisciplineTerms t on t.DisciplineUUID = p.[disciplineUUID]
   join 
   (
     select distinct groupID, versionNumber, planVerion from  Students s where s.Status='Активный'
   ) g on g.versionNumber = p.versionNumber and g.planVerion = p.eduplanNumber
   join groupsHistory h on  h.GroupId = g.GroupId and h.Course = t.course
   where p.additionalType in ('Производственная практика', 'Учебная практика') 
) gg on t.DisciplineUUID = gg.DisciplineUUID and t.Year = gg.YearHistory  and t.SemesterId = gg.SemesterID
");

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeTeachers", "GroupHistoryId", "dbo.GroupsHistory");
            DropForeignKey("dbo.PracticeThemes", "GroupHistoryId", "dbo.GroupsHistory");
            DropIndex("dbo.PracticeTeachers", new[] { "GroupHistoryId" });
            DropIndex("dbo.PracticeThemes", new[] { "GroupHistoryId" });
            DropColumn("dbo.PracticeTeachers", "GroupHistoryId");
            DropColumn("dbo.PracticeThemes", "GroupHistoryId");
        }
    }
}
