using Ext.Utilities;
//using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TemplateEngine;
using Urfu.Its.Integration;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model;
using Urfu.Its.Web.Model.Models.Practice;
using Urfu.Its.Web.Models;
using Microsoft.Extensions.Hosting.Internal;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.PracticeView)]
    public class PracticeOrdersChangesController : Controller
    {
        private static List<string> PlanPracticeTypes = new List<string> { "Учебная практика", "Производственная практика" };

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PracticeOrdersChanges
        public ActionResult Index(int? id, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                return DecreeAjax(id);
            }
            else
            {
                var decree = db.PracticeDecrees.Include(d => d.Group).FirstOrDefault(d => d.Id == id);
                ChangedDecreesViewModel model = new ChangedDecreesViewModel();

                if (decree == null)
                    return View(model);

                var plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == decree.DisciplineUUID);
                var practiceInfo = db.PracticeInfo.FirstOrDefault(p =>
                    p.DisciplineUUID == plan.disciplineUUID && p.GroupId == decree.GroupId && p.SemesterId == decree.SemesterID);

                model.DisciplineTitle = plan?.disciplineTitle; 
                model.Group = decree.Group.Name; 
                model.GroupId = decree.GroupId;
                model.IsOldPlanVersion = plan.remove;
                model.IsRemovedDiscipline = plan.remove;
                model.MainDecreeId = id;
                model.MainDecreeSedId = decree.SedId;
                model.PlanNumber = plan.eduplanNumber.Value;
                model.PlanVersion = plan.versionNumber;
                model.PlanStatus = plan.versionStatus;
                model.Semester = db.Semesters.FirstOrDefault(s => s.Id == decree.SemesterID).Name;
                model.GroupYear = decree.Group.YearHistory.ToString();
                model.DecreeNumber = db.PracticeDecreeNumbers.FirstOrDefault(d => d.Year.ToString() == model.GroupYear).ChangedDecreeNumber; 
                model.DecreeDate = db.PracticeDecreeNumbers.FirstOrDefault(d => d.Year.ToString() == model.GroupYear).ChangedDecreeDate; 
                model.ExecutorName = practiceInfo?.ExecutorName;
                model.ExecutorPhone = practiceInfo?.ExecutorPhone;
                model.ExecutorEmail = practiceInfo?.ExecutorEmail;
                model.ROPInitials = practiceInfo?.ROPInitials;

                ViewBag.CanEdit = User.IsInRole(ItsRoles.PracticeManager);

                return View(model);
            }
        }

        public ActionResult DecreeAjax(int? id)
        {
            var decree = db.PracticeDecrees.FirstOrDefault(d => d.Id == id);

            if (decree == null) // основной приказ не сформирован
                return Json(new
                {
                    data = new List<object>(),
                    total = 0
                },
                new JsonSerializerSettings()
            );

            var semester = db.Semesters.FirstOrDefault(s => s.Id == decree.SemesterID);
            var plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == decree.DisciplineUUID);

            var decrees = db.PracticeChangedDecrees.Include(d => d.MainDecree).Include(d => d.MainDecree.Group).Where(d => d.MainDecreeId == id).ToList()
                .Select(d => new
                {
                    d.Id,
                    Status = d.StatusName,
                    DateExportToSed = d.DateExportToSed?.ToShortDateString(),
                    Number = d.DecreeNumber,
                    d.SerialNumber,
                    d.Comment,
                    SedOp = PracticeOrdersHelper.SedOp(d.Status),
                    d.ExecutorName,
                    d.ExecutorPhone,
                    d.ExecutorEmail,
                    d.ROPInitials
                })
                .ToList();

            return Json(new
            {
                data = decrees,
                total = decrees.Count()
            },
                new JsonSerializerSettings()
            );
        }
        
        private void Add(List<PracticeOrderViewModel> list, PracticeOrderViewModel m)
        {
            m.Description = m.Name;
            list.Add(m);
            return;
        }
        
        public ActionResult CreateOrder(int id, int decreeId, string students, string name, string phone, string email, string initials)
        {
            try
            {
                List<StudentsInfo> studentsInfo = JsonConvert.DeserializeObject<List<StudentsInfo>>(students);

                var decree = db.PracticeDecrees.Include(d => d.Group).FirstOrDefault(d => d.Id == decreeId);
                if (decree == null)
                    return Json(new { success = false, message = $"Приказ не сформирован: Не найден основной приказ" });

                var decreeNumber = db.PracticeDecreeNumbers.FirstOrDefault(n => n.Year == decree.Group.YearHistory);
                if (decreeNumber?.ChangedDecreeNumber == null || decreeNumber?.ChangedDecreeDate == null)
                    return Json(new { success = false, message = $"Приказ не сформирован: Не задан номер или дата Приказа во изменение в справочнике. Обращайтесь в учебный отдел." });

                var newDecree = db.PracticeChangedDecrees.FirstOrDefault(d => d.Id == id);

                if (newDecree == null)
                {
                    newDecree = db.PracticeChangedDecrees.Create();
                    newDecree.MainDecreeId = decreeId;
                    
                    // порядковый номер считается внутри учебного года 
                    var otherDecrees = db.PracticeChangedDecrees.Include(d => d.MainDecree.Group)
                        .Where(d => d.MainDecree.Group.YearHistory == decree.Group.YearHistory)
                        .OrderByDescending(d => d.SerialNumber).ToList();

                    newDecree.SerialNumber = otherDecrees.Count() == 0 ? 1 : otherDecrees.First().SerialNumber + 1;
                    newDecree.DecreeNumber = $"{decreeNumber.ChangedDecreeNumber}-{newDecree.SerialNumber}";
                    newDecree.DecreeDate = decreeNumber.ChangedDecreeDate;
                    newDecree.SedId = null;
                    db.PracticeChangedDecrees.Add(newDecree);
                }
                else
                {
                    switch (newDecree.Status)
                    {
                        case PtraciceDecreeStatus.Sended:
                            return Json(new { success = false, message = $"Приказ уже отправлен в СЭД" });
                        case PtraciceDecreeStatus.Processed:
                            return Json(new { success = false, message = $"Приказ уже в работе" });
                        case PtraciceDecreeStatus.Sign:
                            return Json(new { success = false, message = $"Приказ уже подписан" });
                    }
                }

                newDecree.Status = PtraciceDecreeStatus.Create;
                
                newDecree.Comment = null;
                if (newDecree.FileStorageId != null)
                    DataContext.FileStorageHelper.RemoveFile((int)newDecree.FileStorageId);

                newDecree.ExecutorName = name;
                newDecree.ExecutorPhone = phone;
                newDecree.ExecutorEmail = email;
                newDecree.ROPInitials = initials;

                db.SaveChanges();

                var studentsDecree = db.PracticeChangedDecreeStudents.Where(s => s.ChangedDecreeId == newDecree.Id).ToList();
                db.PracticeChangedDecreeStudents.RemoveRange(studentsDecree);
                db.SaveChanges();

                foreach (var s in studentsInfo)
                {
                    db.PracticeChangedDecreeStudents.Add(new PracticeChangedDecreeStudent()
                    {
                        ChangedDecreeId = newDecree.Id,
                        ReasonId = s.ReasonId,
                        RecoveryDate = s.RecoveryDate,
                        StudentId = s.Id
                    });
                }
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = $"Приказ во изменение сформирован",
                    number = newDecree.DecreeNumber,
                    statusName = newDecree.StatusName,
                    sedOp = PracticeOrdersHelper.SedOp(newDecree.Status),
                    practiceDecreeID = newDecree.Id
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Приказ не сформирован: {ex.Message}" });
            }
        }

        public ActionResult GetOrder(int? id)
        {
            try
            {
                var decree = db.PracticeChangedDecrees.Include(d => d.FileStorage).FirstOrDefault(d => d.Id == id);

                if (decree == null)
                    return Json(new { success = false, message = $"Приказ не сформирован" });

                switch (decree.Status)
                {
                    case PtraciceDecreeStatus.Create:
                    case PtraciceDecreeStatus.ErorrSED:
                    case PtraciceDecreeStatus.Revision:
                        {
                            var model = PracticeOrdersHelper.GetModel(decree);
                            var he = new HostingEnvironment();

                            var fullName = Path.Combine(he.ContentRootPath, @"PracticeChangedOrder.docx");

                            using (var input = System.IO.File.Open(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                var output = new MemoryStream();

                                var engine = new WordDocxTemplateReportingEngine();
                                engine.Build(input, model, output, FileFormat.Docx);

                                output.Position = 0;

                                return File(output, System.Net.Mime.MediaTypeNames.Application.Octet, "Во изменение приказа по практикам.docx");
                            }
                        }
                    case PtraciceDecreeStatus.Sended:
                    case PtraciceDecreeStatus.Processed:
                    case PtraciceDecreeStatus.Sign:
                         if(decree.FileStorageId!= null)
                            return File(DataContext.FileStorageHelper.GetBytes((int)decree.FileStorageId), System.Net.Mime.MediaTypeNames.Application.Octet, decree.FileStorage.FileNameForUser);
                         break;
                }

                return Json(new { success = false, message = $"Приказ не сформирован" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Приказ не сформирован\n{ex.Message}" });
            }

        }
        
        public ActionResult SendToSed(int? id)
        {
            try
            {
                var decree = db.PracticeChangedDecrees.FirstOrDefault(d => d.Id == id);

                if (decree == null)
                    return Json(new { success = false, message = $"Приказ не сформирован" });

                var mainDecree = db.PracticeDecrees.FirstOrDefault(d => d.Id == decree.MainDecreeId);
                if (mainDecree?.SedId == null)
                    return Json(new { success = false, message = $"Основной приказ не отправлен в СЭД" });

                switch (decree.Status)
                {
                    case PtraciceDecreeStatus.Sended:
                    case PtraciceDecreeStatus.Processed:
                    case PtraciceDecreeStatus.Sign:
                        return Json(new { success = false, message = $"Приказ уже отправлен" });
                }

                var model = PracticeOrdersHelper.GetModel(decree);
                var he = new HostingEnvironment();

                var fullName = Path.Combine(he.ContentRootPath, @"PracticeChangedOrder.docx");

                var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                using (var input = System.IO.File.Open(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var output = new MemoryStream();

                    var engine = new WordDocxTemplateReportingEngine();
                    engine.Build(input, model, output, FileFormat.Docx);

                    output.Position = 0;

                    var bytes = output.ToArray();

                    //номер приказа_название практики_название исторической группы без черточки
                    var groupName = model.GroupName.Replace("-", "");
                    var filename = $"{model.Number}_{model.Infos.FirstOrDefault()?.PracticeName}_{groupName}.docx";

                    var document = new SedDocument
                    {
                        base_issue_sedid = mainDecree.SedId,
                        practice_type = model.Infos.FirstOrDefault()?.PracticeName,
                        practice_uuid = model.Infos.FirstOrDefault()?.PrcaticeUUID,
                        group_uuid = model.GroupId,
                        group_name = model.GroupName,
                        term = model.Term,
                        term_name = model.SemesterName,
                        year = model.EduYear,
                        profile_uuid = model.ProfileUUID,
                        profile_name = model.ProfileName,
                        institution_uuid = model.InstututeUUID,
                        institution_name = model.Institute,
                        issue_date = model.ChangedDecreeDate?.Date ?? DateTime.Now.Date,
                        issue_number = model.ChangedDecreeNumber,
                        file_name = filename,
                        file_size = bytes.Length,
                        file = bytes,
                        started_by = User.Identity.Name,
                        supervisors = model.Supervisiors,
                        AdName = user?.AdName,
                        SamAccountName = user?.SamAccountName
                    };

                    var service = new SedRestService();

                    if (decree?.SedId == null)
                    {
                        // если приказ впервые отправляется в СЭД, то записываем его SedId
                        int sedId = service.SendDocument(document, method: "POST", sedId: decree?.SedId);
                        decree.SedId = sedId;
                    }
                    else
                    {
                        // приказ повторно отправляется в СЭД
                        service.SendDocument(document, method: "PUT", sedId: decree?.SedId);
                    }

                    output.Position = 0;
                    int? fileid = DataContext.FileStorageHelper.SaveFile(output, filename, DataContext.FileCategory.Practice, $"{decree.MainDecree.Group.Profile.Direction.okso}_{model.Year}", $"PracticeChangedId {decree.Id} ", decree.FileStorageId);
                    decree.FileStorageId = fileid;

                    decree.Comment = null;
                    decree.Status = PtraciceDecreeStatus.Sended;
                    decree.DateExportToSed = DateTime.Now;

                    db.SaveChanges();

                    var sedOp = PracticeOrdersHelper.SedOp(decree.Status);

                    return Json(new { success = true, message = $"Документ отправлен в СЭД", status = (int)decree.Status, statusName = decree.StatusName, sedOp, DateExportToSed = decree.DateExportToSed?.ToShortDateString() });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Документ не отправлен в СЭД\n{ex.Message}" });
            }
        }

        public ActionResult GetSedStatus(int? id)
        {
            try
            {
                var decree = db.PracticeChangedDecrees.FirstOrDefault(d => d.Id == id);

                if (decree == null)
                    return Json(new { success = false, message = $"Приказ не сформирован" });

                if (decree.SedId == null)
                    return Json(new { success = false, message = $"Приказ в СЭД не отправлялся" });

                var service = new SedRestService();
                var document = service.GetDocument(decree.SedId.Value);

                switch (document.sed_state)
                {
                    case "W":
                        decree.Status = PtraciceDecreeStatus.Processed;
                        decree.Comment = document.sed_lastcomm ?? document.sed_syscomm;
                        break;
                    case "N":
                        decree.Status = PtraciceDecreeStatus.Revision;
                        decree.Comment = document.sed_lastcomm ?? document.sed_syscomm;
                        break;
                    case "D":
                        decree.Status = PtraciceDecreeStatus.Sign;
                        decree.Comment = document.sed_lastcomm ?? document.sed_syscomm;
                        break;
                    case "B":
                        decree.Status = PtraciceDecreeStatus.ErorrSED;
                        decree.Comment = document.sed_syscomm ?? document.sed_lastcomm;
                        break;
                    default:
                        decree.Comment = document.sed_syscomm ?? document.sed_lastcomm;
                        break;
                }

                db.SaveChanges();

                var sedOp = PracticeOrdersHelper.SedOp(decree.Status);

                return Json(new { success = true, message = $"Статус документа обновлен", status = (int)decree.Status, statusName = decree.StatusName, sedOp, comment = decree.Comment ?? "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ошибка при обновлении статуса\n{ex.Message}" });
            }

        }

        public ActionResult Students(int decreeId, string groupId)
        {
            var group = db.GroupsHistories.FirstOrDefault(g => g.Id == groupId);
            var studentsDecree = db.PracticeChangedDecreeStudents.Where(s => s.ChangedDecreeId == decreeId).ToList();
            var students = db.Students
                    .Include(s => s.Person)
                    .Where(s => s.GroupId == group.GroupId
                        && (s.Status == "Активный" || s.Status == "Отп.с.посещ." || s.Status == "Отп.дород.послерод.")
                        ).ToList()
                    .Select(s => new
                    {
                        Id = s.Id,
                        Name = s.Person.FullName(),
                        Status = s.Status,
                        IsChecked = studentsDecree.FirstOrDefault(st => st.StudentId == s.Id) != null,
                        RecoveryDate = studentsDecree.FirstOrDefault(st => st.StudentId == s.Id)?.RecoveryDate?.ToShortDateString() ?? "",
                        Reason = studentsDecree.FirstOrDefault(st => st.StudentId == s.Id)?.Reason.Reason,
                    }).OrderBy(s => s.Name);

            return Json(
                new
                {
                    data = students,
                    total = students.Count()
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Reasons()
        {
            var reasons = db.PracticeChangedDecreeReasons.ToList();

            return Json(
                new
                {
                    data = reasons,
                    total = reasons.Count()
                },
                new JsonSerializerSettings()
            );
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

    }    
}