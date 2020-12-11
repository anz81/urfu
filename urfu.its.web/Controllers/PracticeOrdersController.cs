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
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting.Internal;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.PracticeView)]
    public class PracticeOrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PracticeOrders
        public ActionResult Index(string userId, string sort, string filter)
        {
            var years = db.GroupsHistories.Select(g => new
            {
                Year = g.YearHistory
            })
                .Distinct()
                .OrderBy(y => y.Year)
                .ToList();

            var divisions = db.InstitutesForUser(User)
                .OrderBy(d => d.shortTitle).Select(d => new
                {
                    Id = d.uuid,
                    Name = d.shortTitle
                })
                .ToList();

            //var directions = db.Directions.Select(d => new
            //{
            //    Id = d.uid,
            //    Name = d.okso + " " + d.title + " ("+ d.standard +")" 
            //})
            //    .OrderBy(d => d.Name)
            //    .ToList();

            ViewBag.Years = JsonConvert.SerializeObject(years);
            ViewBag.Semesters = JsonConvert.SerializeObject(db.Semesters);
            ViewBag.Divisions = JsonConvert.SerializeObject(divisions);
            //ViewBag.Directions = JsonConvert.SerializeObject(directions);
            ViewBag.FamilirizationTypes = JsonConvert.SerializeObject(db.FamilirizationTypes);
            ViewBag.Qualifications = JsonConvert.SerializeObject(db.Qualifications);

            var statuses = new List<object>()
            {
                new { Value = "", Name = "Все" }
            };
            statuses.AddRange(PracticeDecree.DecreeStatusNames.Select(s => new { Value = s.Value, Name = s.Value }));
            ViewBag.Statuses = JsonConvert.SerializeObject(statuses);

            ViewBag.CanEdit = User.IsInRole(ItsRoles.PracticeManager);

            return View();
        }

        private JsonResult EmtyResult()
        {
            return Json(new
            {
                data = new object[0],
                total = 0
            },
               new JsonSerializerSettings()
           );
        }

        public ActionResult GetGroups(string userId, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            Debug.WriteLine($"GetGroups -> ajax {isAjax}, sort={sort}, userID={userId}, focus={focus}, filter={filter}");

            if (!isAjax)
                return null;

            if (filter == null)
            {
                //ilter = "[{'property':'Year', 'value': 2017}]";
                return EmtyResult();
            }

            ((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 1200000;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var plans = db.Plans.Where(p => PracticeOrdersHelper.PlanPracticeTypes.Contains(p.additionalType)); // && !p.remove);
            var students = db.Students.ActiveOrGraduated();

            var divisionCache = db.Divisions.ToList();
            var divisions = db.InstitutesForUser(User);

            var divisionUUIDs = new List<string>() { divisions.OrderBy(d => d.shortTitle).First().uuid };
            var filterGroups = new List<string>();
            var filterDirectionUIDs = new List<string>();
            string filterPracticeName = "";
            bool? filterOldVersion = null;
            string filterStatus = "";

            //var years = db.GroupsHistories.Select(g => new { Year = g.YearHistory }).Distinct().OrderBy(y => y.Year).ToList();
            //var year = years.Last().Year;
            var year = 0;
            var semesterId = 0;

            ObjectableFilterRules rules = ObjectableFilterRules.Deserialize(filter);

            //var rules = JsonConvert.DeserializeObject<List<PracticeListFulterRule>>(filter);

            foreach (var rule in rules)
            {
                if (rule.Value != null)
                {
                    switch (rule.Property)
                    {
                        case "Year":
                            year = Int32.Parse(rule.Value.ToString());
                            break;
                        case "Semester":
                            semesterId = Int32.Parse(rule.Value.ToString());
                            break;
                        case "Division":
                            divisionUUIDs = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                            break;
                        case "Direction":
                            var directionUIDs = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                            if (directionUIDs.Count() != 0 && directionUIDs.First() != "")
                            {
                                filterDirectionUIDs = directionUIDs;
                                plans = plans.Where(p => directionUIDs.Contains(p.directionId));
                            }
                            break;
                        case "Group":
                            var groupIds = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                            if (groupIds?.Count() != 0 && groupIds?.First() != "")
                                filterGroups = groupIds;
                            break;
                        case "FamilirizationType":
                            var familirizationType = rule.Value.ToString();
                            if (!string.IsNullOrWhiteSpace(familirizationType))
                                plans = plans.Where(p => p.familirizationType == familirizationType);
                            break;
                        case "Qualification":
                            var qualification = rule.Value.ToString();
                            if (!string.IsNullOrWhiteSpace(qualification))
                                plans = plans.Where(p => p.qualification == qualification);
                            break;
                        case "PracticeName":
                            filterPracticeName = rule.Value?.ToString() ?? "";
                            plans = plans.Where(p => p.disciplineTitle.Contains(filterPracticeName));
                            break;
                        case "IsOldVersion":
                            bool isOldVersion;
                            var isBool = bool.TryParse(rule.Value?.ToString(), out isOldVersion);
                            if (isBool)
                                filterOldVersion = isOldVersion;
                            break;
                        case "Status":
                            filterStatus = rule.Value.ToString();
                            break;
                    }
                }
            }

            //students = students.Where(s=>divisionUUIDs.Contains(s.Group.ManagingDivisionId))
            //.Where(h => divisionUUIDs.Contains(h.Group.ManagingDivisionId) || divisionUUIDs.Contains(h.Group.ManagingDivisionParentId))


            var groupPlan = students.Join(plans,
                 s => new { N = s.planVerion, V = s.versionNumber },
                 p => new { N = p.eduplanNumber, V = (int?)p.versionNumber },
                 (s, p) => new
                 {
                     s.GroupId,
                     //s.Group.ManagingDivisionId,
                     //s.Group.ManagingDivisionParentId,
                     s.Group.FormativeDivisionId,
                     s.Group.FormativeDivisionParentId,
                     p,
                 })
                 .Distinct()
                 .Where(j => divisionUUIDs.Contains(j.FormativeDivisionId) || divisionUUIDs.Contains(j.FormativeDivisionParentId))
                 .Join(db.Directions, j => j.p.directionId, d => d.uid, (j, d) => new
                 {
                     j.p,
                     j.GroupId,
                     d,
                     //t = db.PlanDisciplineTerms.Where(pt=>pt.DisciplineUUID == j.p.disciplineUUID).ToList()
                 })
                 .ToList();

            //var r1 = groupPlan.ToList();

            //var res = groupPlan.Select(a =>
            //new
            //{
            //    a.GroupId,
            //    a.p,
            //Direction = db.Directions.FirstOrDefault(d => d.uid == a.p.directionId),
            //GroupsHistory = db.GroupsHistories.Where(h => h.GroupId == a.Group.Id).ToList(),
            //PlanTmers = db.PlanTerms.Where(pt => pt.eduplanUUID == a.p.eduplanUUID).ToList(),
            //Decrees = db.PracticeDecrees.Where(d=>d.Group.GroupId == a.Group.Id).ToList(),
            //});

            //var list = res.ToList();
            var list = groupPlan;

            var practiceList = new List<PracticeOrderViewModel>();


            //var planTerms = db.PlanTerms.Join(plans, t => t.eduplanUUID, p => p.eduplanUUID, (t, p) => t).ToList();
            //var planTerms = db.PlanTerms.ToList();
            var planTerms = db.PlanDisciplineTerms.ToList();

            var groupsHistory = db.GroupsHistories
                .Include(g => g.Group)
                .Where(h => divisionUUIDs.Contains(h.Group.FormativeDivisionId) || divisionUUIDs.Contains(h.Group.FormativeDivisionParentId))
                .ToList();

            var decrees = db.PracticeDecrees.ToList().Where(d => d.StatusName.Contains(filterStatus));

            var semesters = db.Semesters.ToList();

            var practices = db.Practices.Include("Group.Group").Where(p =>
                    p.Year == year
                    && p.SemesterId == semesterId
                    && (filterGroups.Count == 0 || filterGroups.Contains(p.GroupHistoryId))
                    && (divisionUUIDs.Contains(p.Group.Group.FormativeDivisionId) || divisionUUIDs.Contains(p.Group.Group.FormativeDivisionParentId))
                    && (filterDirectionUIDs.Count == 0 || filterDirectionUIDs.Contains(p.Group.Profile.DIRECTION_ID))
                    && !p.remove)
                    .ToList();

            Debug.WriteLine($"SQL duration {stopwatch.ElapsedMilliseconds}");

            foreach (var r in list)
            {
                //надо понять курс и семестр для практики
                var terms = planTerms.Where(pt => pt.DisciplineUUID == r.p.disciplineUUID);

                foreach (var t in terms)
                {
                    if (year == 0) continue;
                    var group = groupsHistory.FirstOrDefault(g => g.GroupId == r.GroupId && g.Course == t.Course && g.YearHistory == year);
                    if (group == null) continue;
                    if (filterGroups.Count > 0 && !filterGroups.Contains(group.Id)) continue;

                    if (semesterId != 0 && t.SemesterID != semesterId) continue;
                    var decree = decrees.FirstOrDefault(d => d.DisciplineUUID == r.p.disciplineUUID && d.GroupId == group.Id && d.Term == t.Term);
                    Semester semester = semesters.FirstOrDefault(s => s.Id == t.SemesterID);

                    var practiceOrderViewModel = GetPracticeOrderViewModel(divisionCache, r.p, decree, group, semester, t.Term, isOldVersion: false);
                    Add(practiceList, practiceOrderViewModel);

                    practices.RemoveAll(p => p.DisciplineUUID == r.p.disciplineUUID && p.GroupHistoryId == group.Id);
                }
            }

            for (int j = 0; j < practices.Count(); j++)
            {
                var practice = practices[j];

                var plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == practice.DisciplineUUID); // && !p.remove);
                if (plan == null) continue;
                if (!string.IsNullOrEmpty(filterPracticeName) && !plan.disciplineTitle.Contains(filterPracticeName)) continue;

                var term = planTerms.FirstOrDefault(pt => pt.DisciplineUUID == plan.disciplineUUID && pt.Course == practice.Group.Course && pt.SemesterID == semesterId);
                var decree = decrees.FirstOrDefault(d => d.DisciplineUUID == plan.disciplineUUID && d.GroupId == practice.Group.Id && d.Term == term.Term);

                var practiceOrderViewModel = GetPracticeOrderViewModel(divisionCache, plan, decree, practice.Group, practice.Semester, term.Term, 
                                                                                isOldVersion: !StudentsExtension.GraduatedStatuses.Contains(practice.Student.Status));
                Add(practiceList, practiceOrderViewModel);

                practices.RemoveAll(p => p.DisciplineUUID == plan.disciplineUUID && p.GroupHistoryId == practice.GroupHistoryId);
                j = -1;
            }

            if (filterOldVersion != null)
            {
                practiceList = practiceList.Where(p => p.IsOldPlanVersion == filterOldVersion).ToList();
            }

            if (!string.IsNullOrEmpty(filterStatus))
            {
                practiceList = practiceList.Where(p => p.Status != null).ToList();
            }

            var result = practiceList.OrderBy(r => r.Group);

            stopwatch.Stop();

            Debug.WriteLine($"GetGroups duration {stopwatch.ElapsedMilliseconds}");

            return Json(new
            {
                data = result,
                total = result.Count()
            },
                new JsonSerializerSettings()
            );
        }

        private PracticeOrderViewModel GetPracticeOrderViewModel(List<Division> divisionCache, Plan plan, PracticeDecree decree,
            GroupsHistory groupHistory, Semester semester, int term, bool isOldVersion)
        {
            var department = divisionCache.FirstOrDefault(d => d.uuid == groupHistory.Group.FormativeDivisionId);
            var departmentParent = divisionCache.FirstOrDefault(d => d.uuid == groupHistory.Group.FormativeDivisionParentId);
            var departmentTitle = "";
            var institute = "";
            var instituteId = "";
            if (department != null)
            {
                departmentTitle = department.typeTitle == "Департамент" ? department.shortTitle : "";
                institute = department.typeTitle == "Институт" ? department.shortTitle :
                        (departmentParent.typeTitle == "Институт" ? departmentParent.shortTitle : "");
                instituteId = department.typeTitle == "Институт" ? department.uuid :
                        (departmentParent.typeTitle == "Институт" ? departmentParent.uuid : "");
            }

            var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == plan.disciplineUUID && p.GroupId == groupHistory.Id && p.SemesterId == semester.Id);

            var practiceOrderViewModel = new PracticeOrderViewModel
            {
                Group = groupHistory.Name,
                DisciplineTitle = plan.disciplineTitle,
                DisciplineUID = plan.disciplineUUID,
                Term = term,
                SemesterID = semester.Id,
                PracticeDecreeID = decree?.Id,
                PracticeType = plan.additionalType,
                Status = decree?.StatusName,
                StatusId = decree?.Status == null ? 0 : (int)decree.Status,
                DateExportToSed = decree?.DateExportToSed?.ToShortDateString(),
                OrderNumber = decree?.DecreeNumber,
                GroupYear = groupHistory.YearHistory.ToString(),
                Semester = semester.Name,
                GroupId = groupHistory.Id,
                Comment = decree?.Comment,
                SedOp = PracticeOrdersHelper.SedOp(decree?.Status ?? 0),
                SedId = decree?.SedId,
                InstituteId = instituteId,
                DirectionId = groupHistory.Profile.Direction.uid,
                ExecutorName = practiceInfo?.ExecutorName,
                ExecutorPhone = practiceInfo?.ExecutorPhone,
                ExecutorEmail = practiceInfo?.ExecutorEmail,
                ROPInitials = practiceInfo?.ROPInitials,
                IsOldPlanVersion = isOldVersion,
                IsRemovedDiscipline = plan.remove,
                PlanNumber = plan.eduplanNumber.Value,
                PlanVersion = plan.versionNumber,
                PlanStatus = plan.versionStatus,
                GroupUuid = groupHistory.GroupId
            };

            return practiceOrderViewModel;
        }


        private void Add(List<PracticeOrderViewModel> list, PracticeOrderViewModel m)
        {
            m.Description = m.Name;
            list.Add(m);
            return;

            //var practice = list.FirstOrDefault(t => t.GroupId == m.GroupId);

            //if (practice == null)
            //{
            //    m.Description = m.Name;
            //    list.Add(m);
            //    return;
            //}

            //practice.Description += $", {m.Name}";
        }

        public ActionResult OksoList(string institute)
        {
            string[] instituteIds = institute.Split(',');
            if (instituteIds.Length > 0 && !instituteIds.Contains(""))
            {
                var directions = new List<Direction>();
                foreach (var instituteId in instituteIds)
                {
                    var chairs = db.ChairsForDivision(instituteId).Select(d => d.uuid).ToList();
                    directions.AddRange(db.Profiles.Where(p => !p.remove && chairs.Contains(p.CHAIR_ID)).Select(p => p.Direction).ToList());
                }

                var okso = directions.Distinct().Select(p => new
                {
                    Id = p.uid,
                    Name = p.okso + " " + p.title + " (" + p.standard + ")"
                }).OrderBy(d => d.Name).ToList();

                return Json(
                    new
                    {
                        data = okso
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                return Json(
                    new
                    {
                        data = new List<Direction>()
                    },
                    new JsonSerializerSettings()
                );
            }
        }

        public ActionResult GroupList(int year, string institute, string okso)
        {
            string[] institutes = institute.Split(',');
            string[] directions = okso?.Split(',') ?? new string[0];

            if (institutes.Length > 0 && !institutes.Contains(""))
            {
                var divisions = db.Divisions.Where(d => institutes.Contains(d.uuid) || institutes.Contains(d.parent)).Select(d => d.uuid).ToList();

                var groups = db.GroupsHistories.Include(g => g.Group).Where(g => g.YearHistory == year && divisions.Contains(g.Group.FormativeDivisionId));

                if (directions.Length > 0 && !directions.Contains(""))
                {
                    groups = groups.Where(g => directions.Contains(g.Profile.DIRECTION_ID));
                }

                var result = groups.Select(g => new
                {
                    Id = g.Id,
                    Name = g.Name
                }).OrderBy(g => g.Name).ToList();

                return Json(
                    new
                    {
                        data = result.ToList()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                return Json(
                    new
                    {
                        data = new List<GroupsHistory>()
                    },
                    new JsonSerializerSettings()
                );
            }
        }
        
        public ActionResult EditExecutorInfo(string disciplineUUID, string groupId, int semesterId, string name, string phone, string email, string ropInitials)
        {
            bool isNewPracticeInfo = false;
            var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == disciplineUUID && p.GroupId == groupId && p.SemesterId == semesterId);
            if (practiceInfo == null)
            {
                isNewPracticeInfo = true;
                practiceInfo = new PracticeInfo()
                {
                    DisciplineUUID = disciplineUUID,
                    GroupId = groupId,
                    SemesterId = semesterId
                };
            }

            practiceInfo.ExecutorName = name;
            practiceInfo.ExecutorPhone = phone;
            practiceInfo.ExecutorEmail = email;
            practiceInfo.ROPInitials = ropInitials;

            if (isNewPracticeInfo)
                db.PracticeInfo.Add(practiceInfo);
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }


        public ActionResult CopyExecutorInfo(List<PracticeOrderViewModel> decrees)
        {
           if(decrees.Count ==0 || decrees.Count ==1)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var soursedecree = decrees[0];
            var sourseexecutorinfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == soursedecree.DisciplineUID
            && p.GroupId == soursedecree.GroupId && p.SemesterId == soursedecree.SemesterID);
            if (sourseexecutorinfo.ExecutorName == null)
            {
                return Json(new { succsess = false, message = "Информация об исполнителе не задана " });
            }

            decrees.RemoveAt(0);
            bool isNewPracticeInfo;

            foreach (PracticeOrderViewModel order in decrees)
            {

                var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == order.DisciplineUID && p.GroupId == order.GroupId && p.SemesterId == order.SemesterID);

                isNewPracticeInfo = false;
                if (practiceInfo == null)
                {
                    isNewPracticeInfo = true;
                    practiceInfo = new PracticeInfo()
                    {
                        DisciplineUUID = order.DisciplineUID,
                        GroupId = order.GroupId,
                        SemesterId = order.SemesterID
                    };
                }
                practiceInfo.ExecutorName = sourseexecutorinfo.ExecutorName;
                practiceInfo.ExecutorPhone = sourseexecutorinfo.ExecutorPhone;
                practiceInfo.ExecutorEmail = sourseexecutorinfo.ExecutorEmail;
                practiceInfo.ROPInitials = sourseexecutorinfo.ROPInitials;

                if (isNewPracticeInfo)
                    db.PracticeInfo.Add(practiceInfo);
                db.SaveChanges();
            }

            return new StatusCodeResult(StatusCodes.Status200OK);
        }


        public ActionResult CreateOrder(string groupId, string disciplineUID, int semesterID, int term, string number)
        {
            try
            {
                var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == disciplineUID && p.GroupId == groupId && p.SemesterId == semesterID);
                if (practiceInfo?.ExecutorName == null && practiceInfo?.ExecutorPhone == null)
                    return Json(new { success = false, message = "Приказ не сформирован. Заполните информацию об исполнителе приказа." });


                var group = db.GroupsHistories.FirstOrDefault(g => g.Id == groupId);
                if (group == null)
                    return Json(new { success = false, message = $"Приказ не сформирован: Не найдена группа" });

                var decreeNumber = db.PracticeDecreeNumbers.FirstOrDefault(n => n.Year == group.YearHistory);
                if (decreeNumber == null)
                    return Json(new { success = false, message = $"Приказ не сформирован: Не задан номер приказа" });

                var decree = db.PracticeDecrees.FirstOrDefault(d => d.GroupId == groupId && d.SemesterID == semesterID && d.Term == term && d.DisciplineUUID == disciplineUID);
                if (decree == null)
                {
                    decree = db.PracticeDecrees.Create();
                    decree.GroupId = groupId;
                    decree.DisciplineUUID = disciplineUID;
                    decree.SemesterID = semesterID;
                    decree.Term = term;
                    decree.SedId = null;
                    db.PracticeDecrees.Add(decree);
                }
                else
                {
                    switch (decree.Status)
                    {
                        case PtraciceDecreeStatus.Sended:
                            return Json(new { success = false, message = $"Приказ уже отправлен в СЭД" });
                        case PtraciceDecreeStatus.Processed:
                            return Json(new { success = false, message = $"Приказ уже в работе" });
                        case PtraciceDecreeStatus.Sign:
                            return Json(new { success = false, message = $"Приказ уже подписан" });
                    }
                }

                decree.Status = PtraciceDecreeStatus.Create;

                decree.DecreeNumber = decreeNumber.Number;
                decree.DecreeDate = decreeNumber.DecreeDate;
                decree.Comment = null;
                if (decree.FileStorageId != null)
                    Urfu.Its.Web.DataContext.FileStorageHelper.RemoveFile((int)decree.FileStorageId);

                db.SaveChanges();

                return Json(new { success = true, message = $"Приказ сформирован", number = decreeNumber.Number, statusName = decree.StatusName,
                    sedOp = PracticeOrdersHelper.SedOp(decree.Status), practiceDecreeID = decree.Id });
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
                var decree = db.PracticeDecrees.Include(d=>d.FileStorage).FirstOrDefault(d => d.Id == id);

                if (decree == null)
                    return Json(new { success = false, message = $"Приказ не сформирован" });

                switch (decree.Status)
                {
                    case PtraciceDecreeStatus.Create:
                    case PtraciceDecreeStatus.ErorrSED:
                    case PtraciceDecreeStatus.Revision:
                        {

                        var model = PracticeOrdersHelper.GetModel(decree);

                            HostingEnvironment hi = new HostingEnvironment();
                            var fullName = Path.Combine(hi.ContentRootPath, @"PracticeOrder.docx");

                            using (var input = System.IO.File.Open(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                var output = new MemoryStream();

                                var engine = new WordDocxTemplateReportingEngine();
                                engine.Build(input, model, output, FileFormat.Docx);

                                engine.SetKeepNextParagraph(output,"Итого:");
                                output.Position = 0;

                                return File(output, System.Net.Mime.MediaTypeNames.Application.Octet, "Приказ по практикам.docx");
                            }
                        }
                    case PtraciceDecreeStatus.Sended:
                    case PtraciceDecreeStatus.Processed:
                    case PtraciceDecreeStatus.Sign:
                        if (decree.FileStorageId != null)
                            return File(DataContext.FileStorageHelper.GetBytes((int)decree.FileStorageId),System.Net.Mime.MediaTypeNames.Application.Octet, decree.FileStorage.FileNameForUser);
                       break;
                }

                return Json(new { success = false, message = $"Приказ не сформирован" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Приказ не сформирован\n{ex.Message}" });
            }
        }

        public ActionResult GetOrderPdf(int? id)
        {
            try
            {
                var decree = db.PracticeDecrees.FirstOrDefault(d => d.Id == id);

                if (decree == null)
                    return Json(new { success = false, message = $"Приказ не сформирован" });

                var model = PracticeOrdersHelper.GetModel(decree);
                var hi = new HostingEnvironment();
                var fullName = System.IO.Path.Combine(hi.ContentRootPath, @"PracticeOrder.docx");

                using (var input = System.IO.File.Open(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var output = new MemoryStream();

                    var engine = new WordDocxTemplateReportingEngine();
                    engine.Build(input, model, output, FileFormat.Pdf);
                    engine.SetKeepNextParagraph(output, "Итого:");

                    output.Position = 0;

                    return File(output, System.Net.Mime.MediaTypeNames.Application.Octet, "Приказ по практикам.pdf");
                    //return File(output, System.Net.Mime.MediaTypeNames.Application.Octet, "Приказ по практикам.pdf");
                }
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
                var decree = db.PracticeDecrees.FirstOrDefault(d => d.Id == id);

                if (decree == null)
                    return Json(new { success = false, message = $"Приказ не сформирован" });

                switch (decree.Status)
                {
                    case PtraciceDecreeStatus.Sended:
                    case PtraciceDecreeStatus.Processed:
                    case PtraciceDecreeStatus.Sign:
                        return Json(new { success = false, message = $"Приказ уже отправлен" });
                }

                var model = PracticeOrdersHelper.GetModel(decree);
                var hi = new HostingEnvironment();
                var fullName = Path.Combine(hi.ContentRootPath, @"PracticeOrder.docx");

                var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                using (var input = System.IO.File.Open(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var output = new MemoryStream();

                    var engine = new WordDocxTemplateReportingEngine();
                    engine.Build(input, model, output, FileFormat.Docx);
                    engine.SetKeepNextParagraph(output, "Итого:");

                    output.Position = 0;

                    var bytes = output.ToArray();

                    //номер приказа_название практики_название исторической группы без черточки
                    var groupName = model.GroupName.Replace("-", "");
                    var filename = $"{model.Number}_{model.Infos.FirstOrDefault()?.PracticeName}_{groupName}.docx";

                    var document = new SedDocument
                    {
                        base_issue_sedid = null,
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
                        //issue_date = model.Date?.ToShortDateString(),
                        issue_date = model.Date?.Date ?? DateTime.Now.Date,
                        issue_number = model.Number,
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
                    int? fileid = DataContext.FileStorageHelper.SaveFile(output, filename, DataContext.FileCategory.Practice, $"{decree.Group.Profile.Direction.okso}_{model.Year}", $"PracticeDecreeId {decree.Id} ", decree.FileStorageId);
                        decree.FileStorageId = fileid;
                    
                    decree.Comment = null;
                    decree.Status = PtraciceDecreeStatus.Sended;
                    decree.DateExportToSed = DateTime.Now;

                    db.SaveChanges();

                    var sedOp = PracticeOrdersHelper.SedOp(decree.Status);

                    return Json(new { success = true, message = $"Документ отправлен в СЭД", status = (int)decree.Status, statusName = decree.StatusName, sedOp, DateExportToSed = decree.DateExportToSed?.ToShortDateString()});
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
                var decree = db.PracticeDecrees.FirstOrDefault(d => d.Id == id);

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
        

        public ActionResult Numbers()
        {
            ViewBag.CanEdit = User.IsInRole(ItsRoles.PracticeManager);
            return View();
        }

        public ActionResult GetNumbers()
        {
            var r1 = db.PracticeDecreeNumbers.Select(d => new {
                d.Id,
                d.Year,
                d.Number,
                d.DecreeDate,
                ChangeDecreeNumber = d.ChangedDecreeNumber,
                ChangeDecreeDate = d.ChangedDecreeDate
            }).OrderBy(d => d.Year).ToList();
            var r2 = r1.Select(r => new {
                r.Id,
                r.Year,
                r.Number,
                DecreeDate = r.DecreeDate?.ToShortDateString(),
                r.ChangeDecreeNumber,
                ChangeDecreeDate = r.ChangeDecreeDate?.ToShortDateString()
            }).ToList();
            return Json(r2, new JsonSerializerSettings());
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult EditNumber(int id, int year, string number, DateTime? decreeDate, string changeDecreeNumber, DateTime? changeDecreeDate)
        {
            var existYear = db.PracticeDecreeNumbers.FirstOrDefault(n => n.Year == year && n.Id != id);
            if (existYear != null)
                return Json(new { success = false, message = "Номер прикза для этого года уже задан" });//, "text/html", Encoding.Unicode);

            if (id == 0)
            {
                db.PracticeDecreeNumbers.Add(new PracticeDecreeNumber {
                    Year = year,
                    Number = number,
                    DecreeDate = decreeDate,
                    ChangedDecreeNumber = changeDecreeNumber,
                    ChangedDecreeDate = changeDecreeDate
                });
                db.SaveChanges();
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }

            var data = db.PracticeDecreeNumbers.FirstOrDefault(n => n.Id == id);
            if (data == null)
            {
                return Json(new { success = false, message = "Редактируемый номер не найден" });//, "text/html", Encoding.Unicode);
            }

            data.Year = year;
            data.Number = number;
            data.DecreeDate = decreeDate;
            data.ChangedDecreeNumber = changeDecreeNumber;
            data.ChangedDecreeDate = changeDecreeDate;

            db.SaveChanges();

            return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveNumber(int id)
        {
            var data = db.PracticeDecreeNumbers.FirstOrDefault(n => n.Id == id);
            if (data == null)
            {
                return Json(new { success = false, message = "Удаляемый номер не найден" });//, "text/html", Encoding.Unicode);
            }

            db.PracticeDecreeNumbers.Remove(data);
            db.SaveChanges();
            return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
        }

        public ActionResult CheckAdmissions(string groupuuid, string disciplineId,int year, int semester)
        {
            List<string> studentslist = new List<string>();

            var students = db.Students
                .Include(s => s.Person)
                .Include(s => s.Practices)
                .Where(s => s.GroupId == groupuuid
                            && StudentsExtension.ActiveStatuses.Any(sx => sx == s.Status))
                .Select(s => new
                {
                    Student = s,

                    Practice = s.Practices
                        .FirstOrDefault(p => p.DisciplineUUID == disciplineId && p.Group.GroupId == groupuuid && p.SemesterId == semester && p.Year == year)
                })
                .ToList();

            students.ForEach(s =>
                {
                   if(s.Practice== null)
                       studentslist.Add(s.Student.Person.Surname + ' ' + s.Student.Person.Name + ' ' + s.Student.Person.PatronymicName);
                   else
                    {
                       var admission = s.Practice.Admissions.LastOrDefault(a => a.Status == AdmissionStatus.Admitted); 
                       var companyadmission = s.Practice.AdmissionCompanys.LastOrDefault(a => a.Status == AdmissionStatus.Admitted);
                        if (admission == null && companyadmission == null)
                            studentslist.Add(s.Student.Person.Surname + ' ' + s.Student.Person.Name + ' ' + s.Student.Person.PatronymicName);
                    }
                    
                });
            return Json(new {studentslist});
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

    }

}