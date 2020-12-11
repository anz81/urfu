using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Its.Web.Model.Models.Practice
{
    public class PracticeInfoModel
    {
        //для СЭД
        public string PrcaticeUUID { get; set; }

        //Вид практики
        public string PracticeName { get; set; }
        //Тип практики
        public string PracticeType { get; set; }
        //з.ед
        public int Units { get; set; }
        public int? Weeks { get; set; }

        public string BeginDate { get; set; }
        public string EndDate { get; set; }

        //частично рассредоточенная,   рассредоточенная ,непрерывная
        public string PracticeTime { get; set; }

        public string PracticeWay { get; set; }

        public string Semester { get; set; }
        public string StudyYear { get; set; }

        //для шаблона
        public string Standard { get; set; }

    }

    /// <summary>
    /// Используется для чтения сериализованных дат в PracticeAdmissionCompanys и PracticeAdmissions,
    /// которые были сохранены в формате Start-Finish
    /// </summary>
    public class PracticePeriodModelOld
    {
        public DateTime? Start { get; set; }
        public DateTime? Finish { get; set; }
    }

    public class PracticePeriodModel
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        [JsonIgnore]
        public string StartStr => BeginDate?.ToShortDateString() ?? "";


        [JsonIgnore]
        public string FinishStr => EndDate?.ToShortDateString() ?? "";

        public PracticePeriodModel()
        {

        }

        public PracticePeriodModel(DateTime? beginDate, DateTime? endDate)
        {
            BeginDate = beginDate;
            EndDate = endDate;
        }

        public static List<PracticePeriodModel> GetDates(List<DateTime> dateTimes)
        {
            if (dateTimes.Count != 0)
            {
                var dates = new List<PracticePeriodModel>();
                for (int i = 0; i < dateTimes.Count(); i++)
                {
                    if (i % 2 == 0 && i + 1 < dateTimes.Count())
                    {
                        dates.Add(new PracticePeriodModel(dateTimes[i], dateTimes[i + 1]));
                    }
                }
                return dates;
            }

            return new List<PracticePeriodModel>();
        }

        public static List<PracticePeriodModel> GetDates(string json)
        {
            var dates = new List<PracticePeriodModel>();
            try
            {
                if (!string.IsNullOrWhiteSpace(json) && json.Contains("Start"))
                {
                    // если сериализованные даты старого формата
                    var oldDates = JsonConvert.DeserializeObject<List<PracticePeriodModelOld>>(json);
                    foreach (var o in oldDates)
                    {
                        if (o.Start != null && o.Finish != null)
                            dates.Add(new PracticePeriodModel()
                            {
                                BeginDate = o.Start,
                                EndDate = o.Finish
                            });
                    }
                }
                else
                {
                    dates = JsonConvert.DeserializeObject<List<PracticePeriodModel>>(json);
                }
            }
            catch { }
            return dates ?? new List<PracticePeriodModel>();
        }

        /// <summary>
        /// Возвращает json строку с датами
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        public static string GetDatesJson(List<DateTime> dates)
        {
            return JsonConvert.SerializeObject(GetDates(dates));
        }
    }

    public class PracticeAddressAndDates
    {
        //в том числе Кафедра ВИШ, г.Екатеринбург
        public string Address { get; set; }
        public PracticePeriodModel PracticePeriod { get; set; }

        public string PracticePeriodStr => $"{PracticePeriod.BeginDate:dd.MM.yy} - {PracticePeriod.EndDate:dd.MM.yy}";
    }

    public class PracticeStudentModel
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Budget { get; set; }
        public string IsTarget { get; set; }

        public string ContractNumber { get; set; }
        
        public List<PracticeAddressAndDates> AddressAndDates { get; set; }
        public string Teachers { get; set; }

        /// <summary>
        /// Причина во изменения приказа, если она есть
        /// </summary>
        public string Reason { get; set; }

        public List<PracticePeriodModel> PracticeExternalPeriod { get; set; }

        public string PracticeExternalPeriodStr
        {
            get
            {
                string result = "";
                foreach (var period in PracticeExternalPeriod)
                {
                    if (period.BeginDate == null && period.EndDate == null)
                        result += "Без выезда" + "\n";
                    else
                        result += $"{period.BeginDate:dd.MM.yy} - {period.EndDate:dd.MM.yy}" + "\n";
                }

                return result;
            }
        }
         public string RecoveryDate { get; set; }
      
      }

    public class PracticeOrderModel
    {
        public string Number { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime? Date { get; set; }

        public string ChangedDecreeNumber { get; set; }
        public string ChangedDecreeDay { get; set; }
        public string ChangedDecreeMonth { get; set; }
        public string ChangedDecreeYear { get; set; }
        public DateTime? ChangedDecreeDate { get; set; }

        public string GroupName { get; set; }
        public string Departament { get; set; }
        public string Institute { get; set; }

        public string InstituteDirector { get; set; }

        //Код , наименование направления
        public string OKSO { get; set; }

        //Наименование образовательной программы/ Наименование магистерской программы
        public string ProgramType { get; set; }
        public string ProgramName { get; set; }

        public List<PracticeInfoModel> Infos { get; set; }

        public string PracticeWay { get; set; }

        public List<PracticeStudentModel> Students { get; set; }

        public int StudentTotal { get; set; }
        public int BudgetTotal { get; set; }
        public int ContractTotal { get; set; }
        public int TargetTotal { get; set; }

        public int Ekaterinburg { get; set; }
        public int OtherCity { get; set; }
        public int Sverdlovsk { get; set; }
        public int OtherCountry { get; set; }

        //для СЭД
        public string GroupId { get; set; }
        public int Term { get; set; }
        public string ProfileUUID { get; set; }
        public string ProfileName { get; set; }
        public string InstututeUUID { get; set; }
        public string Supervisiors { get; set; }
        public string SemesterName { get; set; }
        public int EduYear { get; set; }

        public string ExecutorName { get; set; }
        public string ExecutorPhone { get; set; }
        public string ExecutorEmail { get; set; }

        public string ROPInitials { get; set; }
    }

    public class PracticeOrderViewModel
    {
        public int OrderId { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Group { get; set; }
        public string DisciplineUID { get; set; }
        public int Term { get; set; }
        public int SemesterID { get; set; }
        public int? PracticeDecreeID { get; set; }

        public string DisciplineTitle { get; set; }

        /// <summary>
        /// Название дисциплины (plan.disciplineTitle)
        /// </summary>
        public string Name => DisciplineTitle + (IsRemovedDiscipline ? " (Удаленная дисциплина)" : "");

        public string PracticeType { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string Comment { get; set; }
        public int SedOp { get; set; }  //0 не показывать, 1 - отправить в сэд, 2 - статус из СЭД
        public string OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string GroupYear { get; set; }

        /// <summary>
        /// Id исторической группы (groupHistory.Id)
        /// </summary>
        public string GroupId { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Название семестра
        /// </summary>
        public string Semester { get; set; }
        public string InstituteId { get; set; }
        public string DirectionId { get; set; }
        public string ExecutorName { get; set; }
        public string ExecutorPhone { get; set; }
        public string ExecutorEmail { get; set; }
        public string ROPInitials { get; set; }
        public bool IsOldPlanVersion { get; set; }
        public int PlanNumber { get; set; }
        public int PlanVersion { get; set; }
        public string PlanStatus { get; set; }
        public string PlanNumberAndVersion
        {
            get
            {
                string planNumberAndVersion = $"{PlanNumber} ({PlanVersion})";
                return IsOldPlanVersion ? $"Старая версия ({planNumberAndVersion})" : $"{PlanStatus} {planNumberAndVersion}";
            }
        }

        public string DateExportToSed { get; set; }
        public int? SedId { get; set; }
        public bool IsRemovedDiscipline { get; set; }

        public string GroupUuid { get; set; }
    }

    public class ChangedDecreesViewModel : PracticeOrderViewModel
    {
        public int? MainDecreeId { get; set; }
        public int? MainDecreeSedId { get; set; }
        
        public string DecreeNumber { get; set; }
        public DateTime? DecreeDate { get; set; }
        public string DecreeDateStr => DecreeDate.HasValue ? DecreeDate.Value.ToShortDateString() : "";
    }

    public struct StudentsInfo
    {
        public string Id { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public int? ReasonId { get; set; }
    }
}
