using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;

using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Practices.Loaders
{
    public class PracticeStructuresLoader : ObjectBlockContentLoader<IEnumerable<FdpPracticeStructureInfo>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public PracticeStructuresLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<FdpPracticeStructureInfo> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.Fdps)];
            var actualItems = BlockDataHelper.GetActualMergedData<FamilirizationTypeDirectionPlanInfo, FdpPracticeStructureInfo, string>
                (fdps, blockContent, fdp => fdp.ItemId, td => td.FdpId).ToList();

            var moduleId = _module.uuid;
            var disciplines = _module.disciplines;
            foreach (var fdpItem in actualItems)
            {
                var fdp = fdps.First(f => f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() == fdpItem.FdpId);
                var disciplineId = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DisciplineId)].Value<string>();
                var directionId = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionId)].Value<string>();
                var famType = fdp[nameof(FamilirizationTypeDirectionPlanInfo.FamType)].Value<string>();
                var version = _db.Plans.First(p => p.disciplineUUID == disciplineId)?.versionUUID;
                var modulePlans = _db.Plans.Where(p => p.moduleUUID == moduleId && p.directionId == directionId && p.familirizationType == famType && p.versionUUID == version).ToList();

                var actualSubItems = BlockDataHelper.GetActualMergedData(modulePlans.ToArray(),
                    fdpItem.Items.ToArray(), s => s.disciplineUUID, l => l.DisciplineId).ToList();
                
                foreach (var subItem in actualSubItems)
                {
                    var plan = modulePlans.First(p => p.disciplineUUID == subItem.DisciplineId);
                    if (disciplines.FirstOrDefault(d => d.pkey == plan.catalogDisciplineUUID)?.section?.Contains("Контроль") == true) continue;

                    subItem.CatalogDisciplineId = plan.catalogDisciplineUUID;
                    subItem.Semesters = string.Join(", ", plan.allTermsExtracted?.TrimStart('[').TrimEnd(']').Split(',') ?? new string[0]);
                    subItem.Title = $"{plan.disciplineTitle} ({plan.additionalType})";
                    subItem.AdditionalType = plan.additionalType;
                    subItem.AdditionalWeeks = plan?.additionalWeeks;
                    subItem.TotalUnits = plan.testUnitsByTerm == "null" ? (decimal?)null : JObject.Parse(plan.testUnitsByTerm).Properties().Sum(p => p.Value.Value<int>());
                    subItem.DisciplineName = plan.disciplineTitle;
                    fdpItem.Items.Add(subItem);
                    fdpItem.Items = fdpItem.Items.OrderByDescending(i => i.AdditionalType).ToList();
                }
            }
            
            return actualItems;
        }
        
    }
}
