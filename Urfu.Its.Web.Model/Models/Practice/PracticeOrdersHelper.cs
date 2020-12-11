using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.Practice
{
    public static class PracticeOrdersHelper
    {
        public static List<string> PlanPracticeTypes = new List<string> { "Учебная практика", "Производственная практика" };

        public static int SedOp(PtraciceDecreeStatus status)
        {
            switch (status)
            {
                case PtraciceDecreeStatus.None:
                    return 0;
                case PtraciceDecreeStatus.Create:
                    return 1;
                case PtraciceDecreeStatus.Sended:
                case PtraciceDecreeStatus.Processed:
                case PtraciceDecreeStatus.Sign:
                    return 2;
                case PtraciceDecreeStatus.Revision:
                case PtraciceDecreeStatus.ErorrSED:
                    return 1;

            }
            return 0;
        }

        /// <summary>
        /// Для приказов
        /// </summary>
        /// <param name="decree">Приказ по практикам</param>
        /// <returns></returns>
        public static PracticeOrderModel GetModel(PracticeDecree decree)
        {
            using (var db = new ApplicationDbContext())
            {
                var decreeVM = new PracticeOrderViewModel()
                {
                    DisciplineUID = decree.DisciplineUUID,
                    GroupId = decree.GroupId,
                    SemesterID = decree.SemesterID.Value,
                    OrderNumber = decree.DecreeNumber,
                    OrderDate = decree.DecreeDate,
                    Term = decree.Term ?? 0,
                    OrderId = decree.Id
                };
                
                var model = GetModel(decreeVM, isChangedDecree: false, mainDecreeId: null);
                return model;
            }
        }

        /// <summary>
        /// Для приказов во изменение
        /// </summary>
        /// <param name="decree">Приказ во изменение</param>
        /// <returns></returns>
        public static PracticeOrderModel GetModel(PracticeChangedDecree decree)
        {
            using (var db = new ApplicationDbContext())
            {
                var mainDecree = db.PracticeDecrees.FirstOrDefault(d => d.Id == decree.MainDecreeId);

                var decreeVM = new PracticeOrderViewModel()
                {
                    DisciplineUID = mainDecree.DisciplineUUID,
                    GroupId = mainDecree.GroupId,
                    SemesterID = mainDecree.SemesterID.Value,
                    OrderNumber = decree.DecreeNumber,
                    OrderDate = decree.DecreeDate,
                    Term = mainDecree.Term ?? 0,
                    OrderId = decree.Id
                };
                
                var model = GetModel(decreeVM, isChangedDecree: true, mainDecreeId: decree.MainDecreeId);
                return model;
            }
        }

        private static PracticeOrderModel GetModel(PracticeOrderViewModel decreeVM, bool isChangedDecree, int? mainDecreeId)
        {
            using (var db = new ApplicationDbContext())
            {
                var group = db.GroupsHistories
                    .Include(g => g.Group)
                    .Include(g => g.Profile.Direction)
                    .FirstOrDefault(g => g.Id == decreeVM.GroupId);

                var divisions = db.Divisions.Where(d => d.uuid == group.Group.ChairId
                                        || d.uuid == group.Group.FormativeDivisionId);
                var departament = divisions.FirstOrDefault(d => d.uuid == group.Group.ChairId);
                var institute = divisions.FirstOrDefault(d => d.uuid == group.Group.FormativeDivisionId);

                if (institute.typeTitle == "Департамент")
                {
                    var inst = db.Divisions.FirstOrDefault(d => d.uuid == institute.parent && d.typeTitle == "Институт");
                    institute = inst != null ? inst : institute;
                }

                var practices = db.Practices
                    .Where(p => p.Year == group.YearHistory && p.GroupHistoryId == group.Id && p.SemesterId == decreeVM.SemesterID && p.DisciplineUUID == decreeVM.DisciplineUID)
                    .ToList();

                var studentsQ = db.Students.Where(s => s.GroupId == group.GroupId).ActiveOrGraduated();
                
                if (isChangedDecree)
                {
                    var decreeStudents = db.PracticeChangedDecreeStudents.Where(s => s.ChangedDecreeId == decreeVM.OrderId).Select(s => s.StudentId).ToList();
                    studentsQ = studentsQ.Where(s => decreeStudents.Contains(s.Id));
                }

                var mainDecree = db.PracticeDecrees.FirstOrDefault(d => d.Id == mainDecreeId.Value);

                //получаем все практики для студентов группы, и потом отставим только те которые будут в HistoryGroup
                var plans = db.Plans
                    .Join(studentsQ,
                     p => new { N = p.eduplanNumber, V = (int?)p.versionNumber },
                     s => new { N = s.planVerion, V = s.versionNumber },
                     (p, s) => new { p })
                     .Where(j => PlanPracticeTypes.Contains(j.p.additionalType) && j.p.disciplineUUID == decreeVM.DisciplineUID)
                     .Distinct()
                     .Select(j => new
                     {
                         Plan = j.p,
                         PlanTmers = db.PlanTerms.Where(pt => pt.eduplanUUID == j.p.eduplanUUID).ToList(),
                         PlanTermWeek = db.PlanTermWeeks.Where(ptw => ptw.eduplanUUID == j.p.eduplanUUID).ToList()
                     })
                     .ToList();

                var director = db.Directors.FirstOrDefault(d => d.DivisionUuid == institute.uuid);
                var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == decreeVM.DisciplineUID && p.GroupId == decreeVM.GroupId && p.SemesterId == decreeVM.SemesterID);

                var semesterName = db.Semesters.FirstOrDefault(s => s.Id == decreeVM.SemesterID)?.Name;

                var model = new PracticeOrderModel
                {
                    Number = isChangedDecree ? mainDecree.DecreeNumber : decreeVM.OrderNumber,
                    Date = isChangedDecree ? mainDecree.DecreeDate : decreeVM.OrderDate,
                    Day = isChangedDecree ? $"{mainDecree.DecreeDate:dd}" : $"{decreeVM.OrderDate:dd}",
                    Month = isChangedDecree ? $"{mainDecree.DecreeDate:MM}" : $"{decreeVM.OrderDate:MM}",
                    Year = isChangedDecree ? $"{mainDecree.DecreeDate:yyyy}" : $"{decreeVM.OrderDate:yyyy}",

                    ChangedDecreeNumber = isChangedDecree ? decreeVM.OrderNumber : null,
                    ChangedDecreeDate = isChangedDecree ? decreeVM.OrderDate : null,
                    ChangedDecreeDay = isChangedDecree ? $"{decreeVM.OrderDate:dd}" : null,
                    ChangedDecreeMonth = isChangedDecree ? $"{decreeVM.OrderDate:MM}" : null,
                    ChangedDecreeYear = isChangedDecree ? $"{decreeVM.OrderDate:yyyy}" : null,

                    GroupId = group.Id,
                    GroupName = group.Name,
                    Term = decreeVM.Term,
                    SemesterName = semesterName,
                    EduYear = group.YearHistory,
                    
                    ProfileUUID = group.ProfileId,
                    ProfileName = group.Profile.NAME,

                    Departament = departament.title,
                    InstututeUUID = institute.uuid,
                    Institute = institute.title,
                    InstituteDirector = director?.ShortName2(),

                    OKSO = group.Profile.Direction.OksoAndTitle,
                    ProgramType = group.Profile.QUALIFICATION == "Магистр"
                    ? "Наименование магистерской программы"
                    : "Наименование образовательной программы",
                    ProgramName = group.Profile.OksoAndTitle,

                    ExecutorName = practiceInfo?.ExecutorName,
                    ExecutorPhone = practiceInfo?.ExecutorPhone,
                    ExecutorEmail = practiceInfo?.ExecutorEmail,
                    ROPInitials = practiceInfo?.ROPInitials
                };

                model.Infos = new List<PracticeInfoModel>();
                model.Students = new List<PracticeStudentModel>();
                //info
                var semesters = db.Semesters.ToList();
                var groupPlans = new List<Plan>();

                foreach (var p in plans)
                {
                    var vm = new PlanViewModel(p.Plan, p.PlanTmers, semesters);
                    if (vm.PlanTerms.Any(t => t.Course == group.Course))
                    {
                        groupPlans.Add(p.Plan);

                        var semesterTerms = vm.PlanTerms.Where(t => t.Course == group.Course && t.SemesterID == decreeVM.SemesterID).ToList();

                        var units = semesterTerms.Select(t => p.Plan.GetTermTestUnits(t.Term)).ToList();
                        
                        var info = db.PracticeInfo.FirstOrDefault(i => i.DisciplineUUID == p.Plan.disciplineUUID && i.GroupId == decreeVM.GroupId && i.SemesterId == decreeVM.SemesterID);
                        
                        model.Infos.Add(new PracticeInfoModel
                        {
                            PrcaticeUUID = p.Plan.disciplineUUID,
                            PracticeType = p.Plan.additionalType,
                            PracticeName = p.Plan.disciplineTitle,
                            PracticeTime = info?.Time?.Description ?? "Не задана",
                            PracticeWay = info?.Way?.Description ?? "Не задана",
                            BeginDate = $"{info?.BeginDate:dd.MM.yyyy}",
                            EndDate = $"{info?.EndDate:dd.MM.yyyy}",
                            Units = units.Sum(),
                            Weeks = p.Plan.additionalWeeks,
                            Semester = semesters.FirstOrDefault(s => s.Id == decreeVM.SemesterID)?.Name, //semester,
                            StudyYear = group.YearHistory.ToString() + "/" + (group.YearHistory + 1).ToString(),
                            Standard = group.Profile.Direction.standard
                        });
                    }
                }

                //student
                var students = studentsQ.Include(s => s.Person).OrderBy(s => s.Person.Surname).ToList();
                var pos = 1;
                foreach (var s in students)
                {
                    var studentPractices = practices.Where(p => p.StudentId == s.Id).ToList();
                    var practiceExternalPeriod = new List<PracticePeriodModel>();
                    var teachers = new List<string>();
                    var contractNumber = new List<string>();
                    var addressAndDates = new List<PracticeAddressAndDates>();

                    foreach (var p in groupPlans)
                    {
                        var sp = studentPractices.FirstOrDefault(f => f.DisciplineUUID == p.disciplineUUID);
                        if (sp == null)
                        {
                            practiceExternalPeriod.Add(new PracticePeriodModel());
                        }
                        else
                        {
                            practiceExternalPeriod.Add(new PracticePeriodModel { BeginDate = sp.ExternalBeginDate, EndDate = sp.ExternalEndDate });

                            var compAdm = sp.AdmissionCompanys.Where(a => a.Status == AdmissionStatus.Admitted).FirstOrDefault();
                            var urfu = sp.Admissions.Where(a => a.Status == AdmissionStatus.Admitted).FirstOrDefault();

                            if (urfu != null || compAdm != null)
                                addressAndDates = CreateAddressAndDates(compAdm, urfu, model, 
                                    new PracticePeriodModel { BeginDate = sp.BeginDate, EndDate = sp.EndDate }, departament, contractNumber);

                            if (urfu != null)
                            {
                                if (urfu.Teacher != null)
                                    teachers.Add($"{urfu.Teacher.initials} {urfu.Teacher.post}");
                            }

                        }

                    }
                    
                    var studentReason = db.PracticeChangedDecreeStudents.Include(_ => _.Reason).FirstOrDefault(d => d.StudentId == s.Id && d.ChangedDecreeId == decreeVM.OrderId);
 
                    model.Students.Add(new PracticeStudentModel
                    {
                        Number = pos++,
                        Name = s.Person.FullName(),
                        Budget = s.Compensation,
                        IsTarget = s.IsTarget ? "Да" : "Нет",
                        PracticeExternalPeriod = practiceExternalPeriod,
                        Teachers = string.Join("\n", teachers) ?? "",
                        ContractNumber = string.Join("\n", contractNumber) ?? "",
                        AddressAndDates = addressAndDates,
                        Reason = studentReason?.Reason?.Reason,
                        RecoveryDate = studentReason?.RecoveryDate == null ? "" : "c " + studentReason?.RecoveryDate.Value.ToShortDateString()
                    });


                    switch (s.Compensation)
                    {
                        case "контракт":
                            model.ContractTotal += 1;
                            break;
                        case "бюджет":
                            model.BudgetTotal += 1;
                            break;
                    }

                    if (s.IsTarget)
                        model.TargetTotal += 1;
                }


                model.StudentTotal = students.Count;

                return model;
            }
        }

        private static List<PracticeAddressAndDates> CreateAddressAndDates(PracticeAdmissionCompany compAdm, PracticeAdmission urfuAdm,
            PracticeOrderModel model, PracticePeriodModel periodModel, Division departament, List<string> contructsNumber)
        {
            List<PracticeAddressAndDates> addressAndDates = new List<PracticeAddressAndDates>();
            var compDates = PracticePeriodModel.GetDates(compAdm?.Dates);
            var urfuDates = PracticePeriodModel.GetDates(urfuAdm?.Dates);

            // true - практика на предприятии, false - практика в УрФУ
            Dictionary<PracticePeriodModel, bool> allDates = new Dictionary<PracticePeriodModel, bool>();

            var company = compAdm?.Contract?.Company;
            var contract = compAdm?.Contract;

            // заявки на предприятие нет
            if (company == null)
            {
                // берем только заявку УрФУ 
                // если даты в заявке указаны, то берем их
                if (urfuDates.Count != 0)
                {
                    foreach (var d in urfuDates) { allDates.Add(d, false); }
                }
                else // иначе берем общие даты практики
                {
                    allDates.Add(new PracticePeriodModel(periodModel.BeginDate, periodModel.EndDate), false);
                }
            }
            else // есть заявка на предприятие
            {
                // если указаны даты заявки
                if (compDates.Count != 0)
                {
                    foreach (var d in compDates) { allDates.Add(d, true); }
                }
                else // иначе берем общие даты практики
                {
                    allDates.Add(new PracticePeriodModel(periodModel.BeginDate, periodModel.EndDate), true);
                }

                // если указаны даты в заявке УрФУ
                if (urfuDates.Count != 0)
                {
                    foreach (var d in urfuDates) { allDates.Add(d, false); }
                }
            }

            var _allDates = allDates.OrderBy(d => d.Key.BeginDate);

            foreach (var pair in _allDates)
            {
                var address = new List<string>();

                // практика на предприятии
                if (pair.Value)
                {
                    if (company != null)
                    {
                        address.Add(company.Name);

                        bool contractAdded = false;
                        foreach (var c in contructsNumber)
                        {
                            if (c == contract.Number)
                            {
                                contractAdded = true;
                                break;
                            }
                        }

                        if (!contractAdded)
                        {
                            contructsNumber.Add(contract.Number);
                        }
                        if (company.Location?.Level == 3) // указан город
                        {
                            if (company.Location.Country() != "Россия")
                                address.Add(company.Location.Country());
                            address.Add(company.Location.Name);
                        }

                     }
                }
                // практика в УрФУ
                else
                {
                    if (!string.IsNullOrEmpty(urfuAdm?.Subdivision))
                    {
                        address.Add($"{urfuAdm.Subdivision}");
                    }
                    else
                    {
                        address.Add($"{departament.typeTitle} {departament.title}");
                    }
                    //model.Ekaterinburg += 1;
                    //model.Sverdlovsk += 1; 
                }

                addressAndDates.Add(new PracticeAddressAndDates()
                {
                    Address = string.Join("\n", address) ?? "",
                    PracticePeriod = pair.Key
                });
            }

            // Расчет количества человек в приказе
            if (!(_allDates.FirstOrDefault(d => d.Value == true)).Equals(default(KeyValuePair<PracticePeriodModel, bool>)))
            {
                var location = (company.Location == null) ? 1 :
                               (company.Location.Name == "Екатеринбург") ? 2 :
                               (company.Location.Parent?.Name == "Свердловская область" && company.Location.Name != "Екатеринбург") ? 3 :
                               (company.Location.Parent?.Parent?.Name != "Россия") ? 4 : 5;

                switch (location)
                {
                    case 1:
                        model.Ekaterinburg += 1;
                        model.Sverdlovsk += 1;
                        break;
                    case 2:
                        model.Ekaterinburg += 1;
                        break;
                    case 3:
                        model.Sverdlovsk += 1;
                        break;
                    case 4:
                        model.OtherCountry += 1;
                        break;
                    case 5:
                        model.OtherCity += 1;
                        break;
                }
            }
            else if (!(_allDates.FirstOrDefault(d => d.Value == false)).Equals(default(KeyValuePair<PracticePeriodModel, bool>)))
            {
                model.Ekaterinburg += 1;
            }


                return addressAndDates;
        }

    }
}
