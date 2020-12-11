using Ext.Utilities;
//using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;

namespace Urfu.Its.Web.Controllers
{
    public class PracticeReportController : BaseController
    {
        private static List<string> PlanPracticeTypes = new List<string> { "Учебная практика", "Производственная практика" };

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private void AddFilterData()
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
            
            ViewBag.Years = JsonConvert.SerializeObject(years);
            ViewBag.Semesters = JsonConvert.SerializeObject(db.Semesters);
            ViewBag.Divisions = JsonConvert.SerializeObject(divisions);
            //ViewBag.FamilirizationTypes = JsonConvert.SerializeObject(db.FamilirizationTypes);
            //ViewBag.Qualifications = JsonConvert.SerializeObject(db.Qualifications);

        }

        public ActionResult Students()
        {
            AddFilterData();

            return View("Students");
        }

        public ActionResult ReportStudents(string filter)
        {
            var reportVms = PrepareReportStudent(filter);
            return JsonNet(reportVms);
        }

        public ActionResult Indicators()
        {
            AddFilterData();

            return View("Indicators");
        }

        public ActionResult ReportIndicators(string filter)
        {
            var reportVms = PrepareReportIndicators(filter);
            return JsonNet(reportVms);
        }


        private IEnumerable<object> PrepareReportStudent(string filter)
        {

            var plans = db.Plans.Where(p => PlanPracticeTypes.Contains(p.additionalType));
            var students = db.Students.Where(s => s.Status == "Активный");
            var divisions = db.DivisionsForUser(User).Where(d => d.typeTitle == "Институт");
            var practices = (IQueryable<Practice>)db.Practices;
            List<string> divisionUUIDs = null;// { divisions.OrderBy(d => d.shortTitle).First().uuid };
            List<string> directionUIDs = null;
            List<string> groupIds = null;
            string practiceName = "";
            string studentName = "";

            //var years = db.GroupsHistories.Select(g => new { Year = g.YearHistory }).Distinct().OrderBy(y => y.Year).ToList();
            //var year = years.Last().Year;
            var year = 0;
            var semesterId = 0;

            //var rules = FilterRules.Deserialize(filter);
            if (filter != null)
            {
                var rules = ObjectableFilterRules.Deserialize(filter);

                foreach (var rule in rules)
                {
                    if (rule.Value != null)
                    {
                        switch (rule.Property)
                        {
                            case "Year":
                                year = Int32.Parse(rule.Value.ToString());
                                practices = practices.Where(p => p.Year == year);
                                break;
                            case "Semester":
                                semesterId = Int32.Parse(rule.Value.ToString());
                                practices = practices.Where(p => p.SemesterId == semesterId);
                                break;
                            case "Division":
                                var _divisionUUIDs = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                                if (_divisionUUIDs?.Count() != 0 && _divisionUUIDs?.First() != "")
                                {
                                    divisionUUIDs = _divisionUUIDs;
                                }
                                break;
                            case "Direction":
                                var _directionUIDs = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                                if (_directionUIDs?.Count() != 0 && _directionUIDs?.First() != "")
                                {
                                    directionUIDs = _directionUIDs;
                                }
                                break;
                            case "Group":
                                var _groupIds = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                                if (_groupIds?.Count() != 0 && _groupIds?.First() != "")
                                {
                                    groupIds = _groupIds;
                                }
                                break;
                            case "PracticeName":
                                practiceName = rule.Value.ToString().ToLower();
                                break;
                            case "StudentName":
                                studentName = rule.Value.ToString().ToLower();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (filter == null || year == 0)
            {
                return new List<object>();
            }

            var query = practices
                .Join(db.Plans, p => p.DisciplineUUID, p => p.disciplineUUID, (p, plan) => new { p, plan })
                .Join(db.Directions, j => j.plan.directionId, d => d.uid, (j, d) => new { j.p, j.plan, d })
                .GroupJoin(db.PracticeAdmissions, j => j.p.Id, a => a.PracticeId, (j, a) => new { j.p, j.plan, j.d, a })
                .SelectMany(j => j.a.DefaultIfEmpty(), (j, a) => new { j.p, j.plan, j.d, a })
                .GroupJoin(db.PracticeAdmissionCompanys, j => j.p.Id, ac => ac.PracticeId, (j, ac) => new { j.p, j.plan, j.d, j.a, ac })
                .SelectMany(j => j.ac.DefaultIfEmpty(), (j, ac) => new
                {
                    j.p,
                    j.p.Semester,
                    j.p.Student,
                    j.p.Student.Person,
                    j.p.Student.Group.ChairId,
                    //j.p.Student.Group.ManagingDivisionId,
                    //j.p.Student.Group.ManagingDivisionParentId,
                    j.p.Student.Group.FormativeDivisionId,
                    j.p.Student.Group.FormativeDivisionParentId,
                    GroupId = j.p.Group.Id,
                    GroupName = j.p.Group.Name,
                    PracticeName = j.plan.disciplineTitle,
                    PracticeType = j.plan.additionalType,
                    DirectionUid = j.d.uid,
                    Okso = j.d.okso,
                    DirectionTitle = j.d.title,
                    j.a,
                    j.a.Teacher,
                    j.a.Teacher2,
                    j.a.Theme,
                    ac
                });

            if (divisionUUIDs != null && divisionUUIDs.Count > 0)
                query = query.Where(j => divisionUUIDs.Contains(j.FormativeDivisionId) || divisionUUIDs.Contains(j.FormativeDivisionParentId));

            if (directionUIDs != null && directionUIDs.Count > 0)
                query = query.Where(j => directionUIDs.Contains(j.DirectionUid));

            if (groupIds != null && groupIds.Count > 0)
                query = query.Where(j => groupIds.Contains(j.GroupId));
            
            var result = query.ToList()
                .Where(q => q.Person.FullName().ToLower().Contains(studentName) && q.PracticeName.ToLower().Contains(practiceName))
                .OrderBy(j => j.Person.Surname)
                .ToList();

            var rows = new List<object>();

            foreach (var r in result)
            {
                var departament = db.Divisions.FirstOrDefault(d => d.uuid == r.FormativeDivisionId);
                var chair = db.Divisions.FirstOrDefault(d => d.uuid == r.ChairId);

                if (year != 0 && r.p.Year != year) continue;
                if (semesterId != 0 && r.p.SemesterId != semesterId) continue;

                var o = new
                {
                    r.Person.Surname,
                    r.Person.Name,
                    r.Person.PatronymicName,
                    r.GroupName,
                    r.Student.IsTarget,
                    r.Student.IsInternational,
                    r.Student.Compensation,

                    Department = chair.shortTitle,
                    Institute = departament.shortTitle,

                    Okso = $"{r.Okso} - {r.DirectionTitle}",
                    r.p.Year,
                    Semester = r.p.Semester.Name,

                    r.PracticeName,
                    r.PracticeType,
                    PrcaticeDate = $"{r.p.BeginDate:dd.MM.yyyy} - {r.p.EndDate:dd.MM.yyyy}",
                    r.p.IsExternal,
                    Teacher = r.a?.Teacher?.BigName,
                    Teacher2 = r.a?.Teacher2?.BigName,
                    r.a?.Theme?.Theme,
                    r.p.FinishTheme,
                    UrfuStatus = r.a?.StatusName,
                    Company = r.ac?.Contract.Company.Name,
                    Country = r.ac?.Contract.Company.Location?.Parent?.Parent?.Name,
                    City = r.ac?.Contract.Company.Location?.Name,
                    r.ac?.Contract.PersonInCharge,
                    CompanyStatus = r.ac?.StatusName
                };

                rows.Add(o);
            }

            return rows;
        }

        private IEnumerable<object> PrepareReportIndicators(string filter)
        {
            var plans = db.Plans.Where(p => PlanPracticeTypes.Contains(p.additionalType));
            var students = db.Students.Where(s => s.Status == "Активный");
            var divisions = db.DivisionsForUser(User).Where(d => d.typeTitle == "Институт");
            var practices = (IQueryable<Practice>)db.Practices;
            List<string> divisionUUIDs = null;// { divisions.OrderBy(d => d.shortTitle).First().uuid };
            List<string> directionUIDs = null;

            //var years = db.GroupsHistories.Select(g => new { Year = g.YearHistory }).Distinct().OrderBy(y => y.Year).ToList();
            //var year = years.Last().Year;
            var year = 0;
            var semesterId = 0;
            string practiceName = "";


            //Падает [{"property":"Year","value":null},{"property":"Semester","value":null},{"property":"Division","value":[]},{"property":"Direction","value":["uncass18ggl5g0000k7gr9huvkmks550"]}]
            //var rules = FilterRules.Deserialize(filter);
            var rules = ObjectableFilterRules.Deserialize(filter);

            foreach (var rule in rules)
            {
                if (rule.Value != null)
                {
                    switch (rule.Property)
                    {
                        case "Year":
                            year = Int32.Parse(rule.Value.ToString());
                            practices = practices.Where(p => p.Year == year);
                            break;
                        case "Semester":
                            semesterId = Int32.Parse(rule.Value.ToString());
                            practices = practices.Where(p => p.SemesterId == semesterId);
                            break;
                        case "Division":
                            divisionUUIDs = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                            break;
                        case "Direction":
                            directionUIDs = JsonConvert.DeserializeObject<List<string>>(rule.Value.ToString());
                            break;
                        case "PracticeName":
                            practiceName = rule.Value?.ToString() ?? "";
                            plans = plans.Where(p => p.disciplineTitle.Contains(practiceName));
                            break;
                        default:
                            break;
                    }
                }
            }
            if (year == 0 || semesterId == 0 || divisionUUIDs == null || divisionUUIDs.Contains(""))
            {
                return new List<object>();
            }

            var query = practices.Where(p => p.Student != null)
                .Join(plans, p => p.DisciplineUUID, p => p.disciplineUUID, (p, plan) => new { p, plan })
                .Join(db.Directions, j => j.plan.directionId, d => d.uid, (j, d) => new { j.p, j.plan, d })
                .GroupJoin(db.PracticeAdmissions, j => j.p.Id, a => a.PracticeId, (j, a) => new { j.p, j.plan, j.d, a })
                .SelectMany(j => j.a.DefaultIfEmpty(), (j, a) => new { j.p, j.plan, j.d, a })
                .GroupJoin(db.PracticeAdmissionCompanys, j => j.p.Id, ac => ac.PracticeId, (j, ac) => new { j.p, j.plan, j.d, j.a, ac })
                .SelectMany(j => j.ac.DefaultIfEmpty(), (j, ac) => new
                {
                    j.p,
                    j.plan,
                    j.d,
                    j.p.Semester,
                    j.p.Student,
                    j.p.Student.Person,
                    j.p.Student.Group.ChairId,
                    //j.p.Student.Group.ManagingDivisionId,
                    //j.p.Student.Group.ManagingDivisionParentId,
                    j.p.Student.Group.FormativeDivisionId,
                    j.p.Student.Group.FormativeDivisionParentId,
                    GroupName = j.p.Group.Name,
                    PracticeName = j.plan.disciplineTitle,
                    PracticeType = j.plan.additionalType,
                    DirectionUid = j.d.uid,
                    Okso = j.d.okso,
                    DirectionTitle = j.d.title,
                });

            
            if (divisionUUIDs != null && divisionUUIDs.Count > 0)
            {
                query = query.Where(j => divisionUUIDs.Contains(j.FormativeDivisionId) || divisionUUIDs.Contains(j.FormativeDivisionParentId));
                divisions = divisions.Where(d => divisionUUIDs.Contains(d.uuid));
            }

            if (directionUIDs != null && directionUIDs.Count > 0 && !directionUIDs.Contains(""))
                query = query.Where(j => directionUIDs.Contains(j.DirectionUid));

            var res = query.ToList();

            var institutes = divisions.ToList();
            
            var rep = res
                .GroupBy(r => new { r.p.Year, r.p.Semester, r.plan, r.p.IsExternal, r.p.Student.Group.FormativeDivisionId, r.d })
                .Select(r=> new
                {
                    r.Key.Year,                  //Учебный год
                    Semester = r.Key.Semester.Name,              //Семестр
                    PracticeName = r.Key.plan.disciplineTitle,  //Название практики
                    PracticeType = r.Key.plan.additionalType,   //Вид практики
                    PrcaticeDate = "", //r.Key.                     //Сроки практики
                    //r.Key.IsExternal,            //Признак с выездом / без
                    Institute = divisions.FirstOrDefault(d=>d.uuid == r.Key.FormativeDivisionId)?.shortTitle,    //Институт
                    Department = "",                         //Департамент
                    Okso = r.Key.d.OksoAndTitle,        //Оксо + Направление

                    //Кол - во не в Екатеринбурге
                    NoEkaterinburgCount = r.Sum(i=>i.p.AdmissionCompanys.Any(c=>c.Status == AdmissionStatus.Admitted && c.Contract.Company.Location?.City() == "Екатеринбург") ? 0 : 1),
                    //Кол - во не в России
                    NoRussiaCount = r.Sum(i => i.p.AdmissionCompanys.Any(c => c.Status == AdmissionStatus.Admitted && c.Contract.Company.Location?.Country() == "Россия") ? 0 : 1),
                    //Кол - во иностранных студентов
                    InostranCount = r.Sum(i => i.p.Student.IsInternational ? 1 : 0),
                    //Кол - во целевых студентов
                    TargetCount = r.Sum(i => i.p.Student.IsTarget ? 1 : 0),
                    //Кол - во бюджет
                    BudgetCount = r.Sum(i => i.p.Student.Compensation == "бюджет" ? 1 : 0),
                    //Кол - во контракт
                    ContractCount = r.Sum(i => i.p.Student.Compensation == "контракт" ? 1 : 0),
                })
                .ToList();
            
            return rep;
        }

        public ActionResult DirectionList(string institute)
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

        public ActionResult DownloadStudentsReportExcel(string filter)
        {
            var students = PrepareReportStudent(filter);

            var stream = new VariantExport().Export(new
            {
                Rows = students
            }, "studentsReportTemplate.xlsx");
            return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, "Отчет по студентам.xlsx");
        }

        public ActionResult DownloadIndicatorsReportExcel(string filter)
        {
            var report = PrepareReportIndicators(filter);

            var stream = new VariantExport().Export(new
            {
                Rows = report
            }, "indicatorsReportTemplate.xlsx");
            return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, "Отчет по показателям.xlsx");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }
    }
}