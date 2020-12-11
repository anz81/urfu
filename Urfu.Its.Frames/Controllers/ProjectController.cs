using System;
using System.Linq;
using System.Text;
using System.Web;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Microsoft.EntityFrameworkCore;
using EFExtensions;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Urfu.Its.Frames.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        private bool isTestAuthorize = false;

        [Authorize]
        public ActionResult LK()
        {
            using (var db = new ApplicationDbContext())
            {
                var studentIds = UserSecurity.StudentIDs(User, db);

                Response.Cookies.Append("quest", "");
                var students = db.Students.Where(s => studentIds.Contains(s.Id)).ToList();
                if (students.Count() == 0)
                    return View("Message", new Message("Студент не найден"));

                return View("ListProjects", new ProjectMainList(db, studentIds));
            }
        }

        //Вход из ЛК
        public ActionResult Index()
        {
            Response.Cookies.Append(".AspNetCore.Cookies", "");
            Response.Cookies.Append("quest", "project");

            return RedirectToAction("LK");
        }

        //Вход из ITS
        [Authorize]
        public ActionResult Student(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var studentId = UserSecurity.StudentID(User, db);
                var isAdmin = UserSecurity.IsAdmin(User.GetADName(), db);
                var isProjectView = UserSecurity.IsProjectView(User.GetADName(), db);

                Logger.Info($"Project Student studentId={studentId} isAdmin={isAdmin}");

                var student = db.Students.FirstOrDefault(s => s.Id == id);
                if (student == null)
                    return View("Message", new Message("Студент не найден"));

                if (isTestAuthorize || isAdmin || studentId == id || isProjectView)
                {
                    return View("ListProjects", new ProjectMainList(db, id));
                }

                return new UnauthorizedResult();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Project(string studentID, string groupHistoryID, int year, int semesterID, string disciplineUUID, string level, string search)
        {
            using (var db = new ApplicationDbContext())
            {
                var studentId = UserSecurity.StudentID(User, db);
                var isAdmin = UserSecurity.IsAdmin(User.GetADName(), db);

                Logger.Info($"Project studentId={studentId} isAdmin={isAdmin}");

                var model = new ProjectVM(db, studentID, groupHistoryID, year, semesterID, disciplineUUID, level, search,
                    isAdmin: isAdmin || studentID == studentId); // редактировать данные может либо студент из ЛК, либо админ из ИТС

                return View("Project", model);
            }
        }

        public ActionResult GetProjectDescription(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var module = db.Modules.FirstOrDefault(m => m.uuid == id);

                if (module?.file == null)
                    return NotFound();

                var http = (HttpWebRequest)WebRequest.Create(new Uri(module.file));
                HttpWebResponse resp = (HttpWebResponse)http.GetResponse();
                byte[] bytes = Read(resp.GetResponseStream());
                if (bytes == null)
                    return NotFound();

                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Zip, module.shortTitle ?? module.title);
            }
        }

        private byte[] Read(Stream input)
        {
            try
            {
                int bytesBuffer = 1024;
                byte[] buffer = new byte[bytesBuffer];
                using (MemoryStream ms = new MemoryStream())
                {
                    int readBytes;
                    while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, readBytes);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [ClientErrorHandler]
        public ActionResult Update(string studentId, int? priority, int? role, string moduleId, int competitionGroupId, string groupHistoryId)
        {
            if (priority < 1 || priority > 10)
                throw new Exception("Недопустимый приоритет");
            using (var db = new ApplicationDbContext())
            {
                var project = db.Projects.FirstOrDefault(p => p.ModuleId == moduleId);
                if (project == null)
                    throw new Exception("Проект не найден");
                if (project.WithoutPriorities)
                    throw new Exception("Для этого проекта изменение приоритета невозможно");

                var compGroup = db.ProjectCompetitionGroups.FirstOrDefault(g => g.Id == competitionGroupId);
                if (compGroup == null)
                    throw new Exception("Проектная группа не найдена");

                var group = db.GroupsHistories.FirstOrDefault(g => g.Id == groupHistoryId);
                if (group == null)
                    throw new Exception("Группа не найдена");

                if (db.ProjectStudentSelectionPriorities.Any(p => p.studentId == studentId
                                            && p.priority == priority && priority != null && p.projectId != moduleId && p.competitionGroupId == competitionGroupId))
                    throw new Exception("Данный приоритет уже выбран на другом проекте");

                var hasPlaces = db.ContractLimits.Where(l =>
                    l.Period.ContractId == project.ContractId
                    && l.Period.SemesterId == compGroup.SemesterId
                    && l.Period.Year == compGroup.Year
                    && (l.Course == 0 || l.Course == group.Course)
                    && (l.DirectionId == null || l.DirectionId == group.Profile.DIRECTION_ID)
                    && (l.ProfileId == null || l.ProfileId == group.ProfileId)
                    && (l.QualificationName == null || l.QualificationName == group.Qual)
                    )
                    .Select(l => l.Limit
                        - db.ProjectAdmissions.Count(a => a.ProjectId == moduleId && a.Status == AdmissionStatus.Admitted
                                    && a.ProjectCompetitionGroupId == competitionGroupId
                                    && (a.Student.Group.Profile.ID == group.ProfileId)
                                    && (a.Student.Group.Profile.DIRECTION_ID == group.Profile.DIRECTION_ID)
                        ))
                    .Any(l => l > 0);

                if (!hasPlaces && priority.HasValue)
                    throw new Exception("Недостаточно мест на проекте");

                ProjectRole projectRole = role.HasValue ? db.ProjectRoles.FirstOrDefault(r => r.Id == role.Value) : null;
                Logger.Info($"Запись приоритета студента по проекту {studentId} {priority} {projectRole?.Title} {moduleId} {competitionGroupId}");

                var entry = new ProjectStudentSelectionPriority
                {
                    studentId = studentId,
                    competitionGroupId = competitionGroupId,
                    projectId = moduleId,
                    modified = DateTime.Now,
                    priority = priority,
                    roleId = role
                };

                var op = db.Upsert(new[] { entry });
                op.Key(e => e.studentId);
                op.Key(e => e.projectId);
                op.Key(e => e.competitionGroupId);
                op.ExcludeField(e => e.CompetitionGroup);
                op.ExcludeField(e => e.Project);
                op.ExcludeField(e => e.Student);
                op.ExcludeField(e => e.Role);
                op.Execute();
            }
            return JsonNet("OK");
        }

        [ClientErrorHandler]
        public ActionResult SaveComment(string studentId, string projectId, int competitionGroupId, string comment)
        {
            using (var db = new ApplicationDbContext())
            {
                var project = db.Projects.FirstOrDefault(p => p.ModuleId == projectId);

                if (project == null)
                    throw new Exception("Проект не найден");

                var compGroup = db.ProjectCompetitionGroups.FirstOrDefault(g => g.Id == competitionGroupId);
                if (compGroup == null)
                    throw new Exception("Проектная группа не найдена");

                var projectPriority = db.ProjectStudentSelectionPriorities.FirstOrDefault(p => p.studentId == studentId &&
                                                                                     p.competitionGroupId ==
                                                                                     competitionGroupId &&
                                                                                     p.projectId == projectId);

                if (projectPriority != null && (projectPriority.roleId == null || projectPriority.priority == null))
                    throw new Exception("Для данного проекта не указаны приоритет или роль ");

                projectPriority.Comment = comment;

                db.SaveChanges();

                return JsonNet("OK");

            }
        }
    }
}

