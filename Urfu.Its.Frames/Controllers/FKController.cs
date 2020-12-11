using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using Z.EntityFramework.Extensions;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using EFExtensions;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Frames.Controllers
{
    public class FKController : BaseController
    {
        // GET: FK
        public ActionResult Index(string studentId)
        {
            using (var db = new ApplicationDbContext())
            {
                var authorized = UserSecurity.IsAdmin(User.GetADName(), db);
                if (!authorized)
                    return new UnauthorizedResult();
                return View("FK", new StudentPageVM(db, studentId));
            }
        }

        public ActionResult Info(int competitionGroupId, string moduleId, string studentId)
        {
            using (var db = new ApplicationDbContext())
            {
                var file = db.UniModules().Where(m => m.uuid == moduleId).Select(m => m.file).FirstOrDefault();
                var properties = db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == competitionGroupId && p.SectionFKId == moduleId);
                var teachers = properties.SelectMany(p => p.Teachers).Distinct().Select(t => new
                {
                    t.firstName,
                    t.lastName,
                    t.middleName,
                    t.post
                }).ToList().Select(t => new TeacherVM
                (
                    t.firstName,
                    t.lastName,
                    t.middleName,
                    t.post
                )).ToList();

                var admission = db.SectionFKAdmissions.FirstOrDefault(a => a.studentId == studentId && a.SectionFKCompetitionGroupId == competitionGroupId && a.SectionFKId == moduleId);
                if (admission != null && admission.Status == AdmissionStatus.Admitted)
                {
                    teachers = db.SectionFKSubgroupMemberships.Where(m => (m.studentId == studentId) && (m.Subgroup.Meta.CompetitionGroupId == competitionGroupId))
                  .Select(m => m.Subgroup.Teacher).Distinct().Select(t => new
                  {
                      t.firstName,
                      t.lastName,
                      t.middleName,
                      t.post
                  }).ToList().Select(t => new TeacherVM
                  (
                      t.firstName,
                      t.lastName,
                      t.middleName,
                      t.post
                  )).ToList();

                }

                properties = db.SectionFKProperties.Where(p => p.SectionFKCompetitionGroupId == competitionGroupId && p.SectionFKId == moduleId);
                var places = properties.SelectMany(p => p.TrainingPlaces).Distinct().ToList().Select(p => new PlaceVM(p.Address, p.Description)).ToList();
                return PartialView(new SectionFKInfoVM
                (
                    file,
                    teachers,
                    places
                ));
            }
        }

        [ClientErrorHandler]
        public ActionResult Update(string studentId, int? priority, string moduleId, int competitionGroupId)
        {
            if (priority < 1 || priority > 50)
                throw new Exception("Недопустимый приоритет");
            using (var db = new ApplicationDbContext())
            {
                //if (User.GetStudentIds()?.All(id=>id != studentId)??true)
                //    if (!UserSecurity.IsAdmin(User.GetADName(), db))
                //        throw new HttpException(403,"У вас нет прав на изменение этого приоритета");
                if (db.SectionFKs.Any(s => s.ModuleId == moduleId && s.WithoutPriorities))
                    throw new Exception("Для этой секции изменение приоритета невозможно");


                var hasPlaces =
                db.SectionFKProperties.Where(
                        p => p.SectionFKCompetitionGroupId == competitionGroupId && p.SectionFKId == moduleId)
                    .Select(
                        p =>
                            p.Limit - db.SectionFKAdmissions.Count(a => a.SectionFKId == moduleId && a.SectionFKCompetitionGroupId == competitionGroupId && a.Status == AdmissionStatus.Admitted && !a.Student.Sportsman && (a.Student.Status == "Активный" || a.Student.Status == "Отп.с.посещ.")))
                    .Where(x => x > 0).Any();

                if (!hasPlaces && priority.HasValue)
                    throw new Exception("Недостаточно мест на секции");
                Logger.Info($"Запись приоритета студента по ФК {studentId} {priority} {moduleId} {competitionGroupId}");

                var compGroup = db.SectionFKCompetitionGroups.FirstOrDefault(g => g.Id == competitionGroupId);
                if (compGroup == null)
                    throw new Exception("Конкурсная группа не найдена");

                if (moduleId == "pstcim18hc2jg0000li0lelganc4f93s") // ОФК
                {
                    var priorities = db.SectionFKStudentSelectionPriorities
                        .Where(p => p.studentId == studentId && p.competitionGroupId == competitionGroupId).ToList();
                    db.SectionFKStudentSelectionPriorities.RemoveRange(priorities);
                    db.SaveChanges();
                    if (priority != null)
                        priority = 1;
                }

                var admission = db.SectionFKAdmissions.Include("SectionFKCompetitionGroup")
                    .FirstOrDefault(a => a.studentId == studentId && a.Status == AdmissionStatus.Admitted
                        && a.SectionFKCompetitionGroup.Id == compGroup.Id);

                var entry = new SectionFKStudentSelectionPriority
                {
                    studentId = studentId,
                    competitionGroupId = competitionGroupId,
                    sectionId = moduleId,
                    modified = DateTime.Now
                };

                // если это первый курс осенний семестр (самый первый выбор студента), то priority
                if (compGroup.SemesterId == 1 && compGroup.StudentCourse == 1 && admission == null)
                    entry.priority = priority;
                else // иначе changePriority
                    entry.changePriority = priority;

                var op = db.Upsert(new[] { entry });
                op.Key(e => e.studentId);
                op.Key(e => e.sectionId);
                op.Key(e => e.competitionGroupId);
                op.ExcludeField(e => e.CompetitionGroup);
                op.ExcludeField(e => e.Section);
                op.ExcludeField(e => e.Student);
                op.Execute();
            }
            return JsonNet("OK");
        }
    }

    public class PlaceVM
    {
        public string Address { get; set; }
        public string Description { get; set; }

        public PlaceVM(string address, string description)
        {
            Address = address;
            Description = description;
        }
    }

    public class TeacherVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Post { get; set; }

        public TeacherVM(string firstName, string lastName, string middleName, string post)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            Post = post;
        }
    }

    public class SectionFKInfoVM
    {
        public string File { get; set; }
        public List<TeacherVM> Teachers { get; set; }
        public List<PlaceVM> Places { get; set; }

        public SectionFKInfoVM(string file, List<TeacherVM> teachers, List<PlaceVM> places)
        {
            File = file;
            Teachers = teachers;
            Places = places;
        }
    }

    public class ClientErrorHandler : Attribute
    {
        public void OnException(ExceptionContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.WriteAsync(filterContext.Exception.Message);
            //response.TrySkipIisCustomErrors = true;
            response.ContentType = MediaTypeNames.Text.Plain;
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //response.StatusDescription = filterContext.Exception.Message;
            filterContext.ExceptionHandled = true;
        }
    }
}