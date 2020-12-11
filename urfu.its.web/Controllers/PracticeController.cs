using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Urfu.Its.Web.Model.Models.Practice;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Ajax.Utilities;
using System.Diagnostics;
using System.Text;
using Urfu.Its.Common;
using System.Net;
using Urfu.Its.Web.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
//using System.Web.Script.Serialization;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.PracticeView)]
    public class PracticeController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string userId, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                return PracticAjax(userId, sort, filter, focus);
            }
            else
            {
                ViewBag.Semesters = JsonConvert.SerializeObject(db.Semesters);

                var divisions = db.InstitutesForUser(User).OrderBy(d => d.shortTitle)
                    .Select(d => new
                    {
                        Id = d.uuid,
                        Name = d.shortTitle + " (" + d.title + ")",
                        d.shortTitle
                    }).ToList();

                ViewBag.Divisions = JsonConvert.SerializeObject(divisions);

                var years = db.GroupsHistories.Select(g => new { Year = g.YearHistory }).Distinct().OrderBy(y => y.Year).ToList();

                ViewBag.Years = JsonConvert.SerializeObject(years);

                var ways = db.PracticeWays.Select(w => new
                {
                    WayId = w.Id,
                    WayDescription = w.Description
                });
                ViewBag.Ways = JsonConvert.SerializeObject(ways);

                var time = db.PracticeTimes.Select(t => new
                {
                    TimeId = t.Id,
                    TimeDescription = t.Description
                });
                ViewBag.Time = JsonConvert.SerializeObject(time);
                
                ViewBag.Focus = focus;
                ViewBag.CanEdit = User.IsInRole(ItsRoles.PracticeManager);

                return View();
            }
        }

        public ActionResult PracticAjax(string userId, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                Debug.WriteLine($"GetGroups -> ajax {isAjax}, sort={sort}, userID={userId}, focus={focus}, filter={filter}");
                var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
                optionsBuilder.UseSqlServer(
                    @"Server=.\;Database=db;Trusted_Connection=True;",
                    opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
                    );
                //((IObjectContextAdapter)db.ObjectContext.CommandTimeout = 1200000;

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var divisionCache = db.Divisions.ToList();
                var divisions = db.InstitutesForUser(User).OrderBy(d => d.shortTitle).ToList();

                var filterDivisionUUIDs = new List<string>() { divisions.First().uuid }; //divisions.Select(d => d.uuid);

                var filterPlans = db.Plans.Where(p =>
                    (p.additionalType == "Учебная практика" || p.additionalType == "Производственная практика"));
                    //&& !p.remove);
                int filterYear = 0;
                int filterSemesterId = -1;
                var filterGroups = new List<string>();
                List<string> filterDirectionUIDs = new List<string>();
                string filterPracticeName = "";
                bool? filterOldVersion = null;

                bool isEmptyFilter = true;
                if (filter != null)
                {
                    ObjectableFilterRules rules = ObjectableFilterRules.Deserialize(filter);

                    //var rules = JsonConvert.DeserializeObject<List<PracticeListFulterRule>>(filter);

                    foreach (var rule in rules)
                    {
                        if (rule.Value != null)
                        {
                            switch (rule.Property)
                            {
                                case "Year":
                                    if (Int32.TryParse(rule.Value.ToString(), out filterYear))
                                        isEmptyFilter = false;
                                    break;
                                case "Semester":
                                    if (Int32.TryParse(rule.Value.ToString(), out filterSemesterId))
                                        isEmptyFilter = false;
                                    break;
                                case "Institute":
                                    var _divisionUUIDs = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                                    if (_divisionUUIDs?.Count() != 0 && _divisionUUIDs?.First() != "")
                                    {
                                        isEmptyFilter = false;
                                        filterDivisionUUIDs = _divisionUUIDs;
                                    }
                                    break;
                                case "Direction":
                                    var directionUIDs = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                                    if (directionUIDs?.Count() != 0 && directionUIDs?.First() != "")
                                    {
                                        isEmptyFilter = false;
                                        filterDirectionUIDs = directionUIDs;
                                        filterPlans = filterPlans.Where(p => filterDirectionUIDs.Contains(p.directionId));
                                    }
                                    break;
                                case "Group":
                                    var groupIds = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                                    if (groupIds?.Count() != 0 && groupIds?.First() != "")
                                    {
                                        isEmptyFilter = false;
                                        filterGroups = groupIds;
                                    }
                                    break;
                                case "PracticeName":
                                    filterPracticeName = rule.Value?.ToString() ?? "";
                                    filterPlans = filterPlans.Where(p => p.disciplineTitle.Contains(filterPracticeName)  );
                                    break;
                                case "IsOldVersion":
                                    bool isOldVersion;
                                    var isBool = bool.TryParse(rule.Value?.ToString(), out isOldVersion);
                                    if (isBool)
                                        filterOldVersion = isOldVersion;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                if (filter == null || isEmptyFilter || filterYear == 0)
                {
                    return Json(
                        new
                        {
                            data = new object[0],
                            total = 0
                        },
                            new JsonSerializerSettings()
                    );
                }

                var students = db.Students.ActiveOrGraduated().Where(s => filterDirectionUIDs.Count == 0 || filterDirectionUIDs.Contains(s.Group.Profile.DIRECTION_ID));

                var direction = db.Directions.Where(d => filterDirectionUIDs.Count ==0 || filterDirectionUIDs.Contains(d.uid));

                var groupPlans = students.Join(filterPlans,
                     s => new { N = s.planVerion, V = s.versionNumber },
                     p => new { N = p.eduplanNumber, V = (int?)p.versionNumber },
                     (s, p) => new
                     {
                         s.GroupId,
                         //s.Group.ManagingDivisionId,
                         //s.Group.ManagingDivisionParentId,
                         s.Group.FormativeDivisionId,
                         s.Group.FormativeDivisionParentId,
                         p
                     })
                     .Distinct()
                     .Where(j => filterDivisionUUIDs.Contains(j.FormativeDivisionId) || filterDivisionUUIDs.Contains(j.FormativeDivisionParentId))
                     .Join(direction, j => j.p.directionId, d => d.uid, (j, d) => new
                     {
                         j.p,
                         j.GroupId,
                         d,
                         //t = db.PlanDisciplineTerms.Where(pt=>pt.DisciplineUUID == j.p.disciplineUUID).ToList()
                     })
                     .ToList();

                var practiceList = new List<PracticeListViewModel>();

                var planTerms = db.PlanDisciplineTerms.ToList();

                var groupsHistory = db.GroupsHistories
                    .Include(g => g.Group)
                    .Where(h => filterDivisionUUIDs.Contains(h.Group.FormativeDivisionId) || filterDivisionUUIDs.Contains(h.Group.FormativeDivisionParentId))
                    .ToList();

                var semesters = db.Semesters.ToList();

                var teacherCache = db.PracticeTeachers.ToList();
                var themeCache = db.PracticeThemes.ToList();

                var practices = db.Practices.Include(p => p.Group.Group).Where(p => 
                    p.Year == filterYear
                    && p.SemesterId == filterSemesterId
                    && (filterGroups.Count == 0 || filterGroups.Contains(p.GroupHistoryId))
                    && (filterDivisionUUIDs.Contains(p.Group.Group.FormativeDivisionId) || filterDivisionUUIDs.Contains(p.Group.Group.FormativeDivisionParentId))
                    && (filterDirectionUIDs.Count == 0 || filterDirectionUIDs.Contains(p.Group.Profile.DIRECTION_ID))
                    && !p.remove)
                    .ToList();

                Debug.WriteLine($"SQL duration {stopwatch.ElapsedMilliseconds}");

                foreach (var r in groupPlans)
                {
                    //надо понять курс и семестр для практики
                    var terms = planTerms.Where(pt => pt.DisciplineUUID == r.p.disciplineUUID);

                    foreach (var t in terms)
                    {
                        if (filterYear == 0) continue;

                        var group = groupsHistory.FirstOrDefault(g => g.GroupId == r.GroupId && g.Course == t.Course && g.YearHistory == filterYear);

                        if (group == null) continue;

                        if (filterGroups == null) continue;
                        if (filterGroups.Count > 0 && !filterGroups.Contains(group.Id)) continue;
                        if (filterSemesterId != -1 && t.SemesterID != filterSemesterId) continue;

                        Semester semester = semesters.FirstOrDefault(s => s.Id == t.SemesterID);

                        var practiceListViewModel = GetPracticeListViewModel(divisionCache, teacherCache, themeCache,
                            r.p, group, semester, isOldVersion: false);

                        practiceList.Add(practiceListViewModel);

                        practices.RemoveAll(p => p.DisciplineUUID == r.p.disciplineUUID && p.GroupHistoryId == group.Id);
                    }
                }
                
                for(int j = 0; j < practices.Count(); j++)
                {
                    var practice = practices[j];
                    var plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == practice.DisciplineUUID); // && !p.remove);
                    if (plan == null) continue;
                    if (!string.IsNullOrEmpty(filterPracticeName) && !plan.disciplineTitle.Contains(filterPracticeName)) continue;

                    var practiceListViewModel = GetPracticeListViewModel(divisionCache, teacherCache, themeCache,
                        plan, practice.Group, practice.Semester, isOldVersion: !StudentsExtension.GraduatedStatuses.Contains(practice.Student.Status));

                    practiceList.Add(practiceListViewModel);
                    
                    practices.RemoveAll(p => p.DisciplineUUID == plan.disciplineUUID && p.GroupHistoryId == practice.GroupHistoryId);
                    j = -1;
                }

                if (filterOldVersion != null)
                {
                    practiceList = practiceList.Where(p => p.IsOldPlanVersion == filterOldVersion).ToList();
                }

                var result = practiceList.OrderBy(r => r.PracticeType).ThenBy(r => r.Name);

                stopwatch.Stop();

                Debug.WriteLine($"GetGroups duration {stopwatch.ElapsedMilliseconds}");

                return Json(
                       new
                       {
                           data = result,
                           total = result.Count()
                       },
                       new JsonSerializerSettings()
                   ); ;
            }

            return null;
        }

        private PracticeListViewModel GetPracticeListViewModel(List<Division> divisionCache, List<PracticeTeacher> teacherCache, List<PracticeTheme> themeCache,
            Plan plan, GroupsHistory historyGroup, Semester semester, bool isOldVersion)
        {
            var department = divisionCache.FirstOrDefault(d => d.uuid == historyGroup.Group.FormativeDivisionId);
            var departmentParent = divisionCache.FirstOrDefault(d => d.uuid == historyGroup.Group.FormativeDivisionParentId);

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

            var _teachers = teacherCache.Where(_t => _t.Year == historyGroup.YearHistory && _t.SemesterId == semester.Id
                    && _t.DisciplineUUID == plan.disciplineUUID && _t.GroupHistoryId == historyGroup.Id).OrderBy(_t => _t.Teacher.initials)
                    .Select(_t => _t.Teacher.initials);
            var teachers = String.Join(", ", _teachers);

            var _themes = themeCache.Where(_t => _t.Year == historyGroup.YearHistory && _t.SemesterId == semester.Id
                && _t.DisciplineUUID == plan.disciplineUUID && _t.GroupHistoryId == historyGroup.Id)
                .Select(_t => _t.Theme);
            var themes = String.Join(", ", _themes);

            var info = db.PracticeInfo.FirstOrDefault(i => i.DisciplineUUID == plan.disciplineUUID && i.GroupId == historyGroup.Id && i.SemesterId == semester.Id);
            string practiceDates = GetPracticeDates(info?.BeginDate, info?.EndDate);
            string beginDate = (info?.BeginDate != null) ? info.BeginDate.Value.ToShortDateString() : "";
            string endDate = (info?.EndDate != null) ? info.EndDate.Value.ToShortDateString() : "";
            string reportDates = GetPracticeDates(info?.ReportBeginDate, info?.ReportEndDate);
            string reportBeginDate = (info?.ReportBeginDate != null) ? info.ReportBeginDate.Value.ToShortDateString() : "";
            string reportEndDate = (info?.ReportEndDate != null) ? info.ReportEndDate.Value.ToShortDateString() : "";


            var practiceListViewModel = new PracticeListViewModel()
            {
                DisciplineTitle = plan.disciplineTitle,
                DisciplineUUID = plan.disciplineUUID,
                Department = departmentTitle,
                Institute = institute,
                InstituteId = instituteId,
                Okso = historyGroup.Profile.Direction.okso,
                Direction = historyGroup.Profile.Direction.title,
                DirectionId = historyGroup.Profile.Direction.uid,
                Group = historyGroup.Name,
                GroupYear = historyGroup.YearHistory.ToString(),
                Teachers = teachers,
                Themes = themes,
                PracticeType = plan.additionalType,
                EduplanUUID = plan.eduplanUUID,
                GroupId = historyGroup.Id,
                SemesterName = semester.Name,
                SemesterId = semester.Id,
                Year = historyGroup.YearHistory,
                PracticeDates = practiceDates,
                WayId = info?.PracticeWayId,
                WayDescription = info?.Way?.Description ?? "",
                TimeId = info?.PracticeTimeId,
                TimeDescription = info?.Time?.Description ?? "",
                BeginDate = beginDate,
                EndDate = endDate,
                Subdivision = info?.Subdivision,
                IsOldPlanVersion = isOldVersion,
                IsRemovedDiscipline = plan.remove,
                PlanNumber = plan.eduplanNumber.Value,
                PlanVersion = plan.versionNumber,
                PlanStatus = plan.versionStatus,
                ReportBeginDate = reportBeginDate,
                ReportEndDate = reportEndDate,
                ReportDates = reportDates,
                Standard = historyGroup.Profile.Direction.standard
            };

            return practiceListViewModel;
        }

        public ActionResult CopyPractice(string disciplineUidOld, string disciplineUidNew, string groupHistoryId, int semesterId, int year)
        {
            var groupHistory = db.GroupsHistories.FirstOrDefault(h => h.Id == groupHistoryId && h.YearHistory == year);

            var oldPlan = db.Plans.FirstOrDefault(p => p.disciplineUUID == disciplineUidOld);
            var newPlan = db.Plans.FirstOrDefault(p => p.disciplineUUID == disciplineUidNew);

            CopyPracticeInfo(disciplineUidOld, disciplineUidNew, groupHistoryId, semesterId);
            CopyPracticeTeachers(disciplineUidOld, disciplineUidNew, groupHistoryId, semesterId, year);
            CopyPracticeThemes(disciplineUidOld, disciplineUidNew, groupHistoryId, semesterId, year);
            CopyPractices(disciplineUidOld, disciplineUidNew, groupHistoryId, semesterId, year);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        private void CopyPracticeInfo(string disciplineUidOld, string disciplineUidNew, string groupHistoryId, int semesterId)
        {
            var oldPracticeInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == disciplineUidOld && p.GroupId == groupHistoryId && p.SemesterId == semesterId);
            var newPracticeInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == disciplineUidNew && p.GroupId == groupHistoryId && p.SemesterId == semesterId);

            if (oldPracticeInfo != null)
            {
                bool isNewPracticeInfo = newPracticeInfo == null;
                if (isNewPracticeInfo)
                    newPracticeInfo = new PracticeInfo()
                    {
                        DisciplineUUID = disciplineUidNew,
                        SemesterId = semesterId,
                        GroupId = groupHistoryId
                    };

                newPracticeInfo.BeginDate = newPracticeInfo.BeginDate ?? oldPracticeInfo.BeginDate;
                newPracticeInfo.EndDate = newPracticeInfo.EndDate ?? oldPracticeInfo.EndDate;
                newPracticeInfo.ExecutorEmail = newPracticeInfo.ExecutorEmail ?? oldPracticeInfo.ExecutorEmail;
                newPracticeInfo.ExecutorName = newPracticeInfo.ExecutorName ?? oldPracticeInfo.ExecutorName;
                newPracticeInfo.ExecutorPhone = newPracticeInfo.ExecutorPhone ?? oldPracticeInfo.ExecutorPhone;
                newPracticeInfo.PracticeTimeId = newPracticeInfo.PracticeTimeId ?? oldPracticeInfo.PracticeTimeId;
                newPracticeInfo.PracticeWayId = newPracticeInfo.PracticeWayId ?? oldPracticeInfo.PracticeWayId;
                newPracticeInfo.Subdivision = newPracticeInfo.Subdivision ?? oldPracticeInfo.Subdivision;

                if (isNewPracticeInfo)
                    db.PracticeInfo.Add(newPracticeInfo);
                db.SaveChanges();
            }
        }

        private void CopyPracticeTeachers(string disciplineUidOld, string disciplineUidNew, string groupHistoryId, int semesterId, int year)
        {
            var oldPracticeTeachers = db.PracticeTeachers.Where(t => t.DisciplineUUID == disciplineUidOld && t.GroupHistoryId == groupHistoryId
                && t.SemesterId == semesterId && t.Year == year).ToList();
            var newPracticeTeachers = db.PracticeTeachers.Where(t => t.DisciplineUUID == disciplineUidNew && t.GroupHistoryId == groupHistoryId
                && t.SemesterId == semesterId && t.Year == year).ToList();

            foreach (var oldTeacher in oldPracticeTeachers)
            {
                if (newPracticeTeachers.FirstOrDefault(t => t.TeacherPKey == oldTeacher.TeacherPKey) == null)
                    db.PracticeTeachers.Add(new PracticeTeacher()
                    {
                        DisciplineUUID = disciplineUidNew,
                        GroupHistoryId = groupHistoryId,
                        SemesterId = semesterId,
                        Year = year,
                        TeacherPKey = oldTeacher.TeacherPKey
                    });
            }
            db.SaveChanges();
        }

        private void CopyPracticeThemes(string disciplineUidOld, string disciplineUidNew, string groupHistoryId, int semesterId, int year)
        {
            var oldPracticeThemes = db.PracticeThemes.Where(t => t.DisciplineUUID == disciplineUidOld && t.GroupHistoryId == groupHistoryId
                && t.SemesterId == semesterId && t.Year == year).ToList();
            var newPracticeThemes = db.PracticeThemes.Where(t => t.DisciplineUUID == disciplineUidNew && t.GroupHistoryId == groupHistoryId
                && t.SemesterId == semesterId && t.Year == year).ToList();

            foreach (var oldTheme in oldPracticeThemes)
            {
                if (newPracticeThemes.FirstOrDefault(t => t.Theme == oldTheme.Theme) == null)
                    db.PracticeThemes.Add(new PracticeTheme()
                    {
                        DisciplineUUID = disciplineUidNew,
                        GroupHistoryId = groupHistoryId,
                        SemesterId = semesterId,
                        Year = year,
                        Theme = oldTheme.Theme
                    });
            }
            db.SaveChanges();
        }

        public void CopyPractices(string disciplineUidOld, string disciplineUidNew, string groupHistoryId, int semesterId, int year)
        {
            var groupHistory = db.GroupsHistories.Include(g => g.Group).FirstOrDefault(h => h.Id == groupHistoryId && h.YearHistory == year);
            
            var oldPractices = db.Practices.Where(p => p.DisciplineUUID == disciplineUidOld && p.GroupHistoryId == groupHistoryId
                && p.SemesterId == semesterId && p.Year == year).ToList();
            var newPractices = db.Practices.Where(p => p.DisciplineUUID == disciplineUidNew && p.GroupHistoryId == groupHistoryId
                && p.SemesterId == semesterId && p.Year == year).ToList();

            var students = db.Students.Where(s => s.GroupId == groupHistory.Group.Id).ToList();
            foreach (var student in students)
            {
                var oldStudentPractice = oldPractices.FirstOrDefault(p => p.StudentId == student.Id);
                var newStudentPractice = newPractices.FirstOrDefault(p => p.StudentId == student.Id);
                if (oldStudentPractice != null)
                {
                    bool isNewStudentPractice = newStudentPractice == null;
                    if (isNewStudentPractice)
                    {
                        newStudentPractice = new Practice();
                        newStudentPractice.DisciplineUUID = disciplineUidNew;
                        newStudentPractice.SemesterId = semesterId;
                        newStudentPractice.Year = year;
                        newStudentPractice.StudentId = student.Id;
                        newStudentPractice.GroupHistoryId = oldStudentPractice.GroupHistoryId;
                    }

                    newStudentPractice.BeginDate = newStudentPractice.BeginDate ?? oldStudentPractice.BeginDate;
                    newStudentPractice.EndDate = newStudentPractice.EndDate ?? oldStudentPractice.EndDate;
                    newStudentPractice.ExternalBeginDate = newStudentPractice.ExternalBeginDate ?? oldStudentPractice.ExternalBeginDate;
                    newStudentPractice.ExternalEndDate = newStudentPractice.ExternalEndDate ?? oldStudentPractice.ExternalEndDate;
                    newStudentPractice.FinishTheme = newStudentPractice.FinishTheme ?? oldStudentPractice.FinishTheme;
                    newStudentPractice.ExistContract = oldStudentPractice.ExistContract;
                    newStudentPractice.IsExternal = oldStudentPractice.IsExternal;

                    if (isNewStudentPractice)
                        db.Practices.Add(newStudentPractice);

                    oldStudentPractice.remove = true;
                    db.SaveChanges();

                    var practiceThemes = db.PracticeThemes.Where(t => t.DisciplineUUID == disciplineUidNew && t.GroupHistoryId == groupHistoryId
                        && t.SemesterId == semesterId && t.Year == year).ToList();

                    if (newStudentPractice.Admissions == null || newStudentPractice.Admissions?.Count == 0)
                    {
                        foreach(var adm in oldStudentPractice.Admissions)
                        {
                            var newPracticeAdmission = new PracticeAdmission()
                            {
                                CreateDate = adm.CreateDate,
                                PracticeId = newStudentPractice.Id,
                                ReasonOfDeny = adm.ReasonOfDeny,
                                Status = adm.Status,
                                Subdivision = adm.Subdivision,
                                TeacherPKey = adm.TeacherPKey,
                                TeacherPKey2 = adm.TeacherPKey2
                            };

                            if (adm.PracticeThemeId != null)
                            {
                                var theme = practiceThemes.FirstOrDefault(p => p.Theme == adm.Theme.Theme);
                                newPracticeAdmission.PracticeThemeId = theme?.Id;
                            }

                            db.PracticeAdmissions.Add(newPracticeAdmission);
                            adm.remove = true;
                            db.SaveChanges();
                        }
                    }

                    if (newStudentPractice.AdmissionCompanys == null || newStudentPractice.AdmissionCompanys?.Count == 0)
                    {
                        foreach (var adm in oldStudentPractice.AdmissionCompanys)
                        {
                            var newPracticeAdmissionCompany = new PracticeAdmissionCompany()
                            {
                                CreateDate = adm.CreateDate,
                                PracticeId = newStudentPractice.Id,
                                ReasonOfDeny = adm.ReasonOfDeny,
                                Status = adm.Status,
                                ContractId = adm.ContractId
                            };
                            
                            db.PracticeAdmissionCompanys.Add(newPracticeAdmissionCompany);
                            adm.remove = true;
                            db.SaveChanges();
                        }
                    }

                    if (newStudentPractice.Documents == null || newStudentPractice.Documents?.Count == 0)
                    {
                        foreach (var doc in oldStudentPractice.Documents)
                        {
                            var newDocument = new PracticeDocument()
                            {
                                Comment = doc.Comment,
                                DocumentType = doc.DocumentType,
                                FileStorageId = doc.FileStorageId,
                                PracticeId = newStudentPractice.Id,
                                Status = doc.Status
                            };

                            db.PracticeDocuments.Add(newDocument);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        private string GetPracticeDates(DateTime? beginDate, DateTime? endDate)
        {
            string practiceDates = "";
            if (beginDate != null)
                practiceDates += "c " + beginDate.Value.ToShortDateString();
            if (endDate != null)
                practiceDates += " по " + endDate.Value.ToShortDateString();
            return practiceDates;
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

        public ActionResult PracticeInfo(string disciplineUUID, string groupId, int semesterId)
        {
            var info = db.PracticeInfo.FirstOrDefault(i => i.DisciplineUUID == disciplineUUID && i.GroupId == groupId && i.SemesterId == semesterId);
            return Json(
                new
                {
                    data = info
                },
                new JsonSerializerSettings()
            );
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult EditPractice(string disciplineUUID, string groupId, int semesterId,
            int? practiceWayId, int? practiceTimeId, DateTime? beginDate, DateTime? endDate, DateTime? reportBeginDate, DateTime? reportEndDate)
        {         
            var msg = CheckDates(beginDate, endDate, reportBeginDate, reportEndDate);
            if (!string.IsNullOrEmpty(msg))
            {
                return Json(
                        new
                        {
                            success = false,
                            //msg = "Дата завершения практики не может быть раньше даты начала"
                            msg
                        }); //,
                        //"text/html", Encoding.Unicode);
            }
            else
            {
                var info = db.PracticeInfo.FirstOrDefault(i =>
                    i.DisciplineUUID == disciplineUUID && i.GroupId == groupId && i.SemesterId == semesterId);                
                bool isNewInfo = info == null;
                if (isNewInfo)
                    info = new PracticeInfo()
                    {
                        DisciplineUUID = disciplineUUID,
                        GroupId = groupId,
                        SemesterId = semesterId
                    };
                info.PracticeWayId = practiceWayId;
                info.PracticeTimeId = practiceTimeId;
                info.BeginDate = beginDate;
                info.EndDate = endDate;
                info.ReportBeginDate = reportBeginDate;
                info.ReportEndDate = reportEndDate;

                var practices = db.Practices.Where(p =>
                    p.DisciplineUUID == disciplineUUID && p.GroupHistoryId == groupId && p.SemesterId == semesterId);
                foreach (var p in practices)
                {
                    if (p.BeginDate == null)
                        p.BeginDate = info.BeginDate;
                    if (p.EndDate == null)
                        p.EndDate = info.EndDate;
                    if (p.ReportBeginDate == null)
                        p.ReportBeginDate = info.ReportBeginDate;
                    if (p.ReportEndDate == null)
                        p.ReportEndDate = info.ReportEndDate;
                    if (info.PracticeWayId == 1) // если стационарный способ проведения практики
                    {
                        p.IsExternal = false;
                        p.ExternalBeginDate = null;
                        p.ExternalEndDate = null;
                    }
                }

                if (isNewInfo)
                    db.PracticeInfo.Add(info);
                db.SaveChanges();

                Log(disciplineUUID, groupId, semesterId, "Изменена общая информация о практике");

                var newInfo = db.PracticeInfo.Include("Time").Include("Way").FirstOrDefault(i =>
                    i.DisciplineUUID == disciplineUUID && i.GroupId == groupId && i.SemesterId == semesterId);
                string newBeginDate = (newInfo?.BeginDate != null) ? newInfo.BeginDate.Value.ToShortDateString() : "";
                string newEndDate = (newInfo?.EndDate != null) ? newInfo.EndDate.Value.ToShortDateString() : "";
                string newReportBeginDate = (newInfo?.ReportBeginDate != null)
                    ? newInfo.ReportBeginDate.Value.ToShortDateString()
                    : "";
                string newReportEndDate = (newInfo?.ReportEndDate != null)
                    ? newInfo.ReportEndDate.Value.ToShortDateString()
                    : "";

                return Json(
                    new
                    {
                        success = true,
                        wayDescription = newInfo.Way?.Description,
                        wayId = newInfo.Way?.Id,
                        timeDescription = newInfo.Time?.Description,
                        timeId = newInfo.Time?.Id,
                        practiceDates = GetPracticeDates(newInfo.BeginDate, newInfo.EndDate),
                        beginDate = newBeginDate,
                        endDate = newEndDate,
                        reportBeginDate = newReportBeginDate,
                        reportEndDate = newReportEndDate,
                        reportDates = GetPracticeDates(newInfo.ReportBeginDate, newInfo.ReportEndDate)
                    });
                    //"text/html", Encoding.Unicode);
            }
        }

        public string CheckDates(DateTime? startdate,DateTime? enddate, DateTime? reportbegindate, DateTime? peroptenddate)
        {
            string msg1 = "Дата завершения раньше даты начала!";
            string msg2 = "Дата сдачи отчета раньше даты начала практики";
            if (startdate != null && enddate != null )
            {
                if (startdate> enddate)
                {
                    return msg1;
                }
                else if(reportbegindate!= null && startdate> reportbegindate || peroptenddate != null && startdate> peroptenddate)
                {
                    return msg2;
                }                
            }
            else if (reportbegindate != null && peroptenddate != null && reportbegindate > peroptenddate)
                return msg1;

            return null;
        }

    
        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult SetSubdivision(string disciplineUUID, string groupId, int semesterId, string subdivision)
        {
            var info = db.PracticeInfo.FirstOrDefault(i => i.DisciplineUUID == disciplineUUID && i.GroupId == groupId && i.SemesterId == semesterId);
            bool isNewInfo = info == null;
            if (isNewInfo)
                info = new PracticeInfo()
                {
                    DisciplineUUID = disciplineUUID,
                    GroupId = groupId,
                    SemesterId = semesterId
                };

            info.Subdivision = subdivision;
            var admissions = db.Practices.Include("Admissions").Where(p => p.DisciplineUUID == disciplineUUID && p.GroupHistoryId == groupId && p.SemesterId == semesterId)
                .SelectMany(p => p.Admissions).Where(a => string.IsNullOrEmpty(a.Subdivision)).ToList();
            foreach (var a in admissions)
            {
                a.Subdivision = subdivision;
            }

            if (isNewInfo)
                db.PracticeInfo.Add(info);
            db.SaveChanges();

            Log(disciplineUUID, groupId, semesterId, "Изменена общая информация о практике");
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult Themes(string disciplineUUID, int year, int semesterId, string groupHistoryId)
        {
            var themes = db.PracticeThemes.Where(t => t.DisciplineUUID == disciplineUUID && t.Year == year
                && t.SemesterId == semesterId && t.GroupHistoryId == groupHistoryId).Select(t => new
                {
                    t.Id,
                    t.DisciplineUUID,
                    t.SemesterId,
                    t.Year,
                    t.Theme,
                    t.GroupHistoryId
                }).OrderBy(t => t.Theme);
            return Json(
                new
                {
                    data = themes
                },
                new JsonSerializerSettings()
            );
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult CheckExistsThemesStudents(int themeId, string groupHistoryId)
        {
            var practiceTheme = db.PracticeThemes.FirstOrDefault(t => t.Id == themeId && t.GroupHistoryId == groupHistoryId);
            if (practiceTheme != null)
            {
                var admissions = db.PracticeAdmissions.Where(a => a.PracticeThemeId == themeId);

                if (admissions.Count() == 0)
                    return new StatusCodeResult(StatusCodes.Status200OK);
                else
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
        }

        public ActionResult GetThemesStr(string disciplineUUID, int year, int semesterId, string groupHistoryId)
        {
            var themes = db.PracticeThemes.Where(t => t.DisciplineUUID == disciplineUUID && t.Year == year
                    && t.SemesterId == semesterId && t.GroupHistoryId == groupHistoryId).ToList();
            var themesStr = string.Join(", ", themes.Select(t => t.Theme));

            return Json(new { themes = themesStr });//, "text/html", Encoding.Unicode);
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult UpdateTheme(int id, string themeName, string disciplineUUID, int year, int semesterId, string groupHistoryId)
        {
            PracticeTheme theme = db.PracticeThemes.FirstOrDefault(t => t.Id == id);
            if (theme != null)
            {
                theme.Theme = themeName;
                db.SaveChanges();

                Log(disciplineUUID, groupHistoryId, semesterId, $"Изменена тема (id={theme.Id}) новая: {theme.Theme}");

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            if (id == -1)
            {
                // новая тема 
                theme = new PracticeTheme()
                {
                    DisciplineUUID = disciplineUUID,
                    GroupHistoryId = groupHistoryId,
                    SemesterId = semesterId,
                    Theme = themeName,
                    Year = year
                };
                db.PracticeThemes.Add(theme);
                db.SaveChanges();

                Log(disciplineUUID, groupHistoryId, semesterId, $"Добавлена тема {theme.Theme}");

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult RemoveTheme(int id)
        {
            var theme = db.PracticeThemes.FirstOrDefault(t => t.Id == id);

            if (theme != null)
            {
                // удаление тем
                var practices = db.Practices.Where(p =>
                    p.SemesterId == theme.SemesterId &&
                    p.Year == theme.Year &&
                    p.DisciplineUUID == theme.DisciplineUUID &&
                    p.GroupHistoryId == theme.GroupHistoryId
                    ).Select(p => p.Id).ToList();

                // изменение статуса заявок
                var admissions = db.PracticeAdmissions.Where(a => a.PracticeThemeId == theme.Id && practices.Contains(a.PracticeId)).ToList();
                foreach (var admission in admissions)
                {
                    admission.PracticeThemeId = null;
                    ChangeAdmission(admission);
                }

                Log(theme.DisciplineUUID, theme.GroupHistoryId, theme.SemesterId, $"Удалена тема {theme.Theme}");
                db.PracticeThemes.Remove(theme);
                db.SaveChanges();
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult SetThemeToGroup(int id)
        {
            var theme = db.PracticeThemes.FirstOrDefault(t => t.Id == id);
            if (theme != null)
            {
                var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == theme.DisciplineUUID
                    && p.GroupId == theme.GroupHistoryId && p.SemesterId == theme.SemesterId);

                var admissions = db.PracticeAdmissions
                        .Include(a => a.Teacher)
                        .Include(a => a.Teacher2).ToList();

                var groupId = db.GroupsHistories.FirstOrDefault(g => g.Id == theme.GroupHistoryId).GroupId;
                var students = db.Students
                    .Include(s => s.Person)
                    .Include(s => s.Practices)
                    .Where(s => s.GroupId == groupId)
                    .Select(s => new
                    {
                        Student = s,

                        Practice = s.Practices
                                    .Where(p => p.DisciplineUUID == theme.DisciplineUUID)
                                    .FirstOrDefault()
                    })
                    .ToList();

                foreach (var student in students)
                {
                    var practice = student.Practice;
                    if (practice == null)
                    {
                        practice = new Practice()
                        {
                            BeginDate = practiceInfo?.BeginDate,
                            EndDate = practiceInfo?.EndDate,
                            DisciplineUUID = theme.DisciplineUUID,
                            GroupHistoryId = theme.GroupHistoryId,
                            SemesterId = theme.SemesterId,
                            StudentId = student.Student.Id,
                            Year = theme.Year
                        };
                        db.Practices.Add(practice);
                        db.SaveChanges();
                    }

                    var admission = db.PracticeAdmissions.FirstOrDefault(a => a.PracticeId == practice.Id);
                    if (admission == null)
                    {
                        admission = new PracticeAdmission()
                        {
                            CreateDate = DateTime.Now,
                            PracticeId = practice.Id,
                            PracticeThemeId = theme.Id,
                            Status = AdmissionStatus.Indeterminate
                        };
                        db.PracticeAdmissions.Add(admission);
                        db.SaveChanges();
                    }

                    if (admission.Theme == null)
                    {
                        admission.PracticeThemeId = theme.Id;
                        admission.Status = AdmissionStatus.Indeterminate;
                        db.SaveChanges();
                    }
                }
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public ActionResult Teachers(string disciplineUUID)
        {
            var teachers = db.Teachers.Select(t => new
            {
                teacherId = t.pkey,
                fullName = t.lastName + " " + t.firstName + " " + t.middleName,
                lastName = t.lastName,
                firstName = t.firstName,
                middleName = t.middleName,
                workPlace = t.workPlace,
                email = t.User.Email
            }).OrderBy(t => t.fullName);

            //var serializer = new JavaScriptSerializer();
            //serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult {
                Content = JsonConvert.SerializeObject(teachers, Formatting.Indented),            
                ContentType = "application/json"
            };

            return result;
        }

        public ActionResult SelectedTeachers(string disciplineUUID, int year, int semesterId, string groupHistoryId)
        {
            var teachers = db.PracticeTeachers.Where(p => p.DisciplineUUID == disciplineUUID && p.Year == year
                && p.SemesterId == semesterId && p.GroupHistoryId == groupHistoryId).Select(t => new
                {
                    teacherId = t.Teacher.pkey,
                    fullName = t.Teacher.lastName + " " + t.Teacher.firstName + " " + t.Teacher.middleName,
                    lastName = t.Teacher.lastName,
                    firstName = t.Teacher.firstName,
                    middleName = t.Teacher.middleName,
                    workPlace = t.Teacher.workPlace,
                    selected = true,
                    Id = t.Id,
                    email = t.Email
                }).OrderBy(t => t.fullName);
            return Json(
                new
                {
                    data = teachers
                },
                new JsonSerializerSettings()
            );
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult CheckExistsStudents(string disciplineUUID, string teacherId, int year, int semesterId, string groupHistoryId)
        {
            var practiceTeacher = db.PracticeTeachers.FirstOrDefault(t => t.DisciplineUUID == disciplineUUID && t.Year == year
                && t.SemesterId == semesterId && t.GroupHistoryId == groupHistoryId);
            if (practiceTeacher != null)
            {
                var practices = db.Practices.Where(p => p.SemesterId == semesterId && p.Year == year && p.DisciplineUUID == disciplineUUID && p.GroupHistoryId == groupHistoryId)
                    .Select(p => p.Id).ToList();
                var admissions = db.PracticeAdmissions.Where(a => practices.Contains(a.PracticeId) && (a.TeacherPKey == teacherId || a.TeacherPKey2 == teacherId));

                if (admissions.Count() == 0)
                    return new StatusCodeResult(StatusCodes.Status200OK);
                else
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
        }

        public ActionResult GetTeachersStr(string disciplineUUID, int year, int semesterId, string groupHistoryId)
        {
            var practiceTeachers = db.PracticeTeachers.Where(t =>
                    t.DisciplineUUID == disciplineUUID
                    && t.Year == year
                    && t.SemesterId == semesterId
                    && t.GroupHistoryId == groupHistoryId);

            var practiceTeachersStr = string.Join(", ", practiceTeachers.OrderBy(t => t.Teacher.initials).Select(t => t.Teacher.initials));

            return Json(new { teachers = practiceTeachersStr });//, "text/html", Encoding.Unicode);
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult AddTeacher(string teacherId, string disciplineUUID, int year, int semesterId, string groupHistoryId, string email)
        {
            var practiceTeacher = db.PracticeTeachers.FirstOrDefault(t =>
                t.DisciplineUUID == disciplineUUID
                && t.Year == year
                && t.SemesterId == semesterId
                && t.GroupHistoryId == groupHistoryId
                && t.TeacherPKey == teacherId
                );
            if (practiceTeacher == null)
            {
                practiceTeacher = new PracticeTeacher()
                {
                    DisciplineUUID = disciplineUUID,
                    GroupHistoryId = groupHistoryId,
                    SemesterId = semesterId,
                    TeacherPKey = teacherId,
                    Year = year,
                    Email = email
                };
                db.PracticeTeachers.Add(practiceTeacher);
            }
            else
            {
                practiceTeacher.Email = email;
                db.Entry(practiceTeacher).State = EntityState.Modified;
            }
            db.SaveChanges();
            Log(disciplineUUID, groupHistoryId, semesterId, "Изменен список руководителей");
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult RemoveTeacher(string teacherId, string disciplineUUID, int year, int semesterId, string groupHistoryId)
        {
            var practiceTeacher = db.PracticeTeachers.FirstOrDefault(t =>
                t.DisciplineUUID == disciplineUUID
                && t.Year == year
                && t.SemesterId == semesterId
                && t.GroupHistoryId == groupHistoryId
                && t.TeacherPKey == teacherId
                );

            if (practiceTeacher != null)
            {
                var practices = db.Practices.Where(p => p.SemesterId == semesterId && p.Year == year && p.DisciplineUUID == disciplineUUID && p.GroupHistoryId == groupHistoryId)
                    .Select(p => p.Id).ToList();
                var admissions = db.PracticeAdmissions.Where(a => practices.Contains(a.PracticeId) && (a.TeacherPKey == teacherId || a.TeacherPKey2 == teacherId)).ToList();
                foreach (var admission in admissions)
                {
                    if (admission.TeacherPKey == teacherId) admission.TeacherPKey = null;
                    if (admission.TeacherPKey2 == teacherId) admission.TeacherPKey2 = null;

                    ChangeAdmission(admission);
                }

                db.PracticeTeachers.Remove(practiceTeacher);
                db.SaveChanges();

                Log(disciplineUUID, groupHistoryId, semesterId, "Изменен список руководителей");
            }
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult SetTeacherToGroup(int id)
        {
            var teacher = db.PracticeTeachers.FirstOrDefault(t => t.Id == id);
            if (teacher != null)
            {
                var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == teacher.DisciplineUUID
                    && p.GroupId == teacher.GroupHistoryId && p.SemesterId == teacher.SemesterId);

                var admissions = db.PracticeAdmissions
                        .Include(a => a.Teacher)
                        .Include(a => a.Teacher2).ToList();

                var groupId = db.GroupsHistories.FirstOrDefault(g => g.Id == teacher.GroupHistoryId).GroupId;
                var students = db.Students
                    .Include(s => s.Person)
                    .Include(s => s.Practices)
                    .Where(s => s.GroupId == groupId)
                    .Select(s => new
                    {
                        Student = s,

                        Practice = s.Practices
                                    .Where(p => p.DisciplineUUID == teacher.DisciplineUUID && p.Year == teacher.Year && p.SemesterId == teacher.SemesterId)
                                    .FirstOrDefault()
                    })
                    .ToList();

                foreach (var student in students)
                {
                    var practice = student.Practice;
                    if (practice == null)
                    {
                        practice = new Practice()
                        {
                            BeginDate = practiceInfo?.BeginDate,
                            EndDate = practiceInfo?.EndDate,
                            DisciplineUUID = teacher.DisciplineUUID,
                            GroupHistoryId = teacher.GroupHistoryId,
                            SemesterId = teacher.SemesterId,
                            StudentId = student.Student.Id,
                            Year = teacher.Year
                        };
                        db.Practices.Add(practice);
                        db.SaveChanges();
                    }

                    var admission = db.PracticeAdmissions.FirstOrDefault(a => a.PracticeId == practice.Id);
                    if (admission == null)
                    {
                        admission = new PracticeAdmission()
                        {
                            CreateDate = DateTime.Now,
                            PracticeId = practice.Id,
                            TeacherPKey = teacher.TeacherPKey,
                            Status = AdmissionStatus.Indeterminate
                        };
                        db.PracticeAdmissions.Add(admission);
                        db.SaveChanges();
                    }

                    if (admission.TeacherPKey == null)
                    {
                        admission.TeacherPKey = teacher.TeacherPKey;
                        admission.Status = AdmissionStatus.Indeterminate;
                        db.SaveChanges();
                    }
                }
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        private void ChangeAdmission(PracticeAdmission admission)
        {
            if (admission.PracticeThemeId == null && admission.TeacherPKey == null && admission.TeacherPKey2 == null)
            {
                // заявка пустая. удалеям.
                db.PracticeAdmissions.Remove(admission);
            }
            else
            {
                // изменяем статус, если заявка согласована
                if (admission.Status == AdmissionStatus.Admitted)
                    admission.Status = AdmissionStatus.Denied;

                // добавляем причину отказа
                string reason = "Руководитель (соруководитель)/тема были удалены. Выберите руководителя (соруководителя)/тему заново!";
                if (admission.ReasonOfDeny != null)
                    admission.ReasonOfDeny += " " + reason;
                else
                    admission.ReasonOfDeny = reason;
            }
            db.SaveChanges();


        }

        public ActionResult Years()
        {

            return Json(
                        new
                        {
                            //data = model.Rows,
                            //total = model.Rows.Count()
                        },
                        new JsonSerializerSettings()
                    );
        }

        public ActionResult Semesters()
        {
            var semesters = db.Semesters.ToList();
            return Json(
                new
                {
                    data = semesters
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Divisions()
        {
            var divisions = db.Divisions.Where(d => d.typeTitle == "Институт").Select(d => new
            {
                Id = d.uuid,
                Name = d.shortTitle
            })
                .ToList();
            return Json(
                new
                {
                    data = divisions
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Okso()
        {
            var okso = db.Directions.Select(d => new
            {
                Id = d.uid,
                Name = d.okso + " " + d.title
            }).OrderBy(d => d.Name)
                .ToList();
            return Json(
                new
                {
                    data = okso
                },
                new JsonSerializerSettings()
            );
        }

        //private void Log(ApplicationDbContext db, int practiceId, string message)
        //{
        //    var practice = db.Practices.FirstOrDefault(p => p.Id == practiceId);
        //    Log(db, practice, message);
        //}
        public ActionResult ChangePracticeDates(string grouphistoryId, string disciplineId, int year, int semesterId)
        {
            var practices = db.Practices.Where(p =>
                p.DisciplineUUID == disciplineId && p.GroupHistoryId == grouphistoryId && p.SemesterId == semesterId &&
                p.Year == year).ToList();

            var practiceinfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == disciplineId && p.GroupId == grouphistoryId &&
                                                                 p.SemesterId == semesterId);

            practices.ForEach(p =>
            {
                if (p.TakeDatesfromGroup && practiceinfo?.BeginDate != null && practiceinfo.EndDate != null)
                {
                    p.BeginDate = practiceinfo.BeginDate;
                    p.EndDate = practiceinfo.EndDate;

                    var admission = p.Admissions.Where(a => a.Status != AdmissionStatus.Denied)
                        .OrderByDescending(a => a.Id).FirstOrDefault();
                      
                   if(admission!= null)
                     admission.Dates=PracticePeriodModel.GetDatesJson(new List<DateTime>{(DateTime)practiceinfo.BeginDate, 
                        (DateTime)practiceinfo.EndDate});

                   var companyadmission = p.AdmissionCompanys.Where(a => a.Status != AdmissionStatus.Denied)
                       .OrderByDescending(a => a.Id).FirstOrDefault();

                    if (companyadmission != null)
                        companyadmission.Dates = PracticePeriodModel.GetDatesJson(new List<DateTime>{practiceinfo.BeginDate.Value,
                           practiceinfo.EndDate.Value});
                }
            });
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        public ActionResult ChangePractiсeReportDates(string grouphistoryId, string disciplineId, int year, int semesterId)
        {
            var practices = db.Practices.Where(p =>
                p.DisciplineUUID == disciplineId && p.GroupHistoryId == grouphistoryId && p.SemesterId == semesterId &&
                p.Year == year);

            var practiceinfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == disciplineId && p.GroupId == grouphistoryId &&
                                                                   p.SemesterId == semesterId);
            foreach(var p in practices)
            {
                if (p.TakeReportDatesfromGroup && practiceinfo?.ReportBeginDate != null && practiceinfo.ReportEndDate != null)
                {
                    p.ReportBeginDate = practiceinfo.ReportBeginDate;
                    p.ReportEndDate = practiceinfo.ReportEndDate;
                }
            }
            db.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }


        private void Log(string disciplineUUID, string groupId, int semester, string message)
        {
            var discipline = db.Plans.FirstOrDefault(p => p.disciplineUUID == disciplineUUID);
            var group = db.GroupsHistories.FirstOrDefault(g => g.Id == groupId);

            Logger.Info($"Практика \"{discipline.disciplineTitle}\": {message}, {group.Name}, {group.YearHistory} ({semester})");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            db.Dispose();
        }
    }
}