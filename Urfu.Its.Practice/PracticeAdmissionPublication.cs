using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.Integration.ApiModel;
using Urfu.Its.Integration.Queues;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Common;
using Urfu.Its.Web.Model.Models.Practice;

namespace Urfu.Its.Web.Model
{
    public class PracticeAdmissionPublication
    {
        public static void PublishPracticeAdmission(int practiceId, int contractId)
        {
            try
            {
                Logger.Info($"Отправка зачисления студента в ЛКП practiceId={practiceId}");
                var dto = GetPracticeAdmission(practiceId, contractId);
                if (dto != null)
                {
                    Logger.Info($"Подготовлен пакет зачисления студента в ЛКП practiceId={practiceId}");
                    PersonalCabinetService.PostPracticeAdmission(dto);
                    Logger.Info($"Отправка зачисления студента в ЛКП завершена practiceId={practiceId}");
                }

            }
            catch (Exception ex)
            {
                Logger.Info($"Ошибка при формировании пакета зачисления студента в ЛКП practiceId={practiceId}");
                Logger.Error(ex);
            }
        }

        private static PracticeAdmissionMqDto GetPracticeAdmission(int practiceId, int contractId)
        {
            using (var db = ApplicationDbContext.Create())
            {
                var practice = db.Practices.FirstOrDefault(p => p.Id == practiceId);

                if (practice == null) return null;

                var admissionCompany = practice.AdmissionCompanys.Where(c => c.ContractId == contractId)
                    .OrderByDescending(a => a.Id).FirstOrDefault();

                if (admissionCompany == null || admissionCompany.Contract.IsShortDated || !admissionCompany.Agreement) return null;

                var admissions = practice.Admissions.OrderByDescending(a => a.Id).FirstOrDefault();

                var discipline = db.Plans.FirstOrDefault(p => p.disciplineUUID == practice.DisciplineUUID);

                var limit = admissionCompany.Contract.Periods
                    .FirstOrDefault(p => p.Year == practice.Year && p.SemesterId == practice.SemesterId)?
                    .Limits.FirstOrDefault(l =>
                        (l.DirectionId == null || l.DirectionId == discipline?.directionId)
                        && (l.ProfileId == null || l.ProfileId == practice.Group.ProfileId)
                        && (l.Course == 0 || l.Course == practice.Group.Course)
                        && (l.Qualification == null || l.QualificationName == practice.Group.Qual)
                    );

                var unitsByTerm = discipline?.testUnitsByTerm == "null"
                    ? null
                    : JsonConvert.DeserializeObject<Dictionary<int, int>>(discipline.testUnitsByTerm);

                var term = db.PlanDisciplineTerms.FirstOrDefault(t =>
                  t.DisciplineUUID == practice.DisciplineUUID && t.Course == practice.Group.Course &&
                  t.SemesterID == practice.SemesterId)?.Term;

                int unit = 0;
                if (unitsByTerm != null && term.HasValue)
                {
                    unitsByTerm.TryGetValue((int)term, out unit);
                }
                var dates = new List<PracticePeriodModelMqDto>();
                var admissionsDates = PracticePeriodModel.GetDates(admissionCompany.Dates);
                if (admissionsDates.Count != 0)
                {
                    admissionsDates.ForEach(d =>
                            dates.Add(new PracticePeriodModelMqDto(d.BeginDate, d.EndDate))
                        );
                }
                else
                {
                    dates.Add(new PracticePeriodModelMqDto(practice.BeginDate, practice.EndDate));

                }

                return new PracticeAdmissionMqDto
                {
                    StudentId = practice.StudentId,
                    Surname = practice.Student.Person.Surname,
                    Name = practice.Student.Person.Name,
                    PatronymicName = practice.Student.Person.PatronymicName,
                    GroupId = practice.Group.GroupId,
                    GroupName = practice.Group.Name,
                    Course = practice.Group.Course,
                    PracticeId = practice.Id,
                    LimitId = limit.Id,
                    PracticeDates = dates,
                    Units = unit,
                    PracticeType = discipline?.additionalType,
                    PracticeTitle = discipline?.disciplineTitle,
                    Theme = !string.IsNullOrEmpty(practice.FinishTheme)
                        ? practice.FinishTheme
                        : admissions?.Theme?.Theme,
                    Status = (int)admissionCompany.Status,
                    Agreement = admissionCompany.Agreement,
                };

            }

        }



    }
}
