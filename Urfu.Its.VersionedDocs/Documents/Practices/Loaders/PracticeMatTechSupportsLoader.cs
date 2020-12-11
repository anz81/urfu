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
    public class PracticeMatTechSupportsLoader : ObjectBlockContentLoader<IEnumerable<PracticeMatTechSupportStructure>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public PracticeMatTechSupportsLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<PracticeMatTechSupportStructure> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.Fdps)];
            var practicesStructuresObject = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.PracticeStructures)].Value<object>();
            var practiceStructures = JsonConvert.DeserializeObject<ICollection<FdpPracticeStructureInfo>>(practicesStructuresObject.ToString());
            var items = practiceStructures.SelectMany(s => s.Items);
            
            var array = blockContent as JArray;

            var matsup = array.Select(item => new
            {
                DisciplineUid = item.Value<string>("DisciplineUid"),
                AdditionalType = item.Value<string>("AdditionalType"),
                MatTechSupport = item.Value<string>("MatTechSupport")
            }).ToList();
            
            var disciplines = _module.disciplines.Where(d => !d.section.Contains("Контроль")).ToList().Select(d => new PracticeMatTechSupportStructure
            {
                DisciplineUid = d.pkey,
                Title = d.title,
                MatTechSupport = matsup.Count > 0 ? matsup.FirstOrDefault(_ => _.DisciplineUid == d.pkey).MatTechSupport : null,
                AdditionalType = matsup?.FirstOrDefault(_ => _.DisciplineUid == d.pkey)?.AdditionalType ??
                    items.FirstOrDefault(i => i.CatalogDisciplineId == d.pkey)?.AdditionalType
            }).ToList();

            disciplines = disciplines.OrderByDescending(d => d.AdditionalType).ToList();
           
            return disciplines;
        }
        
    }
}
