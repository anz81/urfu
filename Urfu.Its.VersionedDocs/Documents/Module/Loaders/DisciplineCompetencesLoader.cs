using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickGraph;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Module.Loaders
{
    public class DisciplineCompetencesLoader : ObjectBlockContentLoader<IEnumerable<FdpDisciplineCompetencesInfo>>
    {
        private readonly JObject _loadedDocumentData;
        private readonly ApplicationDbContext _db;

        public DisciplineCompetencesLoader(JObject loadedDocumentData, ApplicationDbContext db)
        {
            _loadedDocumentData = loadedDocumentData;
            _db = db;
        }

        protected override IEnumerable<FdpDisciplineCompetencesInfo> LoadAnyContent(JToken blockContent)
        {
            var structures = _loadedDocumentData[nameof(ModuleWorkingProgramFgosVoSchemaModel.ModuleStructures)];
            var actualItems = BlockDataHelper.GetActualMergedData<FdpModuleStructureInfo, FdpDisciplineCompetencesInfo, string>
                (structures, blockContent, fdp => fdp.FdpId, td => td.FdpId).ToList();

            foreach (var fdpItem in actualItems)
            {
                var fdpStructure = structures.First(f => f[nameof(FdpModuleStructureInfo.FdpId)].Value<string>() == fdpItem.FdpId);

                var moduleStructureItems = JsonConvert.DeserializeObject<ModuleStructureItemInfo[]>(fdpStructure[nameof(FdpModuleStructureInfo.Items)].ToString());
                var actualSubItems = BlockDataHelper.GetActualMergedData(moduleStructureItems,
                    fdpItem.Items.ToArray(), s => s.DisciplineId, l => l.DisciplineId).ToList();

                foreach (var subItem in actualSubItems)
                {
                    var plan = moduleStructureItems.First(p => p.DisciplineId == subItem.DisciplineId);
                    
                    subItem.DisciplineName = plan.DisciplineName;
                    subItem.DisciplineDisplayName = $"({plan.EducationalProgramPart}) {subItem.DisciplineName}";                                          
                }

                fdpItem.Items = actualSubItems.ToList();
            }

            var allCompetenceIds = actualItems.SelectMany(i => i.Items).SelectMany(i => i.CompetenceIds).Distinct();
            var allCompetences = _db.Competences.Where(c => allCompetenceIds.Contains(c.Id)).Select(c=>
                new CompetenceInfo
                {
                    Code = c.Code,
                    Content = c.Content,
                    Id = c.Id,
                    DirectionId = c.DirectionId,
                    Type = c.Type,
                    Order = c.Order
                }).ToList();

            foreach (var fdpItem in actualItems)
            {
                foreach (var subItem in fdpItem.Items)
                {
                    var itemCompetences = allCompetences.Where(c => subItem.CompetenceIds.Contains(c.Id)).ToList();
                    subItem.OkCompetences = itemCompetences.Where(c => c.Type == "ОК" || c.Type == "ДОК").ToList();
                    subItem.OpkCompetences = itemCompetences.Where(c => c.Type == "ОПК" || c.Type == "ДОПК").ToList();
                    subItem.PkCompetences = itemCompetences.Where(c => c.Type == "ПК" || c.Type == "ДПК").ToList();
                }
            }

            return actualItems;
        }
    }
}