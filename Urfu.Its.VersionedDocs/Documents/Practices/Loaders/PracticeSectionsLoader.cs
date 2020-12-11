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
    public class PracticeSectionsLoader : ObjectBlockContentLoader<IEnumerable<PracticeSectionsStructure>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public PracticeSectionsLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<PracticeSectionsStructure> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.Fdps)];

            var array = blockContent as JArray;
            var infosup = array.Select(item => new
            {
                DirectionId = item.Value<string>("DirectionId"),
                Sections = item.Value<object>("Sections")
            }).ToList();
            
            var practicesStructuresObject = _loadedDocumentData[nameof(PracticesWorkingProgramFgosVoSchemaModel.PracticeStructures)].Value<object>();
            var practiceStructures = JsonConvert.DeserializeObject<ICollection<FdpPracticeStructureInfo>>(practicesStructuresObject.ToString());
            
            var result = new List<PracticeSectionsStructure>();

            foreach (var structure in practiceStructures)
            {
                var fdp = fdps.First(f => f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() == structure.FdpId);
                var directionId = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionId)].Value<string>();
                var directionCode = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionCode)].Value<string>();

                var res = result.FirstOrDefault(r => r.DirectionId == directionId);
                if (res != null) continue;

                var sectionStructure = new PracticeSectionsStructure();
                sectionStructure.DirectionId = directionId;
                sectionStructure.DirectionCode = directionCode;
                sectionStructure.Sections = new List<PracticeSection>();

                var _sections = infosup.Count > 0
                    ? JsonConvert.DeserializeObject<ICollection<PracticeSection>>(infosup.FirstOrDefault(_ => _.DirectionId == directionId).Sections.ToString())
                    : null;

                if (_sections == null)
                {
                    _sections = new List<PracticeSection>();
                    foreach (var item in structure.Items)
                    {
                        _sections.Add(new PracticeSection()
                        {
                            AdditionalType = item.AdditionalType,
                            DisciplineUid = item.DisciplineId,
                            Title = item.Title
                        });
                    }
                }

                sectionStructure.Sections = _sections.OrderByDescending(r => r.AdditionalType).ToList();
                result.Add(sectionStructure);
            }
            
            return result; 
        }
    }
}
