using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Processors
{
    public class TimeDistributuionsProcessor : IBlockContentProcessor
    {
        private readonly IVersionedDocumentInspector _inspector;
        private readonly ApplicationDbContext _db;
        private readonly JObject _actualDocumentData;
        private readonly Web.DataContext.Module _module;
        private readonly DisciplineWorkingProgram _dwp;

        public TimeDistributuionsProcessor(IVersionedDocumentInspector inspector, ApplicationDbContext db, JObject actualDocumentData, Web.DataContext.Module module, DisciplineWorkingProgram dwp)
        {
            _inspector = inspector;
            _db = db;
            _actualDocumentData = actualDocumentData;
            _module = module;
            _dwp = dwp;
        }

        public JToken ProcessContent(JToken data)
        {
            var items = (JArray) data;

            var documentSections = (JArray) _actualDocumentData[nameof(DisciplineWorkingProgramFgosVoSchemaModel.Sections)];
            var fdps = _actualDocumentData[nameof(DisciplineWorkingProgramFgosVoSchemaModel.Fdps)];

            foreach (var item in items)
            {
                var fdpId = item[nameof(FdpTimeDistributionInfo.FdpId)].Value<string>();
                var fdp = fdps.First(f => f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() == fdpId);
                var disciplineId = fdp.Value<string>(nameof(FamilirizationTypeDirectionPlanInfo.DisciplineId));
                var plan = _db.GetDisciplinePlan(disciplineId, _dwp.Discipline.title);
                var planId = plan.disciplineUUID;
                var planAdditional = _db.PlanAdditionals.FirstOrDefault(p => p.disciplineUUID == planId);

                var sections = (JArray) item[nameof(FdpTimeDistributionInfo.Sections)];

                var newSectionIds = new List<string>();                
                foreach (var section in sections)
                {
                    var sectionId = section[nameof(TimeDistributionSectionInfo.SectionId)].Value<string>();

                    var sourceSection = documentSections.FirstOrDefault(s => s[nameof(DisciplineSectionInfo.ItemId)].Value<string>() == sectionId);
                    if (sourceSection == null)
                    {
                        _inspector.Error($"Раздел с идентификатором '{sectionId}' не найден в документе.");
                        _inspector.StopProcessing();
                    }

                    section[nameof(TimeDistributionSectionInfo.SectionCode)] = sourceSection[nameof(DisciplineSectionInfo.Code)].Value<string>();
                    section[nameof(TimeDistributionSectionInfo.SectionName)] = sourceSection[nameof(DisciplineSectionInfo.Name)].Value<string>();

                    if (newSectionIds.Contains(sectionId))
                    {
                        _inspector.Error($"Дублирование раздела '{section[nameof(TimeDistributionSectionInfo.SectionCode)].Value<string>()}'.");
                        _inspector.StopProcessing();
                    }

                    newSectionIds.Add(sectionId);                        
                }                

                // TODO Наверное, нужно еще проверять что все разделы добавлены                   

                var sectionsTotalTime = sections.Select(s => s[nameof(TimeDistributionSectionInfo.TotalTime)]).Sum(s => s.Value<decimal?>());
                var sectionsTotalAuditoryTime = sections.Select(s => s[nameof(TimeDistributionSectionInfo.TotalAuditoryTime)]).Sum(s => s.Value<decimal?>());
                var sectionsTotalHomeworkTime = sections.Select(s => s[nameof(TimeDistributionSectionInfo.TotalHomeworkTime)]).Sum(s => s.Value<decimal?>());
                var totalKmsTime = new[]
                {
                    item[nameof(FdpTimeDistributionInfo.TestTime)].Value<decimal?>(),
                    item[nameof(FdpTimeDistributionInfo.ExamTime)].Value<decimal?>()
                }.Sum();

                var totalTime = planAdditional.allload;
                var totalAuditoryTime = planAdditional.allaudit;
                var totalHomeworkTime = planAdditional.self;

                if (totalTime != sectionsTotalTime + totalKmsTime)
                {
                    _inspector.Warning($"Итоговая нагрузка по дисциплине [{totalTime}] должна соответствовать сумме [{sectionsTotalTime + totalKmsTime}] введенных данных по разделам/темам [{sectionsTotalTime}] и введенных данных по промежуточной аттестации [{totalKmsTime}].");
                }

                if (totalAuditoryTime != sectionsTotalAuditoryTime)
                {
                    _inspector.Warning(
                        $"Итоговая аудиторная нагрузка по дисциплине [{totalAuditoryTime}] должна соответствовать сумме введенных данных аудиторной нагрузки по разделам/темам [{sectionsTotalAuditoryTime}].");
                }

                if (totalHomeworkTime != sectionsTotalHomeworkTime + totalKmsTime)
                {
                    _inspector.Warning(
                        $"Итоговая нагрузка самостоятельных работ студентов по дисциплине [{totalHomeworkTime}] должна соответствовать сумме [{sectionsTotalHomeworkTime + totalKmsTime}] введенных данных самостоятельной работы студентов по разделам/темам [{sectionsTotalHomeworkTime}] и введенных данных по промежуточной аттестации [{totalKmsTime}].");
                }

                item[nameof(FdpTimeDistributionInfo.ModuleUnits)] = _module.testUnits;
                item[nameof(FdpTimeDistributionInfo.DisciplineUnits)] = _dwp.Discipline.testUnits;

                item[nameof(FdpTimeDistributionInfo.TotalTime)] = totalTime;
                item[nameof(FdpTimeDistributionInfo.TotalAuditoryTime)] = totalAuditoryTime;
                item[nameof(FdpTimeDistributionInfo.TotalHomeworkTime)] = totalHomeworkTime;
            }

            return items;
        }
    }
}