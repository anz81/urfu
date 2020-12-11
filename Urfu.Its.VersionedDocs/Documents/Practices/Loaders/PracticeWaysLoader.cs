using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;

using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Practices.Loaders
{
    public class FdpPracticeWaysLoader : ObjectBlockContentLoader<IEnumerable<FdpPracticeWaysInfo>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public FdpPracticeWaysLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<FdpPracticeWaysInfo> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.Fdps)];
            var practicesStructuresObject = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.PracticeStructures)].Value<object>();
            var practiceStructures = JsonConvert.DeserializeObject<ICollection<FdpPracticeStructureInfo>>(practicesStructuresObject.ToString());
            var items = practiceStructures.SelectMany(s => s.Items);
            
            var array = blockContent as JArray;
            var infosup = array.Select(item => new
            {
                DisciplineId = item.Value<string>("DisciplineId"),
                DisciplineTitle = item.Value<string>("DisciplineTitle"),
                AdditionalType = item.Value<string>("AdditionalType"),
                PracticeWay = item.Value<string>("PracticeWay"),
                PracticeMethod = item.Value<string>("PracticeMethod")
            }).ToList();

            var disciplines = _module.disciplines.Where(d => !d.section.Contains("Контроль")).ToList().Select(d => new FdpPracticeWaysInfo
            {
                DisciplineId = d.pkey,
                DisciplineTitle = d.title,
                PracticeWay = infosup.Count > 0 ? infosup.FirstOrDefault(_ => _.DisciplineId == d.pkey).PracticeWay : null,
                PracticeMethod = infosup.Count > 0 ? infosup.FirstOrDefault(_ => _.DisciplineId == d.pkey).PracticeMethod : null,
                AdditionalType = infosup?.FirstOrDefault(_ => _.DisciplineId == d.pkey)?.AdditionalType ??
                    items?.FirstOrDefault(i => i.CatalogDisciplineId == d.pkey)?.AdditionalType
            }).ToList();
            
            disciplines = disciplines.OrderByDescending(d => d.AdditionalType).ToList();

            return disciplines;
        }
    }
}
