using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Newtonsoft.Json;

namespace Urfu.Its.VersionedDocs.Documents.Practices.Loaders
{
    public class PracticeEvalutionToolsLoader : ObjectBlockContentLoader<IEnumerable<PracticeEvalutionToolsStructure>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public PracticeEvalutionToolsLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<PracticeEvalutionToolsStructure> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.Fdps)];

            var array = blockContent as JArray;

            var infosup = array.Select(item => new
            {
                DirectionId = item.Value<string>("DirectionId"),
                Items = item.Value<object>("Items")
            }).ToList();

            var practicesStructuresObject = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.PracticeStructures)].Value<object>();
            var practiceStructures = JsonConvert.DeserializeObject<ICollection<FdpPracticeStructureInfo>>(practicesStructuresObject.ToString());

            var result = new List<PracticeEvalutionToolsStructure>();

            foreach (var structure in practiceStructures)
            {
                var fdp = fdps.First(f => f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() == structure.FdpId);
                var directionId = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionId)].Value<string>();
                var directionCode = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionCode)].Value<string>();

                var res = result.FirstOrDefault(r => r.DirectionId == directionId);
                if (res != null) continue;

                var resultStructure = new PracticeEvalutionToolsStructure();
                resultStructure.DirectionId = directionId;
                resultStructure.DirectionCode = directionCode;
                resultStructure.Items = new List<PracticeEvalutionToolsItem>();

                var _result = infosup.Count > 0
                    ? JsonConvert.DeserializeObject<ICollection<PracticeEvalutionToolsItem>>(infosup.FirstOrDefault(_ => _.DirectionId == directionId).Items.ToString())
                    : null;

                if (_result == null)
                {
                    _result = new List<PracticeEvalutionToolsItem>();
                    foreach (var item in structure.Items)
                    {
                        _result.Add(new PracticeEvalutionToolsItem()
                        {
                            AdditionalType = item.AdditionalType,
                            DisciplineUid = item.DisciplineId,
                            Title = item.Title
                        });
                    }
                }
                resultStructure.Items = _result.OrderByDescending(r => r.AdditionalType).ToList();
                result.Add(resultStructure);
            }

            return result;
        }
        
    }
}
