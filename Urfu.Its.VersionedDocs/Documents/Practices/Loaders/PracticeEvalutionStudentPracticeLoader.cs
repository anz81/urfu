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
    public class PracticeEvalutionStudentPracticeLoader : ObjectBlockContentLoader<IEnumerable<PracticeEvalutionStudentPracticeStructure>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public PracticeEvalutionStudentPracticeLoader(JObject loadedDocumentData, ApplicationDbContext db, Web.DataContext.Module module)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
            _module = module;
        }

        protected override IEnumerable<PracticeEvalutionStudentPracticeStructure> LoadAnyContent(JToken blockContent)
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

            var result = new List<PracticeEvalutionStudentPracticeStructure>();

            foreach (var structure in practiceStructures)
            {
                var fdp = fdps.First(f => f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() == structure.FdpId);
                var directionId = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionId)].Value<string>();
                var directionCode = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DirectionCode)].Value<string>();

                var res = result.FirstOrDefault(r => r.DirectionId == directionId);
                if (res != null) continue;

                var esStructure = new PracticeEvalutionStudentPracticeStructure();
                esStructure.DirectionId = directionId;
                esStructure.DirectionCode = directionCode;
                esStructure.Items = new List<PracticeEvalutionStudentPracticeItem>();

                var _items = infosup.Count > 0
                    ? JsonConvert.DeserializeObject<ICollection<PracticeEvalutionStudentPracticeItem>>(infosup.FirstOrDefault(_ => _.DirectionId == directionId).Items.ToString())
                    : null;

                if (_items == null)
                {
                    _items = new List<PracticeEvalutionStudentPracticeItem>();
                    foreach (var item in structure.Items)
                    {
                        var _item = new PracticeEvalutionStudentPracticeItem()
                        {
                            AdditionalType = item.AdditionalType,
                            DisciplineUid = item.DisciplineId,
                            DisciplineName = item.DisciplineName,
                            Title = item.Title
                        };
                        _item.TechCardDisciplineCertification.EduLoad.InitDefaults();
                        _item.TechCardDisciplineCertification.EduLoad.TotalCoefficient = "1";
                        _items.Add(_item);
                    }
                }

                esStructure.Items = _items.OrderByDescending(r => r.AdditionalType).ToList(); ;
                result.Add(esStructure);
            }
            
            return result;
        }
        
    }
}
