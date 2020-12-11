using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Loaders
{
    public class PlannedLearningOutcomesLoader : ObjectBlockContentLoader<PlannedLearningOutcomesInfo>
    {
        private readonly ApplicationDbContext _db;
        private readonly DisciplineWorkingProgram _dwp;
        private readonly IVersionedDocumentService _documentService;

        public PlannedLearningOutcomesLoader(ApplicationDbContext db, DisciplineWorkingProgram dwp, IVersionedDocumentService documentService)
        {
            _db = db;
            _dwp = dwp;
            _documentService = documentService;
        }

        protected override PlannedLearningOutcomesInfo LoadAnyContent(JToken blockContent)
        {
            var jObj = (JObject)blockContent;

            var result = new PlannedLearningOutcomesInfo
            {
                MustKnow = jObj["MustKnow"].Value<string>(),
                MustOwn = jObj["MustOwn"].Value<string>(),
                MustBeAbleTo = jObj["MustBeAbleTo"].Value<string>()
            };

            var moduleDocument = _db.VersionedDocuments.Find(_dwp.ModuleWorkingProgramId);

            var moduleModel = _documentService.CreateModel<ModuleWorkingProgramFgosVoSchemaModel>(moduleDocument, nameof(ModuleWorkingProgramFgosVoSchemaModel.DisciplineCompetences));
            var allowedCompetenceIds = moduleModel.DisciplineCompetences.SelectMany(r => r.Items)
                .Where(d => d.DisciplineName == _dwp.Discipline.title).SelectMany(d => d.CompetenceIds).Distinct()
                .ToList();

            var array = jObj["Items"];
            var competenceIds = array.SelectMany(i => i["Competences"]).Select(c => c["Id"].Value<int>())
                .Where(id=>allowedCompetenceIds.Contains(id)).Distinct().ToList();

            var competences = _db.Competences.Where(c => competenceIds.Contains(c.Id)).ToList();
            foreach (var item in array)
            {
                var info = new CompetencesDescriptionInfo
                {
                    Description = item["Description"].Value<string>()
                };
                result.Items.Add(info);

                var competenceItems = item["Competences"];
                foreach (var competenceItem in competenceItems)
                {
                    var id = competenceItem["Id"].Value<int>();
                    var c = competences.FirstOrDefault(d => d.Id == id);
                    if (c != null)
                    {
                        info.Competences.Add(new CompetenceInfo
                        {
                            Id = c.Id,
                            Code = c.Code,
                            Order = c.Order,
                            Type = c.Type,
                            Content = c.Content,
                            DirectionId = c.DirectionId
                        });
                    }
                }
            }
            
            return result;
        }
    }
}