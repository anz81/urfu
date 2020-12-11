using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.Practice
{

    public class GroupViewModelRow
    {
        public int PracticeID { get; set; } 
        public int AdmissionID { get; set; }
        public int AdmissionCompanyID { get; set; }
        public string StudentID { get; set; }
        public string Name { get; set; }
        public string StudentStatus { get; set; }
        public string StudentCompensation { get; set; }
        public string PersonalNumber { get; set; }
        public string PracticeDateInfo { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalDateInfo { get; set; }

        //сроки выезда - где все это берется
        public string CompanyDateInfo { get; set; }
        public string Teacher { get; set; }
        public string Teacher2 { get; set; }
        public string Subdivision { get; set; }
        public string AdmissionStatus { get; set; }

        public string CompanyName { get; set; }
        public string AdmissionCompanyStatus { get; set; }

        public decimal PercentComplete { get; set; }
        public string PercentCompleteText { get; set; }

        public decimal PercentCompleteAdmDoc { get; set; }
        public string PercentCompleteeAdmDocText { get; set; }
        public string Variant { get; set; }

        /// <summary>
        /// Содержит договор или нет (сделано для Нечепуренко) 
        /// </summary>
        public bool ExistContract { get; set; }

        /// <summary>
        /// Студент целевик или нет (сделано для Нечепуренко) 
        /// </summary>
        public bool IsTarget { get; set; }

        /// <summary>
        /// Номер догвора (сделано для Нечепуренко) 
        /// </summary>
        public string ContractNumber { get; set; }
        
        public bool RejectionLetter { get; set; }

        public bool DocHasNoStatus { get; set; }
        public string ReportDatesInfo { get; set; }

        public string EqualsGroupDates { get; set; }
        public string EqualsGroupReportDates { get; set;}

    }

    public class GroupViewModel
    {
        public string GroupHistoryId { get; set; } 
        public string GroupId { get; set; }
        public string DisciplineUid { get; set; }

        public string GroupName { get; set; }

        public int Year { get; set; }
        public int SemesterID { get; set; }

        public PracticeTitle Title { get; set; }

        /// <summary>
        /// Показывать скрытые поля таблицы (IsTarget и ContractNumber) или нет (сделано для Нечепуренко) 
        /// </summary>
        public bool UserIsInRole { get; set; }

        public GroupViewModel(string groupId, string groupsHistoryId, string disciplineUid)
        {
            GroupId = groupId;
            GroupHistoryId = groupsHistoryId;
            DisciplineUid = disciplineUid;
        }

        public IQueryable<GroupViewModelRow> /*List<GroupViewModelRow>*/ GetRows(List<Student> students, List<PracticeDocument> documents, string groupId)
        {
            var rows = new List<GroupViewModelRow>();
            foreach (var s in students)
            {
                var practice = s.Practices.FirstOrDefault(p => p.DisciplineUUID == DisciplineUid && p.Year == Year && p.SemesterId == SemesterID && p.Group.GroupId == groupId);
              
                var admission = practice?.Admissions.OrderBy(a => a.Id).LastOrDefault();
                var admissionCompany = practice?.AdmissionCompanys.OrderBy(a => a.Id).LastOrDefault();

                var row = new GroupViewModelRow();
                row.PracticeID = practice?.Id ?? 0;
                row.StudentID = s.Id;
                row.Name = $"{s.Person.Surname} {s.Person.Name} {s.Person.PatronymicName}";
                row.StudentStatus = s.Status;
                row.StudentCompensation = s.Compensation;
                row.PersonalNumber = s.PersonalNumber;
                row.PracticeDateInfo = DateInfo(practice?.BeginDate, practice?.EndDate);
                row.IsExternal = practice?.IsExternal ?? false;
                row.ExternalDateInfo = DateInfo(practice?.ExternalBeginDate, practice?.ExternalEndDate);
                row.AdmissionID = admission?.Id ?? 0;

                row.Teacher = FullName(admission?.Teacher);
                row.Teacher2 = FullName(admission?.Teacher2);
                row.Subdivision = admission?.Subdivision;
                row.AdmissionStatus = admission?.StatusName;

                row.CompanyName = admissionCompany?.Contract.Company.Name;
                row.AdmissionCompanyStatus = admissionCompany?.StatusName;

                row.Variant = s.VariantAdmissions.OrderByDescending(_ => _.Variant.Program.Year).FirstOrDefault(v => v.Status == AdmissionStatus.Admitted)?.Variant.Name ?? "";

                row.ExistContract = practice?.ExistContract ?? false;
                row.IsTarget = practice?.Student?.IsTarget ?? false;
                row.ContractNumber = admissionCompany?.Contract?.Number;
                var count = documents.Where(d => d.PracticeId == practice?.Id).Count();
                var total = 10m;//todo надо наверное как то постчитать сколько каких документов нужно для практики 
                row.PercentComplete = count/total;
                row.PercentCompleteText = $"{Math.Round(count/total*100)}%  ({count} из {total})";
                row.RejectionLetter = documents.Where(d => d.PracticeId == practice?.Id).FirstOrDefault(l => l.DocumentType == PracticeDocumentType.Rejection) != null ? true : false;
                var countAdmdoc = documents.Where(d => d.PracticeId == practice?.Id && d.Status == AdmissionStatus.Admitted).Count();
                row.PercentCompleteAdmDoc = countAdmdoc / total;
                row.PercentCompleteeAdmDocText = $"{Math.Round(countAdmdoc / total * 100)}%  ({countAdmdoc} из {total})";
                row.DocHasNoStatus =documents.Any(d => d.PracticeId == practice?.Id && d.Status == 0);
                row.ReportDatesInfo = DateInfo(practice?.ReportBeginDate,practice?.ReportEndDate);
                var EqualsGroupDates = practice != null ? practice.TakeDatesfromGroup : false;
                var EqualsGroupReportDates = practice != null ? practice.TakeReportDatesfromGroup : false;
                row.EqualsGroupDates = EqualsGroupDates ? "Да" : "Нет";
                row.EqualsGroupReportDates = EqualsGroupReportDates ? "Да" : "Нет";
                rows.Add(row);
                
            }
            return rows.AsQueryable();

        }

        private string DateInfo(DateTime? beginDate, DateTime? endDate)
        {
            return beginDate != null || endDate != null
                    ? $"с {beginDate:dd.MM.yy} по {endDate:dd.MM.yy}"
                    : null;
        }
        
        private string FullName(Teacher t)
        {
            return t == null
                ? null
                : $"{t.lastName} {t.firstName} {t.middleName}"; 
        }

    }
}
