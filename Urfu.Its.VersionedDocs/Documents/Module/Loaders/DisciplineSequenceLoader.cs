using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Module.Loaders
{
    public class DisciplineSequenceLoader : ObjectBlockContentLoader<DisciplineSequenceInfo>
    {
        private readonly ApplicationDbContext _db;
        private readonly Web.DataContext.Module _module;

        public DisciplineSequenceLoader(ApplicationDbContext db, Web.DataContext.Module module)
        {
            _db = db;
            _module = module;
        }

        protected override DisciplineSequenceInfo LoadAnyContent(JToken blockContent)
        {
            var result = new DisciplineSequenceInfo
            {
                NoRequirements = blockContent[nameof(DisciplineSequenceInfo.NoRequirements)].Value<bool>()
            };

            var docItems = blockContent[nameof(DisciplineSequenceInfo.Items)];
            var moduleId = _module.uuid;

            var disciplines = _db.Disciplines.Where(d => d.Modules.Any(m=>m.uuid == moduleId)).ToList();
            foreach (var discipline in disciplines)
            {
                var docItem = docItems.FirstOrDefault(i => i.Value<string>(nameof(DisciplineSequenceItemInfo.DisciplineId)) == discipline.uid);

                var item = new DisciplineSequenceItemInfo {DisciplineId = discipline.uid, DisciplineName = discipline.title};
                result.Items.Add(item);

                if (docItem != null)
                    item.Number = docItem.Value<int>(nameof(DisciplineSequenceItemInfo.Number));
            }

            return result;
        }
    }
}