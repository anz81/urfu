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
using Urfu.Its.VersionedDocs.Documents.Module;

namespace Urfu.Its.VersionedDocs.Documents.Practices.Loaders
{
    public class PracticeResultInfosLoader : ObjectBlockContentLoader<IEnumerable<PlannedResultPracticeInfo>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public PracticeResultInfosLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<PlannedResultPracticeInfo> LoadAnyContent(JToken blockContent)
        {
            var array = blockContent as JArray;
            var infosup = array.Select(item => new
            {
                DisciplineId = item.Value<string>("DisciplineId"),
                ProfileId = item.Value<string>("ProfileId"),
                ProfileCode = item.Value<string>("ProfileCode"),
                Results = item.Value<object>("Results")
            }).ToList();
            
            var practicesStructuresObject = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.PracticeStructures)].Value<object>();
            var practiceStructures = JsonConvert.DeserializeObject<ICollection<FdpPracticeStructureInfo>>(practicesStructuresObject.ToString());
            
            var result = new List<PlannedResultPracticeInfo>();

            foreach (var structure in practiceStructures)
            {
                foreach (var item in structure.Items)
                {
                    var info = infosup?.Where(_ => _.DisciplineId == item.DisciplineId).ToList();
                    if (info?.Count > 0)
                    {
                        var newInfo = new PlannedResultPracticeInfo();
                        newInfo.DisciplineId = item.DisciplineId;
                        newInfo.DisciplineName = item.Title;
                        newInfo.ProfileId = infosup.Count > 0 ? infosup.FirstOrDefault(_ => _.DisciplineId == item.DisciplineId).ProfileId : null;
                        newInfo.ProfileCode = infosup.Count > 0 ? infosup.FirstOrDefault(_ => _.DisciplineId == item.DisciplineId).ProfileCode : null;
                        newInfo.Results = new List<EduResultCompetencesInfo>();
                        foreach(var i in info)
                        {
                            var list = JsonConvert.DeserializeObject<ICollection<EduResultCompetencesInfo>>(i.Results.ToString());
                            foreach (var l in list)
                            {
                                newInfo.Results.Add(l);
                            }
                        }
                        result.Add(newInfo);
                    }
                }
            }
            
            return result; 
        }
    }
}
