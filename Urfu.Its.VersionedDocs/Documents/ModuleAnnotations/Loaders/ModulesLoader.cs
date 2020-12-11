using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.ModuleAnnotations.Loaders
{
    public class ModulesLoader : ObjectBlockContentLoader<ModuleAnnotationStructure>
    {
        private readonly ApplicationDbContext _db;
        private readonly ModuleAnnotation _annotation;
        private readonly BasicCharacteristicOPSchemaModel _ohopData;

        public ModulesLoader(ApplicationDbContext db, ModuleAnnotation annotation, BasicCharacteristicOPSchemaModel ohopData)
        {
            _db = db;
            _annotation = annotation;
            _ohopData = ohopData;
        }

        protected override ModuleAnnotationStructure LoadAnyContent(JToken blockContent)
        {
            var item = blockContent as JObject;

            var modules = new ModuleAnnotationStructure();


            // вытаскиваем сохраненные данные
            if (item[nameof(ModuleAnnotationStructure.RequiredModules)].Type != JTokenType.Null)
            {
                var obj = item[nameof(ModuleAnnotationStructure.RequiredModules)].Value<object>();
                modules.RequiredModules = JsonConvert.DeserializeObject<ICollection<ModuleAnnotationRow>>(obj.ToString());
            }
            if (item[nameof(ModuleAnnotationStructure.Modules)].Type != JTokenType.Null)
            {
                var obj = item[nameof(ModuleAnnotationStructure.Modules)].Value<object>();
                modules.Modules = JsonConvert.DeserializeObject<ICollection<ModuleAnnotationRow>>(obj.ToString());
            }
            if (item[nameof(ModuleAnnotationStructure.Practices)].Type != JTokenType.Null)
            {
                var obj = item[nameof(ModuleAnnotationStructure.Practices)].Value<object>();
                modules.Practices = JsonConvert.DeserializeObject<ICollection<ModuleAnnotationRow>>(obj.ToString());
            }
            if (item[nameof(ModuleAnnotationStructure.Gia)].Type != JTokenType.Null)
            {
                var obj = item[nameof(ModuleAnnotationStructure.Gia)].Value<object>();
                modules.Gia = JsonConvert.DeserializeObject<ICollection<ModuleAnnotationRow>>(obj.ToString());
            }

            var plans = _db.Plans.Where(p => p.learnProgramUUID == _annotation.BasicCharacteristicOP.Info.ProfileId && p.eduplanNumber == _annotation.PlanNumber && p.versionNumber == _annotation.PlanVersionNumber && !p.remove);

            string giaType = "Итоговая государственная аттестация";
            string practiceType = "Учебная и производственная практики";

            if (modules.RequiredModules.Count == 0 && _annotation.Status.CanEdit())
                modules.RequiredModules = plans.Where(p => p.moduleGroupType == "Обязательная часть" && p.Module.type != giaType && p.Module.type != practiceType)
                    .GroupBy(p => p.Module).Select(p => p.Key).ToList()
                    .Select(m => new ModuleAnnotationRow(m))
                    .OrderBy(m => m.Name).ToList();

            if (modules.Modules.Count == 0 && _annotation.Status.CanEdit())
                modules.Modules = plans.Where(p => p.moduleGroupType == "Формируемая участниками образовательных отношений"
                                        && p.Module.type != giaType && p.Module.type != practiceType)
                    .GroupBy(p => p.Module).Select(p => p.Key).ToList()
                    .Select(m => new ModuleAnnotationRow(m))
                    .OrderBy(m => m.Name).ToList();

            if (modules.Practices.Count == 0 && _annotation.Status.CanEdit())
                modules.Practices = plans.Where(p => p.Module.type == practiceType)
                    .GroupBy(p => p.Module).Select(p => p.Key).ToList()
                    .Select(m => new ModuleAnnotationRow(m))
                    .OrderBy(m => m.Name).ToList();

            if (modules.Gia.Count == 0 && _annotation.Status.CanEdit())
                modules.Gia = plans.Where(p => p.Module.type == giaType)
                    .GroupBy(p => p.Module).Select(p => p.Key).ToList()
                    .Select(m => new ModuleAnnotationRow(m))
                    .OrderBy(m => m.Name).ToList();
            
            modules.PossibleVariants = _ohopData.Variants.VariantInfos
                .Where(v => v.IdSource == IdSource.Variants || v.IdSource == IdSource.VariantUni)
                .Select(v => new VariantSourceInfo()
                {
                    Id = v.Id,
                    IdSource = v.IdSource,
                    Name = v.Name
                }).OrderBy(v => v.Name).ToList();

            return modules;
        }
    }
}