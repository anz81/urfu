using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using TemplateEngine;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.Practice;
using Microsoft.AspNetCore.Http;


namespace Urfu.Its.Practices
{
    public class PracticeDocuments
    {
        public MemoryStream GetDocuments(PracticeDocumentType type, int practiceId)
        {
            var des = PracticFileDescriptor.Get(type);

            var model = GetModel(type, practiceId);
            var hi = new HostingEnvironment();

            var fullName = Path.Combine(hi.ContentRootPath, @"Content\Practice", des.TemplateName);

            if (model == null)
            {
                var bytes = File.ReadAllBytes(fullName);
                var output = new MemoryStream(bytes);
                return output;
            }
            else
            {
                using (var input = File.Open(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var output = new MemoryStream();
                    var engine = new WordDocxTemplateReportingEngine();
                    engine.Build(input, model, output, FileFormat.Docx);

                    output.Position = 0;
                    return output;
                }
            }
        }

        private object GetModel(PracticeDocumentType type, int practiceId)
        {
            switch (type)
            {
                case PracticeDocumentType.Contract: return GetModelContract(practiceId);
                case PracticeDocumentType.Notice: return GetModelContract(practiceId);
                case PracticeDocumentType.Postponement: return GetModelContract(practiceId);
                case PracticeDocumentType.Referral: return GetModelContract(practiceId);
            //    //case PracticeDocumentType.Rejection: return GetModelContract(practiceId);
                case PracticeDocumentType.Report: return GetModelContract(practiceId);
                case PracticeDocumentType.Resume: return GetModelContract(practiceId);
                case PracticeDocumentType.Review: return GetModelContract(practiceId);
                case PracticeDocumentType.Task: return GetModelContract(practiceId);
                case PracticeDocumentType.Trip: return GetModelContract(practiceId);
                case PracticeDocumentType.DistantContract: return GetModelContract(practiceId);
                case PracticeDocumentType.DistantReferral: return GetModelContract(practiceId);
                case PracticeDocumentType.DistantTask: return GetModelContract(practiceId);
            }
            return null;
        }

        private object GetModelContract(int practiceId)
        {
            using (var db = new ApplicationDbContext())
            {
                var practice = db.Practices
                    .Include(p => p.AdmissionCompanys)
                    .Include(p => p.Student.Person)
                    .Include(p => p.Group)
                    .FirstOrDefault(p => p.Id == practiceId);

                var admission = db.PracticeAdmissions
                    .Include(a=>a.Teacher)
                    .Include(a => a.Theme)
                    .FirstOrDefault(a => a.PracticeId == practiceId && a.Status == AdmissionStatus.Admitted);
                
                var admissionCompany = practice.AdmissionCompanys.FirstOrDefault(a => a.Status == AdmissionStatus.Admitted);

                var contractId = admissionCompany?.ContractId ?? 0;

                var contract = db.Contracts
                    .Include(c=>c.Company)
                    .FirstOrDefault(c=>c.Id == contractId);

                var plan = db.Plans
                    .FirstOrDefault(p=>p.disciplineUUID == practice.DisciplineUUID);

                var planTmers = db.PlanTerms.Where(pt => pt.eduplanUUID == plan.eduplanUUID).ToList();
                var planTermWeek = db.PlanTermWeeks.Where(ptw => ptw.eduplanUUID == plan.eduplanUUID).ToList();

                var direction = db.Directions.FirstOrDefault(d => d.uid == plan.directionId);

                var chair = db.Divisions.FirstOrDefault(d => d.uuid == practice.Group.Profile.CHAIR_ID);
                var department = db.Divisions.FirstOrDefault(d => d.uuid == practice.Group.ManagingDivisionId);
                var institute = department;
                if (institute.typeTitle == "Департамент")
                {
                    var _institute = db.Divisions.FirstOrDefault(d => d.uuid == institute.parent && d.typeTitle == "Институт");
                    institute = _institute != null ? _institute : institute;
                }
                
                var decree = db.PracticeDecreeNumbers.FirstOrDefault(d => d.Year == practice.Year);

                var semesters = db.Semesters.ToList();
                var vm = new PlanViewModel(plan, planTmers, semesters);
                var courseTerms = vm.PlanTerms.Where(t => t.Course == practice.Group.Course).ToList();
                var units = courseTerms.Select(t => plan.GetTermTestUnits(t.Term)).ToList();
                //var theme = (practice.FinishTheme != null && practice.FinishTheme?.Length > 0) ? practice.FinishTheme : admission?.Theme.Theme;
                var theme = admission?.Theme.Theme;

                string practicePlace = "";

                if (contract?.Company != null)
                {
                    practicePlace = contract?.Company.Name;

                    if (contract.Company.Location?.Level == 3) // указан город
                    {
                        var region = contract.Company.Location.Parent;
                        var country = region?.Parent;

                        if (country?.Name != null) practicePlace += ", " + country.Name;
                        if (region?.Name != null) practicePlace += ", " + region.Name;
                        practicePlace += ", " + contract.Company.Location.Name;
                    }
                    else if (contract.Company.Location?.Level == 2) // указан регион
                    {
                        var country = contract.Company.Location.Parent;

                        if (country?.Name != null) practicePlace += ", " + country.Name;
                        practicePlace += ", " + contract.Company.Location.Name;
                    }
                    else if (contract.Company.Location?.Level == 1) // указана страна
                    {
                        practicePlace += ", " + contract.Company.Location.Name;
                    }
                    practicePlace += contract.Company.Address != null ? ", " + contract.Company.Address : "";
                }
                else
                {
                    if (!string.IsNullOrEmpty(admission?.Subdivision))
                    {
                        practicePlace = admission.Subdivision;
                    }
                    else
                    {
                        practicePlace = $"{chair.typeTitle} {chair.shortTitle}";
                    }
                }

                var director = db.Directors.FirstOrDefault(d => d.DivisionUuid == institute.uuid);

                var practiceInfo = db.PracticeInfo.FirstOrDefault(p => p.DisciplineUUID == practice.DisciplineUUID &&
                  p.GroupId == practice.Group.Id && p.SemesterId == practice.SemesterId);

                DateTime? contractdate= null;
                if (contract != null)
                {
                    if (contract.IsShortDated)
                    {
                        contractdate = PracticePeriodModel.GetDates(admissionCompany.Dates).Count != 0 ? PracticePeriodModel.GetDates(admissionCompany.Dates)[0].BeginDate :
                            (practice.BeginDate == null ? practiceInfo.BeginDate :practice.BeginDate);
                    }
                    else
                           contractdate =contract?.ContractDate;
                }

                var practicestartdate = practice?.BeginDate ?? practiceInfo?.BeginDate;
                var letterofattorney = db.LettersOfAttorney.Where(d => d.StartDate <= practicestartdate && d.EndDate >= practicestartdate).FirstOrDefault();

                var teacherEmail = admission?.Teacher?.Practices?.FirstOrDefault(t =>
                    t.DisciplineUUID == practice.DisciplineUUID && t.GroupHistoryId == practice.GroupHistoryId
                    && t.SemesterId == practice.SemesterId && t.Year == practice.Year)?.Email;

                var model = new PracticeDocumentContract
                {
                    ContractNumber = contract?.Number,
                    ContractDate = $"{contractdate:dd MMMM yyyy}",
                    Day = $"{contractdate:dd}",
                    Month = (contractdate != null) ? $"{contractdate:dd MMMM yyyy}".Split(' ')[1] :"",
                    Year = $"{contractdate:yyyy}",
                    NumberLetterOfAttorney = letterofattorney?.Number,
                    DateLetterOfAttorney =  $"{letterofattorney?.StartDate:dd.MM.yyyy}",
                    CompanyName = contract?.Company.Name,
                    City = contract?.Company.Location?.Name,
                    PostGenitive = contract?.PostOfDirectorGenitive ?? contract?.Company.PostOfDirectorGenitive,
                    Director = contract?.Director ?? contract?.Company.Director,
                    DirectorInitials = contract?.DirectorInitials ?? contract?.Company.DirectorInitials,
                    DirectorGenitive = contract?.DirectorGenitive ?? contract?.Company.DirectorGenitive,
                    INN = contract?.Company.INN,
                    Address = contract?.Company.Address,
                    CompanyPhoneNumber = contract?.Company.CompanyPhoneNumber,
                    CompanyEmail = contract?.Company.Email,

                    PostOfPersonInCharge = contract?.PostOfPersonInCharge ?? contract?.Company.PostOfPersonInCharge,
                    PersonInCharge = contract?.PersonInCharge ?? contract?.Company.PersonInCharge,
                    PersonInChargeInitials = contract?.PersonInChargeInitials ?? contract?.Company.PersonInChargeInitials,
                    PhoneNumber = contract?.PhoneNumber ?? contract?.Company.PhoneNumber,

                    Email = contract?.Email ?? contract?.Company.Email,

                    Course = practice.Group.Course.ToString(),
                    Semester = practice.Semester.Name,

                    Group = practice.Group.Name,
                    Direction = direction.title,
                    OKSO = direction.okso,
                    Profile = practice.Group.Profile?.OksoAndTitle,

                    Institute = institute?.shortTitle,
                    Department = department?.shortTitle,
                    DepartmentFullTitle = department?.title,
                    Chair = chair?.title,

                    DirectorOfInstitute = director?.ShortName(),

                    Student = practice.Student.Person.FullName(),
                    StudentShort = practice.Student.Person.ShortName(),
                    StudentName = practice.Student.Person.Name,
                    StudentSurname = practice.Student.Person.Surname,
                    StudentPatronymicName = practice.Student.Person.PatronymicName,
                    StudentPhone = practice.Student.Person.Phone,
                    StudentEmail = practice.Student.Person.EMail,
                    StudentDateOfBirth = $"{practice.Student.Person.DateOfBirth:dd.MM.yyyyг.}",
                    StudentAge = practice.Student.Person.Age().ToString(),
                    Compensation = practice.Student.Compensation + (practice.Student.IsTarget ? " целевой" : ""),
                    Qualification = plan.qualification,

                    FamilirizationTech = plan.familirizationTech,
                    FamilirizationType = plan.familirizationType,
                    FamilirizationCondition = plan.familirizationCondition,

                    LearnProgramCode = plan.learnProgramCode,
                    LearnProgramTitle = plan.learnProgramTitle,

                    StartDate = $"{practice.BeginDate:dd.MM.yyyyг.}",
                    FinishDate = $"{practice.EndDate:dd.MM.yyyyг.}",
                    StartDate2 = $"{practice.BeginDate:dd MMMM yyyyг.}",
                    FinishDate2 = $"{practice.EndDate:dd MMMM yyyyг.}",

                    StudyYear = practice.Group.YearHistory.ToString() + "/" + (practice.Group.YearHistory + 1).ToString(),

                    PracticeName = plan.disciplineTitle,
                    PracticeType = plan.additionalType,
                    PracticePlace = practicePlace,
                    TestUnits = units.Sum().ToString(),

                    Theme = theme,
                    FinishTheme = practice.FinishTheme,
                    Teacher = admission?.Teacher?.initials,
                    TeacherFullName = admission?.Teacher?.FullName,
                    TeacherPost = admission?.Teacher?.post,
                    TeacherEmail = teacherEmail ?? "",

                    DecreeNumber = decree?.Number,
                    DecreeDate = $"{decree?.DecreeDate:dd.MM.yyyyг.}",
                    
                    ReportStartDate = $"{practice.ReportBeginDate:dd.MM.yyyyг.}",
                    ReportFinishDate = $"{practice.ReportEndDate:dd.MM.yyyyг.}",

                    // поля для отзыва по практике
                    Events = practice.Review?.Events ?? "",
                    Description = practice.Review?.Description ?? "",
                    Employment = practice.Review?.Employment != null 
                                    ? practice.Review.Employment ? "да" : "нет"
                                    : "",
                    FuturePractice = practice.Review?.FuturePractice != null
                                    ? practice.Review.FuturePractice ? "да" : "нет"
                                    : "",
                    FutureEmployment = practice.Review?.FutureEmployment != null
                                    ? practice.Review.FutureEmployment ? "да" : "нет"
                                    : "",
                    Suggestions = practice.Review?.Suggestions ?? "",
                    Score = practice.Review?.Score.ToString() ?? "",
                };

                return model;
            }
        }
    }
}
