using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Urfu.Its.Web.DataContext;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Urfu.Its.Practices;

namespace Urfu.Its.Frames.Controllers
{
    public class Teacher2VM
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ThemaVM
    {
        public int? Id { get; set; }
        public string Thema { get; set; }
    }

    public class CompanyVM
    {
        public int ContractID { get; set; }
        public string ContractNumber { get; set; }

        public int PeriodID { get; set; }
        public string hId => $"collapse{PeriodID}";
        public string Name { get; set; }
        public int Admit { get; set; }
        public int Limit { get; set; }
        public string LimitStr { get; set; }

        public bool IsExternal { get; set; }

        public string Direction { get; set; }
        public string Comment { get; set; }

        public string PersonInCharge { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string CompanyPhone { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string Site { get; set; }
        public bool HasScan { get; set; }
        public bool HasCompanyDocument { get; set; }
        public string AdditionalTerms { get; set; }
        public string DivisionDescription { get; set; }
        public int? AdditionalFileId { get; set; }
        public string AdditionalFileName { get; set; }
        public int? AgreementFileStorageId { get; set; }
        public string GetContractNumber()
        {
            return $"Договор № {ContractNumber}";
        }

        public string ScanName()
        {
            return $"Договор № {ContractNumber?.Replace('/', '_')}.pdf";
        }

        public string GetCompanyAddressDescription()
        {
            string fullAddress = (Location != null ? Location + ", " : "") + Address ?? "";
            return $"Адрес: {fullAddress}";
        }

        public string GetCompanyPhoneDescription()
        {
            return $"Телефон: {CompanyPhone}";
        }

        public string GetPersonInChargeDescription()
        {
            return $"Ответственный: {PersonInCharge}, email:{Email}";
        }

        public string GetAdditionalTermsDescription()
        {
            return $"Дополнительные условия прохождения практики:  {(string.IsNullOrEmpty(AdditionalTerms) ? "Не указано" : AdditionalTerms)}";
        }
        public string GetDivisionDescription()
        {
            return $"Вид деятельности отдела, где будет проходить практика:  {(string.IsNullOrEmpty(DivisionDescription) ? "Не указано" : DivisionDescription)}";
        }
    }

    public class AdmissionVM
    {
        public int ID { get; set; }
        public ThemaVM Thema { get; set; }
        public Teacher2VM Teacher { get; set; }
        public Teacher2VM Teacher2 { get; set; }
        public AdmissionStatus Status { get; set; }
        public string StatusName { get; set; }
        public string ReasonOfDeny { get; set; }
        public string Subdivision { get; set; }

    }

    public class AdmissionCompanyVM
    {
        public string Company { get; set; }
        public string Site { get; set; }

        public AdmissionStatus Status { get; set; }
        public string StatusName { get; set; }
        public string ReasonOfDeny { get; set; }

        public bool IsExternal { get; set; }

        public string Direction { get; set; }
        public string Comment { get; set; }

        public string PersonInCharge { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int ContractID { get; set; }
        public string ContractNumber { get; set; }
        public string CompanyPhone { get; set; }
        public string Address { get; set; }
        public bool HasScan { get; set; }
        public bool HasCompanyDocument { get; set; }
        public bool Agreement { get; set; }
        public string AdditionalTerms { get; set; }
        public string DivisionDescription { get; set; }
        public int? AdditionalFileId { get; set; }
        public string AdditionalFileName { get; set; }
        public int? AgreementFileStorageId { get; set; }
        public bool IsShortDated { get; set; }
        public string GetContractNumber()
        {
            return $"Договор № {ContractNumber}";
        }

        public string ScanName()
        {
            return $"Договор № {ContractNumber?.Replace('/', '_')}.pdf";
        }

        public string GetCompanyDescription()
        {
            return $"Адрес: {Address}";
        }

        public string GetPersonInChargeDescription()
        {
            return $"Ответственный: {PersonInCharge}, email:{Email}";
        }
        public string GetAdditionalTermsDescription()
        {
            return $"Дополнительные условия прохождения практики:  {(string.IsNullOrEmpty(AdditionalTerms) ? "Не указано" : AdditionalTerms)}";
        }
        public string GetDivisionDescription()
        {
            return $"Вид деятельности отдела, где будет проходить практика:  {(string.IsNullOrEmpty(DivisionDescription) ? "Не указано" : DivisionDescription)}";
        }
    }

    public class PersonalContractVM
    {
        [DisplayName("Наименование предприятия")]
        [Required(ErrorMessage = "Введите название предприятия")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Недопустимая длина имени")]
        public string CompanyName { get; set; }

        [DisplayName("Сокращенное название")]
        public string ShortName { get; set; }

        [DisplayName("Адрес предприятия")]
        public string Address { get; set; }

        [DisplayName("ИНН")]
        [RegularExpression(@"^\d{10,12}", ErrorMessage = "ИНН должен содержать от 10 до 12 цифр")]
        public string INN { get; set; }

        [DisplayName("Руководитель предприятия")]
        [RegularExpression(@"^\D+$", ErrorMessage = "Используйте только символы английского и русского алфавитов")]
        public string Director { get; set; }

        [DisplayName("Фамилия и инициалы (пр: Иванов И.И.)")]
        [RegularExpression(@"^\D{1,40}$", ErrorMessage = "Используйте только символы английского и русского алфавитов (не более 40 символов)")]
        public string DirectorInitials { get; set; }

        [DisplayName("ФИО в родительном падеже")]
        [RegularExpression(@"^\D+$", ErrorMessage = "Используйте только символы английского и русского алфавитов")]
        public string DirectorGenetive { get; set; }

        [DisplayName("Должность руководителя предприятия")]
        public string PostOfDirector { get; set; }

        [DisplayName("Должность руководителя в родительном падеже")]
        public string PostOfDirectorGenetive { get; set; }

        [DisplayName("Телефон предприятия")]
        public string CompanyPhone { get; set; }

        [DisplayName("E-mail предприятия")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
        public string Email { get; set; }

        [DisplayName("Сайт")]
        public string Site { get; set; }

        [DisplayName("Ответственный от предприятия")]
        [RegularExpression(@"^\D+$", ErrorMessage = "Используйте только символы английского и русского алфавитов")]
        public string PersonInCharge { get; set; }

        [DisplayName("Фамилия и инициалы ответственного (пр: Иванов И.И.)")]
        [RegularExpression(@"^\D{1,40}$", ErrorMessage = "Используйте только символы английского и русского алфавитов (не более 40 символов)")]
        public string PersonInChargeInitials { get; set; }

        [DisplayName("Должность отвественного лица")]
        public string PostOfPersonInCharge { get; set; }

        [DisplayName("Телефон ответственного лица")]
        public string Phone { get; set; }

        [DisplayName("E-mail ответственного лица")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
        public string PersonInChargeEmail { get; set; }

        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        [DisplayName("Страна")]
        [Required(ErrorMessage = "Выберите страну")]
        public int? CountryId { get; set; }

        [DisplayName("Регион")]
        [Required(ErrorMessage = "Выберите регион")]
        public int? RegionId { get; set; }

        [DisplayName("Город")]
        [Required(ErrorMessage = "Выберите город")]
        public int? CityId { get; set; }

        [DisplayName("Расположение")]
        public string Location { get; set; }
        public int CompanyId { get; set; }
        public int PracticeID { get; set; }
        public string InstituteTitle { get; set; }
    }

    public class LocationVM
    {
        public List<SelectListItem> Items { get; set; }

        public string listId { get; set; }

        public string targetProperty { get; set; }
    }

    public class PracticeVM
    {
        private ApplicationDbContext _db;

        private Practice _practice;
        private Student _student;

        public int PracticeID { get; set; }
        public string StudentID { get; set; }
        public string GroupHistoryID { get; set; }
        public string DisciplineUUID { get; set; }

        public string DisciplineTitle { get; set; }

        public string InstituteTitle { get; set; }

        public int Year { get; set; }

        public string YearInfo => $"{Year}/{Year % 100 + 1} уч.год";

        public string PeriodInfo()
        {
            if (_practice.BeginDate != null || _practice.EndDate != null)
                return $"c {_practice.BeginDate:dd.MM.yyyy г.} по {_practice.EndDate:dd.MM.yyyy г.} ";
            else
                return "";
        }

        public bool IsEndDate()
        {
            return _practice.EndDate?.Date < DateTime.Now.Date;
        }

        public string Semester { get; set; }

        [Required(ErrorMessage = "Выберите руководителя")]
        public string TeacherID { get; set; }

        public string Teacher2ID { get; set; }

        [Required(ErrorMessage = "Выберите тему")]
        public int? ThemaID { get; set; }

        public string FinishTheme { get; set; }

        public string Subdivision { get; set; }

        public List<SelectListItem> Teachers { get; set; }
        public List<SelectListItem> Teachers2 { get; set; }

        public List<ThemaVM> Themas { get; set; }

        public AdmissionVM Admission { get; set; }

        public AdmissionCompanyVM AdmissionCompany { get; set; }

        public List<CompanyVM> Companys { get; set; }

        public PersonalContractVM PersonalContract { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Regions { get; set; }
        public List<SelectListItem> Cities { get; set; }

        public bool ShowMessage { get; set; }

        public bool Agreement { get; set; }

        public PracticeVM()
        {
        }

        public PracticeVM(ApplicationDbContext db, int practiceID)
        {
            _db = db;

            _practice = _db.Practices.FirstOrDefault(p => p.Id == practiceID);

            if (_practice == null)
                throw new Exception("Не найдена практика");

            _student = _db.Students.FirstOrDefault(s => s.Id == _practice.StudentId);

            Fill();
        }

        public PracticeVM(ApplicationDbContext db, string studentId, string groupId, int year, int semesterId, string disciplineUID)
        {
            _db = db;

            _student = _db.Students.FirstOrDefault(s => s.Id == studentId);
            //var group = _db.GroupsHistories.FirstOrDefault(g => g.Id == groupId);

            _practice = GetPractice(studentId, groupId, year, semesterId, disciplineUID);

            if (_practice == null)
                throw new Exception("Не найдена практика");

            Fill();

        }

        private void Fill()
        {
            PracticeID = _practice.Id;
            StudentID = _student.Id;
            GroupHistoryID = _practice.GroupHistoryId;
            DisciplineUUID = _practice.DisciplineUUID;

            var plan = _db.Plans.FirstOrDefault(p => p.disciplineUUID == DisciplineUUID);

            DisciplineTitle = plan.disciplineTitle;

            var direction = _db.Directions.FirstOrDefault(d => d.uid == _db.Plans.FirstOrDefault(p => p.disciplineUUID == DisciplineUUID).directionId);

            var group = _db.Groups.FirstOrDefault(g => g.Id == _practice.Group.GroupId);
            var department = _db.Divisions.FirstOrDefault(d => d.uuid == group.ManagingDivisionId);
            var departmentParent = _db.Divisions.FirstOrDefault(d => d.uuid == group.ManagingDivisionParentId);

            var instituteTitle = "";
            if (department != null)
            {
                instituteTitle = department.typeTitle == "Институт" ? department.shortTitle :
                        (departmentParent.typeTitle == "Институт" ? departmentParent.shortTitle : "");
            }
            InstituteTitle = instituteTitle;

            Year = _practice.Year;
            Semester = _practice.Semester.Name;

            Themas = GetThemes();
            Teachers = GetTeachers();

            Teachers2 = GetTeachers();

            FinishTheme = _practice.FinishTheme;



            Admission = _db.PracticeAdmissions
                .Where(a => a.PracticeId == _practice.Id)
                .ToList()
                .Select(a => new AdmissionVM
                {
                    ID = a.Id,
                    Teacher = new Teacher2VM { Id = a.TeacherPKey, Name = a.Teacher?.BigName },
                    Teacher2 = new Teacher2VM { Id = a.TeacherPKey2, Name = a.Teacher2?.BigName },
                    Thema = new ThemaVM { Id = a.PracticeThemeId, Thema = a.Theme?.Theme },
                    Status = a.Status,
                    StatusName = StatusInfo(a.Status),
                    ReasonOfDeny = a.ReasonOfDeny,
                    Subdivision = a.Subdivision
                })
                .FirstOrDefault();

            if (Admission != null)
            {
                var teacherEmail = _db.PracticeTeachers.FirstOrDefault(t => t.TeacherPKey == Admission.Teacher.Id
              && t.DisciplineUUID == DisciplineUUID && t.GroupHistoryId == GroupHistoryID
              && t.SemesterId == _practice.SemesterId && t.Year == Year)?.Email;

                if (!string.IsNullOrWhiteSpace(teacherEmail))
                    Admission.Teacher.Name += $", {teacherEmail}";

                var teacher2Email = _db.PracticeTeachers.FirstOrDefault(t => t.TeacherPKey == Admission.Teacher2.Id
                    && t.DisciplineUUID == DisciplineUUID && t.GroupHistoryId == GroupHistoryID
                    && t.SemesterId == _practice.SemesterId && t.Year == Year)?.Email;

                if (!string.IsNullOrWhiteSpace(teacher2Email))
                    Admission.Teacher2.Name += $", {teacher2Email}";
            }


            Subdivision = Admission?.Subdivision;
            ThemaID = Admission?.Thema.Id;
            TeacherID = Admission?.Teacher.Id;
            Teacher2ID = Admission?.Teacher2.Id;

            var admissionCompany = _db.PracticeAdmissionCompanys
                .Include(a => a.Contract.Company)
                .FirstOrDefault(a => a.PracticeId == _practice.Id);



            if (admissionCompany != null)
            {

                var period = admissionCompany.Contract.Periods.FirstOrDefault(p =>
                    p.Year == _practice.Year && p.SemesterId == _practice.SemesterId);

                var agreementData = _db.PracticeAgreements.FirstOrDefault(a => a.Year == _practice.Year);
                AdmissionCompany = new AdmissionCompanyVM
                {
                    Company = admissionCompany.Contract.Company.Name,
                    Site = admissionCompany.Contract.Company.Site,
                    Status = admissionCompany.Status,
                    StatusName = StatusInfo(admissionCompany.Status),
                    ReasonOfDeny = admissionCompany.ReasonOfDeny,
                    Comment = admissionCompany.Contract.Comment,
                    Direction = $"{direction.title} ({direction.okso})",

                    ContractID = admissionCompany.Contract.Id,
                    ContractNumber = admissionCompany.Contract.Number,
                    HasScan = admissionCompany.Contract.FileStorageId != null,
                    HasCompanyDocument = admissionCompany.Contract.Company.FileStorageId != null,

                    CompanyPhone = admissionCompany.Contract.Company.CompanyPhoneNumber,
                    Address = admissionCompany.Contract.Company.Address,

                    PersonInCharge = admissionCompany.Contract.PersonInCharge ?? admissionCompany.Contract.Company.PersonInCharge,
                    Phone = admissionCompany.Contract.PhoneNumber ?? admissionCompany.Contract.Company.PhoneNumber,
                    Email = admissionCompany.Contract.Email ?? admissionCompany.Contract.Company.Email,
                    AdditionalTerms = period?.AdditionalTerms,
                    DivisionDescription = period?.DivisionDescription,
                    AdditionalFileId = period?.FileStorage?.Id,
                    AdditionalFileName = period?.FileStorage?.FileNameForUser,
                    Agreement = admissionCompany.Agreement,
                    AgreementFileStorageId = agreementData?.FileStorageId,
                    IsShortDated = admissionCompany.Contract.IsShortDated
                };
            }

            if (admissionCompany == null || admissionCompany.Status == AdmissionStatus.Denied)
            {
                var directionTitle = $"{direction.title} ({direction.okso})";

                Companys = GetContracts(directionTitle, _practice?.Group?.ProfileId);
                PersonalContract = new PersonalContractVM();
            }

            Countries = new List<SelectListItem>();
            var countries = _db.CompanyLocations.Where(l => l.Level == 1).Select(l => new SelectListItem()
            {
                Text = l.Name,
                Value = l.Id.ToString()
            }).OrderBy(c => c.Text).ToList();
            countries.Insert(0, new SelectListItem { Value = null, Text = "" });
            Countries = countries;

            Regions = new List<SelectListItem>();

            Cities = new List<SelectListItem>();
        }

        private Practice GetPractice(string studentId, string groupId, int year, int semesterId, string disciplineUID)
        {
            using (var tran = _db.Database.BeginTransaction())
            {
                try
                {
                    var practice = _db.Practices.FirstOrDefault(p
                        => p.StudentId == studentId
                        && p.DisciplineUUID == disciplineUID
                        && p.GroupHistoryId == groupId
                        && p.Year == year
                        && p.SemesterId == semesterId);

                    if (practice == null)
                    {
                        var info = _db.PracticeInfo.FirstOrDefault(i => i.DisciplineUUID == disciplineUID && i.SemesterId == semesterId && i.GroupId == groupId);

                        practice = new Practice();

                        practice.StudentId = studentId;
                        practice.DisciplineUUID = disciplineUID;
                        practice.GroupHistoryId = groupId;
                        practice.Year = year;
                        practice.SemesterId = semesterId;
                        practice.BeginDate = info?.BeginDate;
                        practice.EndDate = info?.EndDate;

                        _db.Practices.Add(practice);

                        _db.SaveChanges();
                    }

                    tran.Commit();

                    return practice;
                }
                catch
                {
                    tran.Rollback();
                    return null;
                }
            }
        }

        private List<ThemaVM> GetThemes()
        {
            var list = _db.PracticeThemes
                .Where(t => t.DisciplineUUID == DisciplineUUID && t.Year == Year && t.SemesterId == _practice.SemesterId && t.GroupHistoryId == GroupHistoryID)
                .Select(t => new ThemaVM { Id = t.Id, Thema = t.Theme })
                .ToList();

            return list;
        }

        private List<SelectListItem> GetTeachers()
        {
            var list = _db.PracticeTeachers.Include("Teacher")
                .Where(t => t.DisciplineUUID == DisciplineUUID && t.Year == Year && t.SemesterId == _practice.SemesterId && t.GroupHistoryId == GroupHistoryID)
                .ToList()
                .Select(t => new SelectListItem { Value = t.TeacherPKey, Text = $"{t.Teacher.BigName}, {t.Email}" })
                .ToList();

            list.Insert(0, new SelectListItem { Value = null, Text = "" });

            return list;
        }


        private List<CompanyVM> GetContracts(string direction, string profileId)
        {
            var plan = _db.Plans.FirstOrDefault(p => p.disciplineUUID == _practice.DisciplineUUID);

            var periods = _db.ContractPeriods
                .Where(p => p.Year == _practice.Year && p.SemesterId == _practice.SemesterId && p.Contract.FinishDate > DateTime.Now && !p.Contract.IsShortDated)
                .Where(p => p.Contract.FinishDate > DateTime.Now)
                .Select(p => new
                {
                    ContractID = p.Contract.Id,
                    ContractNumber = p.Contract.Number,
                    PeriodID = p.Id,
                    Company = p.Contract.Company,
                    Comment = p.Contract.Comment,

                    ContractPersonInCharge = p.Contract.PersonInCharge,
                    ContractPhone = p.Contract.PhoneNumber,
                    ContractEmail = p.Contract.Email,

                    HasScan = (p.Contract.FileStorageId != null),
                    HasCompanyDocument = (p.Contract.Company.FileStorageId != null),

                    Limits = p.Limits.Where(l =>
                           (l.DirectionId == null || l.DirectionId == plan.directionId)
                        && (l.ProfileId == null || l.ProfileId == profileId)
                        && (l.Course == 0 || l.Course == _practice.Group.Course)
                        && (l.Qualification == null || l.QualificationName == _practice.Group.Qual)).ToList(),
                    p.AdditionalTerms,
                    p.DivisionDescription,
                    p.FileStorage,
                    p.Year
                })
                .Where(p => p.Limits.Count > 0)
                .ToList();

            var admissions = _db.PracticeAdmissionCompanys
                          .Include(a => a.Practice.Group)
                          .Where(a => a.Practice.SemesterId == _practice.SemesterId && a.Practice.Year == _practice.Year && a.Status == AdmissionStatus.Admitted
                           )// && a.Practice.DisciplineUUID == _practice.DisciplineUUID)
                          .ToList();

            var res = new List<CompanyVM>();

            foreach (var p in periods)
            {
                var limit = p.Limits.Sum(l => l.Limit);
                var limits = p.Limits
                    .OrderByDescending(l => l.Prioritet())
                    .Select(l => new PracticeLimit { Limit = l })
                    .ToList();

                int over = 0;
                foreach (var l in limits)
                {
                    l.SetAdmissions(_db, admissions);

                    over += admissions.Where(a =>
                         a.ContractId == p.ContractID
                         && (l.Limit.DirectionId == null || l.Limit.DirectionId == a.Practice.Group.Profile.DIRECTION_ID)
                         && (l.Limit.ProfileId == null || l.Limit.ProfileId == a.Practice.Group.ProfileId)
                         && (l.Limit.Course == 0 || l.Limit.Course == a.Practice.Group.Course)
                         && (l.Limit.Qualification == null || l.Limit.QualificationName == a.Practice.Group.Qual)
                    ).GroupBy(a => a.Practice.StudentId).Distinct().Count();
                }

                //var over = admissions.Where(a => a.ContractId == p.ContractID).GroupBy(a => a.Practice.StudentId).Distinct().Count();
                var total = limits.Sum(l => l.Admissions.Count());

                var agreementData = _db.PracticeAgreements.FirstOrDefault(a => a.Year == _practice.Year);
                var company = new CompanyVM
                {
                    ContractID = p.ContractID,
                    ContractNumber = p.ContractNumber,
                    CompanyPhone = p.Company.CompanyPhoneNumber,
                    Site = p.Company.Site,
                    Address = p.Company.Address,
                    Location = p.Company.Location?.FullLocation(),
                    PeriodID = p.PeriodID,
                    Direction = direction,
                    Name = p.Company.Name,
                    //Limit =  $"{total} из {p.Limit}",
                    Admit = total,
                    Limit = limit - total - over,
                    LimitStr = $"Всего {limit} Зачислено {total} + {over}",
                    Comment = p.Comment,
                    PersonInCharge = p.ContractPersonInCharge ?? p.Company.PersonInCharge,
                    Phone = p.ContractPhone ?? p.Company.PhoneNumber,
                    Email = p.ContractEmail ?? p.Company.Email,
                    HasScan = p.HasScan,
                    HasCompanyDocument = p.HasCompanyDocument,
                    AdditionalTerms = p.AdditionalTerms,
                    DivisionDescription = p.DivisionDescription,
                    AdditionalFileId = p.FileStorage?.Id,
                    AdditionalFileName = p.FileStorage?.FileNameForUser,
                    AgreementFileStorageId = agreementData?.FileStorageId,
                };

                res.Add(company);
            }


            var res2 = res.Where(r => r.Limit > 0).OrderBy(r => r.Name).ToList();
            res2.AddRange(res.Where(r => r.Limit == 0).OrderBy(r => r.Name));

            return res2;
        }

        private static string StatusInfo(AdmissionStatus status)
        {
            switch (status)
            {
                case AdmissionStatus.Admitted: return "согласовано";
                case AdmissionStatus.Denied: return "отклонена";
                case AdmissionStatus.Indeterminate: return "на расcмотрении";
            }

            return status.ToString();
        }
    }

    public class Message
    {
        public string Text { get; set; }

        public Message(string text)
        {
            Text = text;
        }
    }
}