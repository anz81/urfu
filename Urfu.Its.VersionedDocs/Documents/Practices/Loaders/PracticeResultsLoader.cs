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
    public class PracticeResultsLoader : ObjectBlockContentLoader<IEnumerable<PracticeResultStructure>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public PracticeResultsLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<PracticeResultStructure> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.Fdps)];

            var array = blockContent as JArray;
            var infosup = array.Select(item => new
            {
                DirectionId = item.Value<string>("DirectionId"),
                Results = item.Value<object>("Results")
            }).ToList();
            
            var practicesStructuresObject = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.PracticeStructures)].Value<object>();
            var practiceStructures = JsonConvert.DeserializeObject<ICollection<FdpPracticeStructureInfo>>(practicesStructuresObject.ToString());
            
            var result = new List<PracticeResultStructure>();

            foreach (var structure in practiceStructures)
            {
                var fdp = fdps.First(f => f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() == structure.FdpId);
                var directionId = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionId)].Value<string>();
                var directionCode = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionCode)].Value<string>();

                var res = result.FirstOrDefault(r => r.DirectionId == directionId);
                if (res != null) continue;
                
                var resultStructure = new PracticeResultStructure();
                resultStructure.DirectionId = directionId;
                resultStructure.DirectionCode = directionCode;
                resultStructure.Results = new List<PracticeResult>();
                
                var _result = infosup.Count > 0
                    ? JsonConvert.DeserializeObject<ICollection<PracticeResult>>(infosup.FirstOrDefault(_ => _.DirectionId == directionId).Results.ToString())
                    : null;

                if (_result == null)
                {
                    _result = new List<PracticeResult>();
                    foreach(var item in structure.Items)
                    {
                        _result.Add(new PracticeResult()
                        {
                            AdditionalType = item.AdditionalType,
                            DisciplineUid = item.DisciplineId,
                            Title = item.Title
                        });
                    }
                }
                resultStructure.Results = _result.OrderByDescending(r => r.AdditionalType).ToList();
                result.Add(resultStructure);
            }
            
            return result; 
        }
    }
}
