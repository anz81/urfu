using Ext.Utilities;
using Ext.Utilities.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Urfu.Its.Common;
using Urfu.Its.Practices;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Model.Models.Practice;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{

    [Authorize(Roles = ItsRoles.PracticeView)]
    public class PracticeGroupController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Group(string groupId, string disciplineUid, int year, int semesterId)
        {
            //groupId = "undigr18ggl5g0000kb0luu5d6e9aslg";
            //disciplineUid = "unpled18hc2jg0000lmdvksargsf1pmc";

            var groupHistory = db.GroupsHistories.FirstOrDefault(h => h.Id == groupId && h.YearHistory == year);

            //var group = db.Groups.FirstOrDefault(g => g.Id == groupId);
            var plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == disciplineUid);
            var semester = db.Semesters.FirstOrDefault(s => s.Id == semesterId);

            var model = new GroupViewModel(groupHistory.GroupId, groupHistory.Id, disciplineUid)
            {
                GroupName = groupHistory.Name,
                Year = year,
                SemesterID = semesterId,

                Title = new PracticeTitle(
                    semester?.Name + " семестр",
                    year.ToString() + "/" + (year + 1).ToString(),
                    plan.disciplineTitle,
                    plan.additionalType,
                    plan.remove
                ),

                UserIsInRole = User.IsInRole(ItsRoles.ConfirmationOfContractPractice)
            };

            return View("Group", model);
        }

        public ActionResult GroupAjax(string groupId, string disciplineId, int year, int semesterId, bool hideStudents, string filter)
        {
            var model = new GroupViewModel(groupId, null, disciplineId)
            {
                Year = year,
                SemesterID = semesterId
            };
            
            var students = db.Students
                    .Include(s => s.Person)
                    .Include(s => s.Practices)
                    .Where(s => s.GroupId == groupId
                        && (!hideStudents || hideStudents && (s.Status == "Активный" || s.Status == "Отп.с.посещ." || s.Status == "Отп.дород.послерод.")
                        ))
                    .Select(s => new
                    {
                        Student = s,

                        Practice = s.Practices
                                    .Where(p => p.DisciplineUUID == disciplineId && p.Group.GroupId == groupId && p.SemesterId == semesterId && p.Year == year)
                                    .FirstOrDefault()
                    })
                    .ToList();

            var practicIds = students.Where(s => 
                s.Practice != null)
                .Select(s => s.Practice.Id).ToList();

            var documents = db.PracticeDocuments
                .Where(d => practicIds.Contains(d.PracticeId))
                .Select(d => new { d.Id, d.PracticeId, d.DocumentType, d.Status })
                .ToList()
                .Select(_ => new PracticeDocument { Id = _.Id, PracticeId = _.PracticeId, DocumentType = _.DocumentType, Status = _.Status })
                .ToList();

            var rows = model.GetRows(students.Select(a => a.Student).ToList(), documents, groupId);

            var rowsList = rows.Where(FilterRules.Deserialize(filter)).OrderBy(r => r.Name).ToList();
            
            return Json(
                new
                {
                    data = rowsList,
                    total = rowsList.Count()
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Practice(int id, string studentID, string groupID, int year, int semesterID, string disciplineUID)
        {
            if (id == 0)
                id = CreatePractice(studentID, groupID, year, semesterID, disciplineUID);

            if (id == 0)
                return null;

            var admissions = db.PracticeAdmissions
                        .Include(a => a.Teacher)
                        .Include(a => a.Teacher2);

            var admissionCompanys = db.PracticeAdmissionCompanys
                                    .Include(a => a.Contract.Company);

            var practice = db.Practices
                .Include(p => p.Student.Person)
                .Include(p => p.Student.Group)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    Practice = p,
                    Admission = admissions.Where(a => a.PracticeId == id).OrderByDescending(a => a.Id).FirstOrDefault(),
                    AdmissionCompany = admissionCompanys.Where(a => a.PracticeId == id).OrderByDescending(a => a.Id).FirstOrDefault(),
                })
                .FirstOrDefault();

            var person = practice.Practice.Student.Person;
            var group = practice.Practice.Student.Group;

            var contractDs = (!practice.AdmissionCompany?.Contract.IsShortDated) ?? false
                ? practice.AdmissionCompany
                : null;

            var contractKs = practice.AdmissionCompany?.Contract.IsShortDated ?? false
                ? practice.AdmissionCompany
                : null;

            var practiceWayId = db.PracticeInfo.FirstOrDefault(i => i.DisciplineUUID == disciplineUID && i.GroupId == groupID && i.SemesterId == semesterID)?.Way?.Id;

            var plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == disciplineUID);
            var semester = db.Semesters.FirstOrDefault(s => s.Id == semesterID);

            var title = new PracticeTitle(
                semester?.Name + " семестр",
                year.ToString() + "/" + (year + 1).ToString(),
                plan.disciplineTitle,
                plan.additionalType,
                plan.remove
            );

            var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == disciplineUID && p.GroupId == groupID && p.SemesterId == semesterID);

            var department = db.Divisions.FirstOrDefault(d => d.uuid == group.FormativeDivisionId);
            var departmentParent = db.Divisions.FirstOrDefault(d => d.uuid == group.FormativeDivisionParentId);

            var instituteTitle = "";
            if (department != null)
            {
                instituteTitle = department.typeTitle == "Институт" ? department.shortTitle :
                        (departmentParent.typeTitle == "Институт" ? departmentParent.shortTitle : "");
            }

            var contractPeriod = contractDs?.Contract.Periods.FirstOrDefault(p =>
                p.Year == practice.Practice.Year && p.SemesterId == practice.Practice.SemesterId);
            var model = new PracticeViewModel
            {
                Id = id,
                DisciplineUID = practice.Practice.DisciplineUUID,
                Year = year,
                SemesterID = semesterID,
                PracticeInfo = $"{practice.Practice.Group.Name} {person.Surname} {person.Name} {person.PatronymicName}" + ", " + title.Title,
                StartDate = practice.Practice.BeginDate,
                FinishDate = practice.Practice.EndDate,
                IsExternal = practice.Practice.IsExternal,
                IsStationary = practiceWayId == 1, // стационарная практика (в бд Id == 1)
                ExternalStartDate = practice.Practice.ExternalBeginDate,
                ExternalFinishDate = practice.Practice.ExternalEndDate,
                InstituteTitle = instituteTitle,
                ReportStartDate = practice.Practice.ReportBeginDate,
                ReportFinishDate = practice.Practice.ReportEndDate,
                EqualsGroupDates = practice.Practice.TakeDatesfromGroup ,
                EqualsGroupReportDates =practice.Practice.TakeReportDatesfromGroup,


                Urfu = new PracticeUrfuViewModel
                {
                    practiceId = id,
                    FinishTheme = practice.Practice.FinishTheme,
                    ThemeId = practice.Admission?.PracticeThemeId ?? 0,
                    TeacherId = practice.Admission?.TeacherPKey,
                    TeacherId2 = practice.Admission?.TeacherPKey2,
                    Subdivision = practice.Admission?.Subdivision ?? practiceInfo?.Subdivision,
                    Status = practice.Admission?.Status,
                    Reason = practice.Admission?.ReasonOfDeny,
                    Dates = PracticePeriodModel.GetDates(practice.Admission?.Dates)
                },

                ContractDs = new PracticeContractDsViewModel
                {
                    practiceId = id,
                    contractId = contractDs?.ContractId,
                    company = contractDs?.Contract.Company.Name,
                    address = (contractDs?.Contract.Company.Location != null ? contractDs?.Contract.Company.Location.FullLocation() + ", " : "") + contractDs?.Contract.Company.Address,
                    phone = contractDs?.Contract.Company.CompanyPhoneNumber,
                    site = contractDs?.Contract.Company.Site,
                    contractNumber = contractDs?.Contract.Number,
                    personInChargeInfo = !string.IsNullOrEmpty(contractDs?.Contract.PersonInCharge)? $"{contractDs?.Contract.PersonInCharge}, email: {contractDs?.Contract.Company.Email}, телефон: {contractDs?.Contract.Company.PhoneNumber}": null,
                    status = contractDs?.Status,
                    reasonOfDeny = contractDs?.ReasonOfDeny,
                    Dates =  PracticePeriodModel.GetDates(contractDs?.Dates),
                    divisionDescription = contractPeriod?.DivisionDescription,
                    additionalTerms = contractPeriod?.AdditionalTerms,
                    fileId = contractPeriod?.FileStorageId,
                    fileName = contractPeriod?.FileStorageId != null ? contractPeriod?.FileStorage.FileNameForUser : "",
                },

                ContractKs = new PracticeContractKsViewModel
                {
                    practiceId = id,
                    contractId = contractKs?.ContractId ?? 0,
                    companyId = contractKs?.Contract.Company.Id,
                    company = contractKs?.Contract.Company.Name,
                    shortname = contractKs?.Contract.Company.ShortName,
                    director = contractKs?.Contract.Director,
                    directorInitials = contractKs?.Contract.DirectorInitials,
                    directorGenetive = contractKs?.Contract.DirectorGenitive,
                    postOfDirector = contractKs?.Contract.PostOfDirector,
                    postOfDirectorGenetive = contractKs?.Contract.PostOfDirectorGenitive,
                    personInCharge = contractKs?.Contract.PersonInCharge,
                    personInChargeInitials = contractKs?.Contract.PersonInChargeInitials,
                    postPersonInCharge = contractKs?.Contract.PostOfPersonInCharge,
                    inn = contractKs?.Contract.Company.INN,
                    address = contractKs?.Contract.Company.Address,
                    email = contractKs?.Contract.Company.Email,
                    personInChargeEmail = contractKs?.Contract.Email,
                    comment = contractKs?.Contract.Comment,
                    phone = contractKs?.Contract.PhoneNumber,
                    companyPhone = contractKs?.Contract.Company.CompanyPhoneNumber,
                    site = contractKs?.Contract.Company.Site,
                    status = contractKs?.Status,
                    reasonOfDeny = contractKs?.ReasonOfDeny,
                    Dates = PracticePeriodModel.GetDates(contractKs?.Dates),

                },

                //Contracts = ContractsData(practice.Practice, contractDs),
                Teachers = TeachersData(practice.Practice),
                Themes = ThemesData(practice.Practice),

                Before = PracticFileDescriptor.Before().Select(f => new PracticeDocumentViewModel(f)).ToList(),
                After = PracticFileDescriptor.After().Select(f => new PracticeDocumentViewModel(f)).ToList(),
                Distant = PracticFileDescriptor.Distant().Select(f => new PracticeDocumentViewModel(f)).ToList()
            };

            model.Contracts = ContractsData(practice.Practice, model.ContractDs);

            var getсontract = model.Contracts.FirstOrDefault(c=>c.contractId == model.ContractDs.contractId);
            if (contractDs!= null && getсontract == null)
            {
                model.ContractDs.reasonOfDeny += ".  Предприятие: " + model.ContractDs.company;
                model.ContractDs.company = string.Empty;
                model.ContractDs.address = string.Empty;
                model.ContractDs.phone = string.Empty;
                model.ContractDs.site = string.Empty;
                model.ContractDs.contractNumber = string.Empty;
                model.ContractDs.personInChargeInfo = string.Empty;
                model.ContractDs.contractId = null;
                //model.Contracts.Add(model.ContractDs);
            }
            var location = contractKs?.Contract.Company.Location;
            if (location != null)
            {
                if (location.Level == 3)
                {
                    model.ContractKs.cityId = location.Id;
                    location = location.Parent;
                }

                if (location.Level == 2)
                {
                    model.ContractKs.regionId = location.Id;
                    location = location.Parent;
                }

                model.ContractKs.countryId = location.Id;
            }

            FillDocuments(model);

            ViewData["GroupId"] = practice.Practice.GroupHistoryId;

            ViewBag.CanEdit = User.IsInRole(ItsRoles.PracticeManager);

            return View("Practice", model);
        }

        private void FillDocuments(PracticeViewModel model)
        {
            var documents = db.PracticeDocuments
                .Where(d => d.PracticeId == model.Id)
                .Select(d => new 
                { d.DocumentType, 
                    d.Id, 
                    d.Status, 
                    d.Comment,
                    DocumentName= d.FileStorageId != null ?  d.FileStorage.FileNameForUser : "", 
                    Date = d.FileStorageId != null ? (DateTime?)d.FileStorage.Date: null
                }).ToList();

            foreach (var f in model.Before)
            {
                var d = documents.FirstOrDefault(t => t.DocumentType == f.DocumentType);
                if (d == null) continue;

                f.DocumentId = d.Id;
                f.DocumentName = d.DocumentName;
                f.Status = d.Status;
                f.Comment = d.Comment;
                f.Date = d.Date.HasValue ? d.Date.Value.ToShortDateString() : "";
            }

            foreach (var f in model.After)
            {
                var d = documents.FirstOrDefault(t => t.DocumentType == f.DocumentType);
                if (d == null) continue;

                f.DocumentId = d.Id;
                f.DocumentName = d.DocumentName;
                f.Status = d.Status;
                f.Comment = d.Comment;
                f.Date = d.Date.HasValue ? d.Date.Value.ToShortDateString() : "";
            }

            foreach (var f in model.Distant)
            {
                var d = documents.FirstOrDefault(t => t.DocumentType == f.DocumentType);
                if (d == null) continue;

                f.DocumentId = d.Id;
                f.DocumentName = d.DocumentName;
                f.Status = d.Status;
                f.Comment = d.Comment;
                f.Date = d.Date.HasValue ? d.Date.Value.ToShortDateString() : "";
            }
        }

        private int CreatePractice(string studentId, string groupId, int year, int semesterId, string disciplineUid)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var practice = db.Practices.FirstOrDefault(p
                        => p.StudentId == studentId
                        && p.DisciplineUUID == disciplineUid
                        && p.GroupHistoryId == groupId
                        && p.Year == year
                        && p.SemesterId == semesterId);

                    if (practice == null)
                    {
                        var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == disciplineUid && p.GroupId == groupId && p.SemesterId == semesterId);

                        practice = new Practice();

                        practice.StudentId = studentId;
                        practice.DisciplineUUID = disciplineUid;
                        practice.GroupHistoryId = groupId;
                        practice.Year = year;
                        practice.SemesterId = semesterId;
                        practice.BeginDate = practiceInfo?.BeginDate;
                        practice.EndDate = practiceInfo?.EndDate;
                        practice.ReportBeginDate = practiceInfo?.ReportBeginDate;
                        practice.ReportEndDate = practiceInfo?.ReportEndDate;
                        practice.TakeDatesfromGroup = true;
                        practice.TakeReportDatesfromGroup = true;

                        db.Practices.Add(practice);

                        db.SaveChanges();
                    }

                    tran.Commit();

                    return practice.Id;
                }
                catch
                {
                    tran.Rollback();
                    return 0;
                }
            }
        }

        public ActionResult Countries()
        {
            var countries = db.CompanyLocations.Where(l => l.Level == 1 && l.ParentId == null)
                .Select(l => new { CountryId = l.Id, Country = l.Name }).OrderBy(c => c.Country);
            return Json(
                new
                {
                    data = countries
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Regions(int? countryId)
        {
            var regions = db.CompanyLocations.Where(l => l.Level == 2 && l.ParentId == countryId)
                .Select(l => new { RegionId = l.Id, Region = l.Name }).OrderBy(c => c.Region);
            return Json(
                new
                {
                    data = regions
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Cities(int? regionId)
        {
            var cities = db.CompanyLocations.Where(l => l.Level == 3 && l.ParentId == regionId)
                .Select(l => new { CityId = l.Id, City = l.Name }).OrderBy(c => c.City);
            return Json(
                new
                {
                    data = cities
                },
                new JsonSerializerSettings()
            );
        }

        //private static bool ToBool(string value)
        //{
        //    switch (value?.ToLower())
        //    {
        //        case null:
        //            return false;
        //        case "1":
        //        case "true":
        //        case "on":
        //            return true;
        //    }

        //    return false;
        //}

        private void Log(int practiceId, string message)
        {
            var practice = db.Practices.FirstOrDefault(p => p.Id == practiceId);
            Log(practice, message);
        }

        private void Log(Practice practice, string message)
        {

            var student = practice.Student.Person;
            var group = practice.Group;
            var discipline = db.Plans.FirstOrDefault(p => p.disciplineUUID == practice.DisciplineUUID);

            Logger.Info($"Практика \"{discipline.disciplineTitle}\"(Id={practice.Id}): {message}, {student.FullName()}, {group.Name}");
        }

        //[Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult PracticeEdit(PracticeViewModel practiceVM)
        {
            if (!User.IsInRole(ItsRoles.PracticeManager))
                return JsonNet(new { success = false, message = "У вас нет прав вносить изменения" });

            var msg = CheckDates(practiceVM.StartDate, practiceVM.FinishDate, practiceVM.ReportStartDate, practiceVM.ReportFinishDate);
            if (!string.IsNullOrEmpty(msg))
            {
                return JsonNet(new { success = false, message = msg });
            }
            var practice = db.Practices.First(p => p.Id == practiceVM.Id);
            practice.BeginDate = practiceVM.StartDate;
            practice.EndDate = practiceVM.FinishDate;

            practice.IsExternal = practiceVM.IsExternal;
            practice.ExternalBeginDate = practiceVM.ExternalStartDate;
            practice.ExternalEndDate = practiceVM.ExternalFinishDate;

            practice.ReportBeginDate = practiceVM.ReportStartDate;
            practice.ReportEndDate = practiceVM.ReportFinishDate;

            if (practice.TakeDatesfromGroup || practice.TakeReportDatesfromGroup)
            {
                var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == practice.DisciplineUUID && p.GroupId == practice.GroupHistoryId && p.SemesterId == practice.SemesterId);

                practice.TakeDatesfromGroup = practice.TakeDatesfromGroup &&
                                              Equalsdates(practiceInfo?.BeginDate, practiceInfo?.EndDate, practice.BeginDate,
                                                  practice.EndDate);

                practice.TakeReportDatesfromGroup = practice.TakeReportDatesfromGroup &&
                                                    Equalsdates(practiceInfo?.ReportBeginDate,
                                                        practiceInfo?.ReportEndDate, practice.ReportBeginDate,
                                                        practice.ReportEndDate);
            }

            db.SaveChanges();
            Log(practice, "Изменены сроки практики");

            return JsonNet(new { success = true, practice.TakeDatesfromGroup,practice.TakeReportDatesfromGroup,status = "OK" });
        }

        public bool Equalsdates(DateTime? groupStartDate, DateTime? groupEndDate, DateTime? beginDate, DateTime? endDate)
        {
            if (groupStartDate != null && groupEndDate != null &&
                beginDate != null && endDate != null)
            {
                int startdateequal = DateTime.Compare(groupStartDate.Value, beginDate.Value);
                int endrdateequal = DateTime.Compare(groupEndDate.Value, endDate.Value);
                if (startdateequal != 0 || endrdateequal != 0)
                        return false;
                
                return true;
            }

            return false;
        }

        public bool Equalsdates(DateTime? groupStartDate, DateTime? groupEndDate, List<PracticePeriodModel> dates)
        {
            if (groupStartDate != null && groupEndDate != null)
            {
                bool result = true;
                dates.ForEach(d =>
                {
                    result = result && Equalsdates(groupStartDate, groupEndDate, d.BeginDate, d.EndDate);
                });
                return result;
            }
            return false;
        }

        public string CheckDates(DateTime? startdate, DateTime? enddate, DateTime? reportbegindate, DateTime? peroptenddate)
        {
            string msg1 = "Дата завершения раньше даты начала!";
            string msg2 = "Дата сдачи отчета раньше даты начала практики";
            if (startdate != null && enddate != null)
            {
                if (startdate > enddate)
                {
                    return msg1;
                }
                else if (reportbegindate != null && startdate > reportbegindate || peroptenddate != null && startdate > peroptenddate)
                {
                    return msg2;
                }
            }
            else if (reportbegindate != null && peroptenddate != null && reportbegindate > peroptenddate)
                return msg1;

            return null;
        }


        //[Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult PracticeAdmissionEdit(AdmissionStatus status, List<DateTime> dates, int practiceId, string teacher, string teacher2, string subdivision, int? theme, string finishTheme,
            string reason)
        {
            if (!User.IsInRole(ItsRoles.PracticeManager))
                return JsonNet(new { success = false, message = "У вас нет прав вносить изменения" });

            dates = dates ?? new List<DateTime>();
            Practice practice = db.Practices.FirstOrDefault(p => p.Id == practiceId);
            if (dates.Count > 0 && (practice?.BeginDate == null || practice?.EndDate == null))
                return JsonNet(new { success = false, status = "OK", message = "Введите сроки практики в блоке \"Информация о практике\"" });

            bool correctDates = true;
            if (dates.Count > 0)
                correctDates = CheckDates(PracticePeriodModel.GetDates(dates), practice.BeginDate.Value, practice.EndDate.Value);
            if (!correctDates)
                return JsonNet(new { success = false, status = "OK", message = "Даты выходят за сроки проведения практики или соотношение дат начало-конец введено некорректно" });

            if (practice != null && practice.TakeDatesfromGroup && status != AdmissionStatus.Denied)
                practice.TakeDatesfromGroup = Equalsdates(practice.BeginDate, practice.EndDate,
                                                  PracticePeriodModel.GetDates(dates));
           
            SaveAdmission(status, dates, practiceId, teacher, teacher2, subdivision, theme, finishTheme, reason);

            string message = "";
            switch (status)
            {
                case AdmissionStatus.Admitted:
                    message = "Согласована практика в УрФУ";
                    break;
                case AdmissionStatus.Denied:
                    message = "Отклонена практика в УрФУ";
                    break;
                case AdmissionStatus.Indeterminate:
                    message = "Отправлена на формирование практика в УрФУ";
                    break;
            }

            Log(practiceId, message);

            return JsonNet(new { success = true,  practice?.TakeDatesfromGroup, practice?.TakeReportDatesfromGroup, status = "OK" });
        }

        //[Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult DogovorDsEdit(List<DateTime> dates, PracticeContractDsViewModel ds)
        {
            if (!User.IsInRole(ItsRoles.PracticeManager))
                return JsonNet(new { success = false, message = "У вас нет прав вносить изменения" });

            if (ds.contractId.HasValue)
            {
                var practice = db.Practices.FirstOrDefault(p => p.Id == ds.practiceId);
                var admissions = db.PracticeAdmissionCompanys.Where(a => a.Practice.StudentId == practice.StudentId
                    && a.Status == AdmissionStatus.Admitted
                    && a.Practice.SemesterId == practice.SemesterId
                    //&& a.Practice.DisciplineUUID == practice.DisciplineUUID
                    && a.Practice.Year == practice.Year).Select(a => a.ContractId).ToList();

                if (ds.limit <= 0 && !admissions.Contains(ds.contractId.Value) && ds.status == AdmissionStatus.Admitted)
                {
                    return Json(new { success = false, message = "Вы не можете согласовать заявку на данное предприятие, т.к. лимит отрицителен или равен нулю" });
                        //"text/html", Encoding.Unicode);
                }
                dates = dates ?? new List<DateTime>();
                if (dates.Count > 0 && (practice?.BeginDate == null || practice?.EndDate == null))
                    return JsonNet(new { success = false, status = "OK", message = "Введите сроки практики в блоке \"Информация о практике\"" });
                
                bool correctDates = true;
                if (dates.Count > 0)
                    correctDates = CheckDates(PracticePeriodModel.GetDates(dates), practice.BeginDate.Value, practice.EndDate.Value);
                if (!correctDates)
                    return JsonNet(new { success = false, status = "OK", message = "Даты выходят за сроки проведения практики или соотношение дат начало-конец введено некорректно" });

                if (practice != null && practice.TakeDatesfromGroup && ds.status != AdmissionStatus.Denied)
                    practice.TakeDatesfromGroup = Equalsdates(practice.BeginDate, practice.EndDate,
                        PracticePeriodModel.GetDates(dates));
                SaveDogovorDs(ds, dates);

                string message = "";
                switch (ds.status)
                {
                    case AdmissionStatus.Admitted:
                        message = "Согласована практика на предприятие";
                        break;
                    case AdmissionStatus.Denied:
                        message = "Отклонена практика на предприятие";
                        break;
                    case AdmissionStatus.Indeterminate:
                        message = "Отправлена на формирование практика на предприятие";
                        break;
                }
                Log(ds.practiceId, message);
                var contractsData = ContractsData(practice, ds);
                return JsonNet(new { success = true, status = "OK", contractsData = contractsData, practice?.TakeDatesfromGroup,  practice?.TakeReportDatesfromGroup });
            }
            return Json(new { success = false, message = "Предприятие не выбрано" });//, "text/html", Encoding.Unicode);
        }

        private void SaveDogovorDs(PracticeContractDsViewModel ds, List<DateTime> dates)
        {
            var practice = db.Practices
               .Include(p => p.AdmissionCompanys)
               .FirstOrDefault(p => p.Id == ds.practiceId);

            var admission = practice.AdmissionCompanys.OrderByDescending(a => a.Id).FirstOrDefault();
            if (admission == null)
            {
                admission = new PracticeAdmissionCompany();
                admission.CreateDate = DateTime.Now;
                practice.AdmissionCompanys.Add(admission);
            }
            else
            {
                //можно удалить KS договор
            }
            if (ds.contractId.HasValue)
            {
                admission.ContractId = ds.contractId.Value;
                admission.Status = (AdmissionStatus)ds.status;
                admission.Dates = PracticePeriodModel.GetDatesJson(dates);
                admission.ReasonOfDeny = ds.status == AdmissionStatus.Admitted ? null : ds.reasonOfDeny;
            }

            //Отправка заявки в очередь для ЛКП. Отправлять заявки с 2020 года — начало работы ЛКП.
            if (db.SaveChanges()>0 && admission.Agreement && practice.Year>=2020)
                Task.Run(() =>
                    PracticeAdmissionPublication.PublishPracticeAdmission(practice.Id, admission.ContractId));

        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult GetCompany(string q)
        {
           var companies = db.Companies.Where(c => (c.Name.Contains(q) || c.ShortName.Contains(q)) && c.IsConfirmed && c.CompanyLocationId != null)
                    .Select(c => new { CompanyId = c.Id, c.Name })
                    .Distinct().ToList();
                return Json(companies, new JsonSerializerSettings());            
        }

        public ActionResult GetCompanyInfo(int companyId,int practiceId)
        {
            var company = db.Companies.FirstOrDefault(c => c.Id == companyId);
            var ContractKs = new PracticeContractKsViewModel
            {
                practiceId = practiceId,  
                companyId =  company.Id,
                company = company.Name,
                shortname = company.ShortName,
                director = company.Director,
                directorInitials = company.DirectorInitials,
                directorGenetive = company.DirectorGenitive,
                postOfDirector = company.PostOfDirector,
                postOfDirectorGenetive = company.PostOfDirectorGenitive,
                personInCharge = company.PersonInCharge,
                personInChargeInitials = company.PersonInChargeInitials,
                postPersonInCharge = company.PostOfPersonInCharge,
                inn = company.INN,
                address = company.Address,
                email = company.Email,
                phone = company.PhoneNumber,
                companyPhone = company.CompanyPhoneNumber,
                site = company.Site
            };

            var location = company.Location;
            if (location != null)
            {
                if (location.Level == 3)
                {
                    ContractKs.cityId = location.Id;
                    location = location.Parent;
                }

                if (location.Level == 2)
                {
                    ContractKs.regionId = location.Id;
                    location = location.Parent;
                }

                ContractKs.countryId = location.Id;
            }

            return JsonNet(new { success = true, ContractKs });
        }


        //[Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult SaveDogovorKs(AdmissionStatus status, List<DateTime> dates, PracticeContractKsViewModel ks, string instituteTitle)
        {
            try
            {
                if (!User.IsInRole(ItsRoles.PracticeManager))
                    return JsonNet(new { success = false, message = "У вас нет прав вносить изменения" });

                var practice = db.Practices.FirstOrDefault(p => p.Id == ks.practiceId);
                dates = dates ?? new List<DateTime>();
                if (dates.Count > 0 && (practice?.BeginDate == null || practice?.EndDate == null))
                    return JsonNet(new { success = false, status = "OK", message = "Введите сроки практики в блоке \"Информация о практике\"" });

                if (practice?.BeginDate!= null  && practice.EndDate.HasValue)
                {
                    var correctDates = CheckDates(PracticePeriodModel.GetDates(dates), practice.BeginDate.Value, practice.EndDate.Value);
                    if (!correctDates)
                        return JsonNet(new { success = false, status = "OK", message = "Даты выходят за сроки проведения практики или соотношение дат начало-конец введено некорректно" });
                    if (practice.TakeDatesfromGroup && status != AdmissionStatus.Denied)
                        practice.TakeDatesfromGroup = Equalsdates(practice.BeginDate, practice.EndDate,
                            PracticePeriodModel.GetDates(dates));
                }
                SaveDogovorKs(ks, status, instituteTitle, dates);
                string message = "";
                switch (status)
                {
                    case AdmissionStatus.Admitted:
                        message = "Согласована практика на предприятие по персональному договору";
                        break;
                    case AdmissionStatus.Denied:
                        message = "Отклонена практика на предприятие по персональному договору";
                        break;
                    case AdmissionStatus.Indeterminate:
                        message = "Отправлена на формирование практика на предприятие по персональному договору";
                        break;
                }
                Log(ks.practiceId, message);

                return JsonNet(new { success = true, status = "OK", ks.contractId, ks.companyId, practice?.TakeDatesfromGroup,  practice?.TakeReportDatesfromGroup });
            }
            catch (Exception ex)
            {
                return JsonNet(new { success = false, status = "Error", message = ex.Message });
            }
        }

        private void SaveDogovorKs(PracticeContractKsViewModel ks, AdmissionStatus status, string instituteTitle, List<DateTime> dates)
        {
            var practice = db.Practices
                .Include(p => p.AdmissionCompanys)
                .Where(p => p.Id == ks.practiceId)
                .FirstOrDefault();

            var admission = practice.AdmissionCompanys.OrderByDescending(a => a.Id).FirstOrDefault();
            if (admission == null)
            {
                admission = new PracticeAdmissionCompany();
                admission.CreateDate = DateTime.Now;
                practice.AdmissionCompanys.Add(admission);
            }

            var companydata = db.Companies.FirstOrDefault(c=>c.Id == ks.companyId);
            bool updateContract = false;

            if (companydata != null)
            {
                
                if (ks.contractId == 0)
                {
                    admission.Contract = new Contract();
                    admission.Contract.Company = companydata;
                }
                else if (ks.contractId != admission.ContractId)
                {
                    throw new Exception("Номер КС контракта не соответсвует номеру в заявке");
                } 
                
                updateContract = true;
            }
            else 
            {
                admission.Contract = new Contract();
                admission.Contract.Company = new Company();
            }

            // если номер к/с договора не был указан ранее, то генерим его автоматически
            if (admission.Contract.Number == null)
            {
                admission.Contract.Number = db.CreateContractKsNumber(practice.Year, instituteTitle);
                admission.Contract.Year = practice.Year;
                admission.Contract.SerialNumber = db.GetNextSerialNumberKsContract(practice.Year);
            }

            var contract = admission.Contract;
                contract.IsShortDated = true;
                contract.Director = ks.director;
                contract.DirectorInitials = ks.directorInitials;
                contract.DirectorGenitive = ks.directorGenetive;
                contract.PostOfDirector = ks.postOfDirector;
                contract.PostOfDirectorGenitive = ks.postOfDirectorGenetive;
                contract.PersonInCharge = ks.personInCharge;
                contract.PersonInChargeInitials = ks.personInChargeInitials;
                contract.PostOfPersonInCharge = ks.postPersonInCharge;
                contract.Email = ks.personInChargeEmail;
                contract.PhoneNumber = ks.phone;
                contract.Comment = ks.comment;
                

                if (!updateContract)
                {
                    var company = contract.Company;
                    company.INN = ks.inn;
                    company.Name = ks.company;
                    company.ShortName = ks.shortname;
                    company.Address = ks.address;
                    company.Director = ks.director;
                    company.DirectorInitials = ks.directorInitials;
                    company.DirectorGenitive = ks.directorGenetive;
                    company.PostOfDirector = ks.postOfDirector;
                    company.PostOfDirectorGenitive = ks.postOfDirectorGenetive;
                    company.Email = ks.email;
                    company.PersonInCharge = ks.personInCharge;
                    company.PersonInChargeInitials = ks.personInChargeInitials;
                    company.PostOfPersonInCharge = ks.postPersonInCharge;
                    company.PhoneNumber = ks.phone;
                    company.Site = ks.site;
                    company.CompanyPhoneNumber = ks.companyPhone;
                    company.CompanyLocationId = ks.cityId ?? ks.regionId ?? ks.countryId;
                }

                admission.Status = status;
                admission.ReasonOfDeny = status == AdmissionStatus.Admitted ? null : ks.reasonOfDeny;
                admission.Dates = PracticePeriodModel.GetDatesJson(dates);

                db.SaveChanges();

            ks.contractId = contract.Id;
            ks.companyId = contract.CompanyId;
        }

        //[Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult AcceptDocument(int id, AdmissionStatus status, string comment)
        {
            if (!User.IsInRole(ItsRoles.PracticeManager))
                return JsonNet(new { success = false, message = "У вас нет прав вносить изменения" });

            var document = db.PracticeDocuments.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound("Document not found");
            }

            document.Status = status;
            document.Comment = comment;

            if (document.DocumentType == PracticeDocumentType.Contract && status == AdmissionStatus.Admitted)
            {
                var admission = db.PracticeAdmissionCompanys.FirstOrDefault(a => a.PracticeId == document.PracticeId && a.Contract.IsShortDated);

                if (admission != null)
                {
                    admission.Contract.FileStorageId = document.FileStorageId;
                }
            }

            db.SaveChanges();

            var descr = PracticFileDescriptor.Get(document.DocumentType);

            switch (status)
            {
                case AdmissionStatus.Admitted:
                    Log(document.PracticeId, $"Согласован документ {descr.TypeName}");
                    break;
                case AdmissionStatus.Denied:
                    Log(document.PracticeId, $"Отклонен документ {descr.TypeName}");
                    break;
            }

            return Json(
              new
              {
                  success = true,
                  statusId = (int)status,
                  statusName = PracticeDocumentViewModel.GetStatus(status)
              },
              new JsonSerializerSettings()
          );
            //return Json(
            //    new
            //    {
            //        success = true,
            //        statusId =(int)status,
            //        statusName = PracticeDocumentViewModel.GetSatatus(status)
            //    }
            //, "text/html"
            //, Encoding.Unicode);
        }

        //контролируемая загрузка шаблона, если вдруг надо будет там заполнять поля
        public ActionResult DownloadTemplate(int practiceId, PracticeDocumentType type)
        {
            var file = PracticFileDescriptor.Get(type);

            var documents = new PracticeDocuments();

            using (var stream = documents.GetDocuments(type, practiceId))
            {
                var bytes = stream.ToArray();
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
            }
        }

        public ActionResult DownloadDocument(int id)
        {
            var document = db.PracticeDocuments.Include(d=>d.FileStorage).FirstOrDefault(d => d.Id == id);
            if (document?.FileStorageId == null)
            {
                return NotFound("Document not found");
            }

            return File(Model.FileStorageHelper.GetBytes((int)document.FileStorageId), System.Net.Mime.MediaTypeNames.Application.Octet, document.FileStorage.FileNameForUser.ToDownloadFileName());
        }
        public ActionResult DownloadContractPeriodDocument(int id)
        {
            var document = db.FileStorage.FirstOrDefault(d => d.Id == id);
            if  (document?.Id == null)
            {
                return NotFound("Document not found");
            }
            return File(Model.FileStorageHelper.GetBytes(document.Id), System.Net.Mime.MediaTypeNames.Application.Octet, document.FileNameForUser.ToDownloadFileName());
        }
        //[Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult UploadDocument(int practiceId, PracticeDocumentType? type)
        {
            if (Request.Form.Files.Count > 0)
            {
                if (!User.IsInRole(ItsRoles.PracticeManager))
                    return JsonNet(new { success = false, message = "У вас нет прав вносить изменения" });

                var file = Request.Form.Files[0];
                if (type == null)
                {
                    Log(practiceId, $"Попытка загрузить документ {System.IO.Path.GetFileName(file.FileName)} с неопознанным типом");
                }
                else
                {
                    using (var tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var document = db.PracticeDocuments.Where(d => d.PracticeId == practiceId && d.DocumentType == type.Value).FirstOrDefault();

                            if (document == null)
                            {
                                document = new PracticeDocument();
                                document.PracticeId = practiceId;
                                document.DocumentType = type.Value;

                                db.PracticeDocuments.Add(document);
                            }


                            int? id = DataContext.FileStorageHelper.SaveFile(file, DataContext.FileCategory.Practice, folder: $"{document.Practice.Group.Profile.Direction.okso}_{document.Practice.Year}", comment:$"{("PracticeId: " + document.PracticeId + "DocumentType: " + document.DocumentType)}", id: document.FileStorageId);
                            if(id == null)                                                       
                              return Json(new { success = false });

                            document.FileStorageId = id;

                            db.SaveChanges();
                            tran.Commit();

                            var descr = PracticFileDescriptor.Get(type.Value);
                            Log(practiceId, $"Загружен документ {descr.TypeName}");

                            db.Entry(document).Reference(d => d.FileStorage).Load();

                            return Json(new
                            {
                                success = true,
                                fileId = document.Id,
                                fileName = document.FileStorage.FileNameForUser,
                                date = document.FileStorage.Date.ToShortDateString()
                            });
                            //"text/html", Encoding.Unicode);
                        }
                        catch (Exception ex)
                        {                            
                            tran.Rollback();
                            return Json(new { success = false, message = $"{ex.Message}" });//, "text/html", Encoding.Unicode);
                        }
                    }
                }
            }
            return Json(new { success = false, message = "Передан пустой список файлов" });//, "text/html", Encoding.Unicode);
        }

        [Authorize(Roles = ItsRoles.PracticeManager)]
        public ActionResult DeleteDocument(int practiceId, PracticeDocumentType type)
        {            
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var document = db.PracticeDocuments.Where(d => d.PracticeId == practiceId && d.DocumentType == type).FirstOrDefault();

                    if (document == null || document.Status == AdmissionStatus.Admitted)
                        return NotFound("Document not found");

                      if(document.FileStorageId!= null)
                        DataContext.FileStorageHelper.RemoveFile((int)document.FileStorageId);

                    db.PracticeDocuments.Remove(document);
                    db.SaveChanges();
                    tran.Commit();

                    var descr = PracticFileDescriptor.Get(type);
                    Log(practiceId, $"Удален документ {descr.TypeName}");
                    return Json(new { success = true, message = "Документ удален" });//, "text/html", Encoding.Unicode);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Json(new { success = false, message = $"{ex.Message}" });//, "text/html", Encoding.Unicode);
                }
            }
        }

        [Authorize(Roles = ItsRoles.ConfirmationOfContractPractice)]
        public ActionResult CheckConfirmRole()
        {
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [Authorize(Roles = ItsRoles.ConfirmationOfContractPractice)]
        public ActionResult SetExistContract(int practiceId, bool isChecked)
        {
            var practice = db.Practices.FirstOrDefault(p => p.Id == practiceId);
            if (practice != null)
            {
                practice.ExistContract = isChecked;
                db.SaveChanges();
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }
            else
            {
                return Json(new { success = false, message = "Информация по практике студента еще не заполнена" });//, "text/html", Encoding.Unicode);
            }
        }

        private void SaveAdmission(AdmissionStatus status, List<DateTime> dates, int practiceId, string teacher, string teacher2, string subdivision, int? theme, string finishTheme, string reason)
        {
            var practice = db.Practices.Include(p => p.Admissions).First(p => p.Id == practiceId);

            var admission = practice.Admissions.OrderByDescending(a => a.Id).FirstOrDefault();
            if (admission == null)
            {
                admission = new PracticeAdmission();
                admission.CreateDate = DateTime.Now;
                practice.Admissions.Add(admission);
            }

            admission.TeacherPKey = string.IsNullOrWhiteSpace(teacher) ? null : teacher;
            admission.TeacherPKey2 = string.IsNullOrWhiteSpace(teacher2) ? null : teacher2;
            admission.Subdivision = subdivision;
            admission.PracticeThemeId = theme;
            admission.ReasonOfDeny = status == AdmissionStatus.Admitted ? null : reason;
            admission.Dates = PracticePeriodModel.GetDatesJson(dates);
            practice.FinishTheme = finishTheme;

            admission.Status = status;

            db.SaveChanges();
        }

        internal List<TeachersVM> TeachersData(Practice p)
        {
            var teachers = db.PracticeTeachers
               .Where(t => t.DisciplineUUID == p.DisciplineUUID && t.Year == p.Year && t.SemesterId == p.SemesterId && t.GroupHistoryId == p.GroupHistoryId)
               .Select(t => new TeachersVM() {
                   lastName = t.Teacher.lastName,
                   firstName = t.Teacher.firstName,
                   middleName = t.Teacher.middleName,
                   initials = t.Teacher.initials,
                   workPlace = t.Teacher.workPlace,
                   pkey = t.Teacher.pkey,
                   post = t.Teacher.post,
                   email = t.Email
               })
               .OrderBy(t => t.lastName)
               .ToList();

            return teachers;
        }

        internal List<PracticeTheme> ThemesData(Practice p)
        {
            var themes = db.PracticeThemes
                .Where(t => t.DisciplineUUID == p.DisciplineUUID && t.Year == p.Year && t.SemesterId == p.SemesterId && t.GroupHistoryId == p.GroupHistoryId)
                .ToList();

            return themes;
        }

        internal List<PracticeContractDsViewModel> ContractsData(Practice practice, PracticeContractDsViewModel contractDs)
        {
            var plan = db.Plans.FirstOrDefault(p => p.disciplineUUID == practice.DisciplineUUID);

            var periods = db.ContractPeriods
                .Include(p => p.Contract.Company)
                .Where(p => p.Year == practice.Year && p.SemesterId == practice.SemesterId && p.Contract.FinishDate > DateTime.Now && !p.Contract.IsShortDated)
                .Select(p => new
                {
                    Limits = p.Limits.Where(l =>
                           (l.DirectionId == null || l.DirectionId == plan.directionId)
                        && (l.ProfileId == null || l.ProfileId == practice.Group.ProfileId)
                        && (l.Course == 0 || l.Course == practice.Group.Course)
                        && (l.Qualification == null || l.QualificationName == practice.Group.Qual)
                        && (l.Limit >= 0)).ToList(),
                     p.ContractId,
                     p.Contract,
                     p.DivisionDescription,
                     p.AdditionalTerms,
                     p.FileStorage
                })
                .Where(p => p.Limits.Count > 0)
                .ToList();

            var admissions = db.PracticeAdmissionCompanys
                .Include(a => a.Practice.Group)
                .Where(a => a.Practice.SemesterId == practice.SemesterId && a.Practice.Year == practice.Year && a.Status == AdmissionStatus.Admitted)
                .ToList();

            List<PracticeContractDsViewModel> contracts = new List<PracticeContractDsViewModel>();

            foreach (var p in periods)
            {
                var limits = p.Limits
                    .OrderByDescending(l => l.Prioritet())
                    .Select(l => new PracticeLimit { Limit = l })
                    .ToList();

                int over = 0;
                foreach (var l in limits)
                {
                    l.SetAdmissions(db, admissions);

                    over += admissions.Where(a =>
                        a.ContractId == p.ContractId
                        && (l.Limit.DirectionId == null || l.Limit.DirectionId == a.Practice.Group.Profile.DIRECTION_ID)
                        && (l.Limit.ProfileId == null || l.Limit.ProfileId == a.Practice.Group.ProfileId)
                        && (l.Limit.Course == 0 || l.Limit.Course == a.Practice.Group.Course)
                        && (l.Limit.Qualification == null || l.Limit.QualificationName == a.Practice.Group.Qual)
                    ).GroupBy(a => a.Practice.StudentId).Distinct().Count();
                }

                //var studentIds = limits.SelectMany(l => l.Admissions.Select(a => a.Practice.StudentId)).ToList(); // id уже зачисленных студентов. Чтоб не повторялись
                //var over = admissions.Where(a =>
                //    a.ContractId == p.ContractId 
                //    && !studentIds.Contains(a.Practice.StudentId)
                //    ).GroupBy(a => a.Practice.StudentId).Distinct().Count();
                var total = limits.Sum(l => l.Admissions.Count());

                var contract = new PracticeContractDsViewModel()
                {
                    practiceId = practice.Id,
                    contractId = p.ContractId,
                    company = p.Contract.Company.Name,
                    address = (p.Contract?.Company?.Location != null ? p.Contract.Company.Location.FullLocation() + ", " : "") + p.Contract.Company.Address,
                    phone = p.Contract.Company.CompanyPhoneNumber,
                    site = p.Contract.Company.Site,
                    contractNumber = p.Contract.Number,
                    personInChargeInfo = !string.IsNullOrEmpty(p.Contract.PersonInCharge) ? $"{p.Contract.PersonInCharge}, email: {p.Contract.Company.Email}, телефон: {p.Contract.Company.PhoneNumber}": null,
                    limit = p.Limits.Sum(l => l.Limit) - total - over,
                    divisionDescription = p.DivisionDescription,
                    additionalTerms = p.AdditionalTerms,
                    fileName = p.FileStorage?.FileNameForUser,
                    fileId = p.FileStorage?.Id,
                    status = contractDs.contractId == p.ContractId ? contractDs.status : null,
                    reasonOfDeny = contractDs.contractId == p.ContractId ? contractDs.reasonOfDeny : ""
                };

                if (contractDs.contractId == p.ContractId)
                    contractDs.limit = contract.limit;

                contracts.Add(contract);
            }

            return contracts.OrderBy(c => c.company).ToList();
        }

        private bool CheckDates(List<PracticePeriodModel> dates, DateTime beginDate, DateTime endDate)
        {
            bool correct = true;
            foreach (var date in dates)
            {
                if (date.BeginDate < beginDate || date.BeginDate > endDate)
                    correct = false;
                if (date.EndDate < beginDate || date.EndDate > endDate)
                    correct = false;
                if (date.BeginDate > date.EndDate)
                    correct = false;
            }
            return correct;
        }

        //public ActionResult Teachers(string disciplineUid)
        //{

        //    var data = TeachersData(disciplineUid)
        //        .Select(t => new { Id = t.pkey, Name = t.BigName })
        //        .ToList();

        //    return Json(
        //       new { data },
        //       JsonRequestBehavior.AllowGet
        //   );
        //}

        //public ActionResult Themes(string disciplineUid)
        //{
        //    var themes = ThemesData(disciplineUid)
        //        .Select(p => new { p.Id, p.Theme })
        //        .ToList();

        //    return Json(
        //       new { data = themes },
        //       JsonRequestBehavior.AllowGet
        //   );
        //}

    }
}