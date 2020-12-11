using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Module
{
    public class ModuleStructuresLoader : ObjectBlockContentLoader<IEnumerable<FdpModuleStructureInfo>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public ModuleStructuresLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<FdpModuleStructureInfo> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(ModuleWorkingProgramFgosVoSchemaModel.Fdps)];
            var actualItems = BlockDataHelper.GetActualMergedData<FamilirizationTypeDirectionPlanInfo, FdpModuleStructureInfo, string>
                (fdps, blockContent, fdp => fdp.ItemId, td => td.FdpId).ToList();

            var moduleId = _module.uuid;
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
                    var planAdditional = _db.PlanAdditionals.FirstOrDefault(p => p.disciplineUUID == plan.disciplineUUID && p.versionUUID == plan.versionUUID);

                    subItem.DisciplineName = plan.disciplineTitle;
                    subItem.Semesters = string.Join(", ", plan.allTermsExtracted?.TrimStart('[').TrimEnd(']').Split(',') ?? new string[0]);
                    subItem.Lections = planAdditional?.lecture;
                    subItem.Practices = planAdditional?.practice;
                    subItem.Labs = planAdditional?.labs;
                    subItem.AuditoryTotal = planAdditional?.allaudit;
                    subItem.SelfWork = planAdditional?.self;
                    subItem.IntermediateCertification = string.Join(", ",
                        planAdditional?.controls.Split(',').Select(n =>
                            KmName(n))??Enumerable.Empty<string>());
                    subItem.TotalTime = planAdditional?.allload;
                    subItem.TotalUnits = plan.testUnitsByTerm == "null" ? (decimal?) null : JObject.Parse(plan.testUnitsByTerm).Properties().Sum(p => p.Value.Value<int>());
                }

                fdpItem.Items = actualSubItems.ToList();
            }

            return actualItems;
        }

        private static string KmName(string n)
        {
            if (n == "Проект по модулю")
            {
                return "ПМ";
            }
            return string.Join("", n.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => char.ToUpper(c[0]).ToString()));
        }
    }
}