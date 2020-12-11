using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Module.Loaders
{
    public class ModuleIntermediateCertificationFormLoader : ObjectBlockContentLoader<IEnumerable<FdpModuleIntermediateCertificationFormInfo>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;

        public ModuleIntermediateCertificationFormLoader(JObject loadedDocumentData, ApplicationDbContext db)
        {
            _loadedDocumentData = loadedDocumentData;            
            _db = db;
        }

        protected override IEnumerable<FdpModuleIntermediateCertificationFormInfo> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(ModuleWorkingProgramFgosVoSchemaModel.Fdps)];
            var actualItems = BlockDataHelper.GetActualMergedData<FamilirizationTypeDirectionPlanInfo, FdpModuleIntermediateCertificationFormInfo, string>(fdps, blockContent, fdp => fdp.ItemId, td => td.FdpId).ToList();

            var disciplineIds = fdps.Select(fdp => fdp[nameof(FamilirizationTypeDirectionPlanInfo.DisciplineId)].Value<string>()).ToList();
            var plans = _db.Plans.Where(p => disciplineIds.Contains(p.disciplineUUID)).ToList();

            foreach (var form in actualItems)
            {
                var fdp = fdps.First(f => f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() == form.FdpId);
                var disciplineId = fdp.Value<string>(nameof(FamilirizationTypeDirectionPlanInfo.DisciplineId));
                var plan = plans.First(p => p.disciplineUUID == disciplineId);
                var versionId = plan.versionUUID;
                var moduleId = plan.moduleUUID;
                var directionId = plan.directionId;
                var famType = plan.familirizationType;
                var versionPlans = _db.Plans.Where(p => p.versionUUID == versionId && p.moduleUUID == moduleId && p.familirizationType == famType && p.directionId == directionId).ToList();
                
                var dic = new Dictionary<string, bool>
                {
                    { "Зачет-Проект по модулю", false },
                    { "Интегрированный экзамен", false },
                    { "Проект по модулю", false },
                };

                foreach (var pair in dic.ToList())
                {
                    dic[pair.Key] = versionPlans.Any(p=>p.controls.Contains(pair.Key));                    
                }

                Debug.Assert(dic.Count(d => d.Value) < 2);

                var control = dic.FirstOrDefault(d => d.Value).Key;
                form.Form = control ?? "Отсутствует";

                yield return form;
            }
        }
    }
}