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
    public class PracticeManualsLoader : ObjectBlockContentLoader<IEnumerable<PracticeManualsStructure>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public PracticeManualsLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<PracticeManualsStructure> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.Fdps)];
            var practicesStructuresObject = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.PracticeStructures)].Value<object>();
            var practiceStructures = JsonConvert.DeserializeObject<ICollection<FdpPracticeStructureInfo>>(practicesStructuresObject.ToString());
            var items = practiceStructures.SelectMany(s => s.Items);
 
            var array = blockContent as JArray;

            var infosup = array.Select(item => new
            {
                DisciplineUid = item.Value<string>("DisciplineUid"),
                AdditionalType = item.Value<string>("AdditionalType"),
                Literature = item.Value<object>("Literature"),
                MethodicalSupport = item.Value<string>("MethodicalSupport"),
                SoftwareNotUsed = item.Value<bool>("SoftwareNotUsed"),
                SoftwareSystemOrOffice = item.Value<object>("SoftwareSystemOrOffice"),
                Databases = item.Value<string>("Databases"),
                ElectronicEducationalResources = item.Value<string>("ElectronicEducationalResources"),
            }).ToList();
            
            var disciplines = _module.disciplines.Where(d => !d.section.Contains("Контроль")).ToList().Select(d => new PracticeManualsStructure
            {
                DisciplineUid = d.pkey,
                Title = d.title,
                Literature = infosup.Count > 0 
                    ? JsonConvert.DeserializeObject<LiteratureInfo>(infosup.FirstOrDefault(_ => _.DisciplineUid == d.pkey).Literature.ToString()) 
                    : new LiteratureInfo(),
                MethodicalSupport = infosup.Count > 0 ? infosup.FirstOrDefault(_ => _.DisciplineUid == d.pkey).MethodicalSupport : null,
                SoftwareNotUsed = infosup.Count > 0 ? infosup.FirstOrDefault(_ => _.DisciplineUid == d.pkey).SoftwareNotUsed : false,
                SoftwareSystemOrOffice = infosup.Count > 0
                    ? JsonConvert.DeserializeObject<ICollection<SoftwareItemInfo>>(infosup.FirstOrDefault(_ => _.DisciplineUid == d.pkey).SoftwareSystemOrOffice.ToString())
                    : new List<SoftwareItemInfo>(),
                Databases = infosup.Count > 0 ? infosup.FirstOrDefault(_ => _.DisciplineUid == d.pkey).Databases : null,
                ElectronicEducationalResources = infosup.Count > 0 ? infosup.FirstOrDefault(_ => _.DisciplineUid == d.pkey).ElectronicEducationalResources : null,
                AdditionalType = infosup?.FirstOrDefault(_ => _.DisciplineUid == d.pkey)?.AdditionalType ??
                    items.FirstOrDefault(i => i.CatalogDisciplineId == d.pkey)?.AdditionalType
            }).ToList();

            disciplines = disciplines.OrderByDescending(d => d.AdditionalType).ToList();
            return disciplines;
        }
        
    }
}
