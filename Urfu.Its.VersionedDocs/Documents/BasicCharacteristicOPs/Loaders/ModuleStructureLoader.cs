using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Loaders
{
    public class ModuleStructureLoader : ObjectBlockContentLoader<object>
    {
        private readonly ApplicationDbContext _db;
        private readonly BasicCharacteristicOPInfo _info;
        private readonly BasicCharacteristicOP _ohop;

        public ModuleStructureLoader(ApplicationDbContext db, BasicCharacteristicOPInfo info, BasicCharacteristicOP ohop)
        {
            _db = db;
            _info = info;
            _ohop = ohop;
        }

        protected override object LoadAnyContent(JToken blockContent)
        {
            var structure = new ModuleStructure();

            var item = blockContent as JObject;

            // вытаскиваем сохраненные данные

            if (item[nameof(ModuleStructure.Practices)].Type != JTokenType.Null)
            {
                var practicesObj = item[nameof(ModuleStructure.Practices)].Value<object>();
                structure.Practices = JsonConvert.DeserializeObject<ICollection<ModuleInfoSelected>>(practicesObj.ToString());
            }

            if (item[nameof(ModuleStructure.Facultative)].Type != JTokenType.Null)
            {
                var facultativeObj = item[nameof(ModuleStructure.Facultative)].Value<object>();
                structure.Facultative = JsonConvert.DeserializeObject<ICollection<ModuleInfoSelected>>(facultativeObj.ToString());
            }

            if (item[nameof(ModuleStructure.Gia)].Type != JTokenType.Null)
            {
                var giaObj = item[nameof(ModuleStructure.Gia)].Value<object>();
                structure.Gia = JsonConvert.DeserializeObject<ICollection<ModuleInfoSelected>>(giaObj.ToString());
            }

            if (item[nameof(ModuleStructure.Modules)].Type != JTokenType.Null)
            {
                var modulesObj = item[nameof(ModuleStructure.Modules)].Value<object>();
                structure.Modules = JsonConvert.DeserializeObject<ICollection<ModuleInfoSelected>>(modulesObj.ToString());
            }

            structure = new BasicCharacteristicOPDefaultValues(_info).ModuleStructure(structure, addModulesFromDb: _ohop.Status.CanEdit());

            return structure;
        }
    }
}