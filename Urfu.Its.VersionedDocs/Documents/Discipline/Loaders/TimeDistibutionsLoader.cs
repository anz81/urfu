using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Loaders
{
    public class TimeDistibutionsLoader : ObjectBlockContentLoader<IEnumerable<FdpTimeDistributionInfo>>
    {
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;
        private readonly DisciplineWorkingProgram _dwp;
        private readonly JObject _loadedDocumentData;
        private readonly Web.DataContext.Discipline _discipline;

        public TimeDistibutionsLoader(ApplicationDbContext db, Web.DataContext.Module module, DisciplineWorkingProgram dwp, JObject loadedDocumentData, Web.DataContext.Discipline discipline)
        {
            _db = db;
            _module = module;
            _dwp = dwp;
            _loadedDocumentData = loadedDocumentData;
            _discipline = discipline;
        }

        protected override IEnumerable<FdpTimeDistributionInfo> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(DisciplineWorkingProgramFgosVoSchemaModel.Fdps)];
            var actualItems = BlockDataHelper.GetActualMergedData<FamilirizationTypeDirectionPlanInfo, FdpTimeDistributionInfo, string>(fdps, blockContent, fdp=>fdp.ItemId, td=>td.FdpId).ToList();
            foreach (var timeDistribution in actualItems)
            {
                var fdp = fdps.First(f => f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() == timeDistribution.FdpId);
                var fakePlanId = fdp.Value<string>(nameof(FamilirizationTypeDirectionPlanInfo.DisciplineId));
                var plan = _db.GetDisciplinePlan(fakePlanId, _discipline.title);
                var actualPlanId = plan.disciplineUUID;
                var planAdditional = _db.PlanAdditionals.FirstOrDefault(p => p.disciplineUUID == actualPlanId);

                timeDistribution.ModuleUnits = _module.testUnits;
                timeDistribution.DisciplineUnits = _dwp.Discipline.testUnits;

                if (planAdditional != null)
                {
                    timeDistribution.TotalTime = planAdditional.allload;
                    timeDistribution.TotalAuditoryTime = planAdditional.allaudit;
                    timeDistribution.TotalHomeworkTime = planAdditional.self;
                }
            }

            return actualItems;
        }
    }
}