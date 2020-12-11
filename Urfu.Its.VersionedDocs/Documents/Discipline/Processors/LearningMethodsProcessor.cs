using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Processors
{
    public class LearningMethodsProcessor : IBlockContentProcessor
    {
        private readonly JObject _actualDocumentData;

        public LearningMethodsProcessor(JObject actualDocumentData)
        {
            _actualDocumentData = actualDocumentData;
        }

        public JToken ProcessContent(JToken data)
        {
            var result = new JArray();
            var items = (JArray)data;
            var sections = _actualDocumentData[nameof(DisciplineWorkingProgramFgosVoSchemaModel.Sections)];
            var existingItems = items.Where(item => sections.Select(s => s[nameof(DisciplineSectionInfo.ItemId)].Value<string>()).Contains(item[nameof(LearningMethodsInfo.SectionId)].Value<string>())).ToList();
            var newSections = sections.Where(section => !items.Select(item => item[nameof(LearningMethodsInfo.SectionId)].Value<string>()).Contains(section[nameof(DisciplineSectionInfo.ItemId)].Value<string>())).ToList();
            //var oldItems = items.Where(item => !fdps.Select(fdp => fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>()).Contains(item[nameof(DisciplineScopeInfo.FdpId)].Value<string>()));

            foreach (var item in existingItems)
                result.Add(item);

            foreach (var newSection in newSections)
            {
                result.Add(JToken.FromObject(new LearningMethodsInfo
                {
                    SectionId = newSection[nameof(DisciplineSectionInfo.ItemId)].Value<string>()
                }));
            }

            return result;
        }
    }
}