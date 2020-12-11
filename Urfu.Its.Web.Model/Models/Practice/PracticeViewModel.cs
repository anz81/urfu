using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models.Practice
{
    public class PracticeDocumentViewModel
    {
        public PracticeDocumentViewModel(PracticFileDescriptor f)
        {
            DocumentType = f.Type;
            DocumentTypeName = f.TypeName;
            TemplateName = f.TemplateName;
        }

        public PracticeDocumentType DocumentType { get; set; }

        public string DocumentTypeName { get; set; }

        public string TemplateName { get; set; }

        public int DocumentId { get; set; }

        public string DocumentName { get; set; }

        public AdmissionStatus Status { get; set; }

        public string StatusName => GetStatus(Status);
        
        public string Comment { get; set; }

        public string Date { get; set; }

        public static string GetStatus(AdmissionStatus status)
        {
            switch (status)
            {
                case AdmissionStatus.Indeterminate: return "";
                case AdmissionStatus.Admitted: return "Согласовано";
                case AdmissionStatus.Denied: return "Отклонено";
            }
            return "";
        }
    }

    public class PracticeContractDsViewModel
    {
        public int practiceId { get; set; }
        public int? contractId { get; set; }


        public int limit { get; set; }
        public string company { get; set; }
        public string name => !string.IsNullOrEmpty(company) ? company + " Лимит: " + limit : "";
        public string address { get; set; }
        public string phone { get; set; }
        public string site { get; set; }
        public string personInChargeInfo { get; set; }

        public string contractNumber { get; set; }

        public AdmissionStatus? status { get; set; }
        //public bool IsAdmitted=> status != null && status.Value == AdmissionStatus.Admitted;
        public string reasonOfDeny { get; set; }
        public List<PracticePeriodModel> Dates { get; set; }
        public string divisionDescription { get; set; }

        public string additionalTerms { get; set; }
        public int? fileId { get; set; }
        public string fileName { get; set; }
    }

   

    public class PracticeContractKsViewModel
    {
        public int practiceId { get; set; }
        public int contractId { get; set; }
        public string company { get; set; }
        public int? companyId { get; set; }
        public string shortname { get; set; }
        public string companyPhone { get; set; }
        public int? countryId { get; set; }
        public int? regionId { get; set; }
        public int? cityId { get; set; }
        public string address { get; set; }
        public string inn { get; set; }
        public string director { get; set; }
        public string directorInitials { get; set; }
        public string directorGenetive { get; set; }
        public string postOfDirector { get; set; }
        public string postOfDirectorGenetive { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string personInCharge { get; set; }
        public string personInChargeInitials { get; set; }
        public string postPersonInCharge { get; set; }

        public string personInChargeEmail { get; set; }
        public string site { get; set; }
        public string comment { get; set; }

        public AdmissionStatus? status { get; set; }
        public bool IsAdmitted => status != null && status.Value == AdmissionStatus.Admitted;
        public string statusName { get; set; }
        public string reasonOfDeny { get; set; }
        public List<PracticePeriodModel> Dates { get; set; }
    }

    public class PracticeUrfuViewModel
    {
        public int practiceId { get; set; }
        public string TeacherId { get; set; }
        public string TeacherId2 { get; set; }
        public string Subdivision { get; set; }
        public int ThemeId { get; set; }
        public string FinishTheme { get; set; }
        public AdmissionStatus? Status { get; set; }
        public string Reason { get; set; }
        public bool IsAdmitted => Status != null && Status.Value == AdmissionStatus.Admitted;
        public List<PracticePeriodModel> Dates { get; set; }
    }
    
    public class PracticeTitle
    {
        public string Semester { get; set; }

        /// <summary>
        /// Учебный год в формате 2017/2018
        /// </summary>
        public string YearsStr { get; set; }

        public string PlanDisciplineTitle { get; set; }
        public string PlanAdditionalType { get; set; }

        public string Title=> Semester + ", " + YearsStr + ", " + PlanDisciplineTitle + ", " + PlanAdditionalType;
    
        public PracticeTitle(string semester, string year, string planDisciplineTitle, string planAdditionalType, bool isRemovedDiscipline)
        {
            Semester = semester;
            YearsStr = year;
            PlanDisciplineTitle = planDisciplineTitle + (isRemovedDiscipline ? " (Удаленная дисциплина)" : "");
            PlanAdditionalType = planAdditionalType;
        }
    }

    public class PracticeViewModel
    {
        public int Id { get; set; }

        public string DisciplineUID { get; set; }
        public int Year { get; set; }
        public int SemesterID { get; set; }

        public string PracticeInfo { get; set; }

        public PracticeTitle Title { get; set; }

        public string InstituteTitle { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public bool IsExternal { get; set; }
        public bool IsStationary { get; set; }

        public DateTime? ExternalStartDate { get; set; }

        public DateTime? ExternalFinishDate { get; set; }

        public PracticeUrfuViewModel Urfu { get; set; }

        public PracticeContractDsViewModel ContractDs { get; set; }

        public PracticeContractKsViewModel ContractKs { get; set; }

        public List<TeachersVM> Teachers { get; set; }

        public List<PracticeTheme> Themes { get; set; }

        public List<PracticeContractDsViewModel> Contracts { get; set; }

        public List<PracticeDocumentViewModel> Before { get; set; }

        public List<PracticeDocumentViewModel> After { get; set; }
        public List<PracticeDocumentViewModel> Distant { get; set; }

        public DateTime? ReportStartDate { get; set; }
        public DateTime? ReportFinishDate { get; set; }

        public bool EqualsGroupDates { get; set;}
        public bool EqualsGroupReportDates { get; set; }

    }
}
