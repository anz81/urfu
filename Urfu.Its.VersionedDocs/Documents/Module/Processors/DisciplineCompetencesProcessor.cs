using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;

namespace Urfu.Its.VersionedDocs.Documents.Module.Processors
{
    public class DisciplineCompetencesProcessor :IBlockContentProcessor
    {
        private readonly JObject _actualDocumentData;

        public DisciplineCompetencesProcessor(JObject actualDocumentData)
        {
            _actualDocumentData = actualDocumentData;
        }

        public JToken ProcessContent(JToken data)
        {
            var plannedCompetenceIds = _actualDocumentData[nameof(ModuleWorkingProgramFgosVoSchemaModel.PlannedResults)]
                .SelectMany(pr => pr[nameof(PlannedResultItemInfo.Results)].SelectMany(r =>
                        r[nameof(EduResultCompetencesInfo.Competences)]
                            .Select(c => c.Value<int>(nameof(CompetenceInfo.Id))))
                    .Union(pr[nameof(PlannedResultItemInfo.UniversalCompetences)]
                        .Select(c => c.Value<int>(nameof(CompetenceInfo.Id)))))
                .Distinct()
                .ToList();

            foreach (var fdpItem in data)
            {
                foreach (var disciplineCompetences in fdpItem[nameof(FdpDisciplineCompetencesInfo.Items)])
                {
                    foreach (var idToken in disciplineCompetences[nameof(DisciplineCompetencesInfo.CompetenceIds)].ToList())
                    {
                        var value = idToken.Value<int>();
                        if(!plannedCompetenceIds.Contains(value))
                            idToken.Remove();
                    }
                }
            }

            return data;
        }
    }
}