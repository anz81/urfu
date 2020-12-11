using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Processors
{
    public class ModuleStructureProcessor : IBlockContentProcessor
    {
        private readonly IVersionedDocumentInspector _inspector;
        private readonly ApplicationDbContext _db;

        public ModuleStructureProcessor(IVersionedDocumentInspector inspector, ApplicationDbContext db)
        {
            _inspector = inspector;
            _db = db;
        }

        public JToken ProcessContent(JToken data)
        {
            var item = data as JObject;

            var structure = new ModuleStructure();

            if (item[nameof(ModuleStructure.Practices)].Type != JTokenType.Null)
            {
                var listObj = item[nameof(ModuleStructure.Practices)].Value<object>();
                structure.Practices = JsonConvert.DeserializeObject<ICollection<ModuleInfoSelected>>(listObj.ToString());
                item[nameof(ModuleStructure.Practices)] = JArray.Parse(JsonConvert.SerializeObject(structure.Practices));
                item[nameof(ModuleStructure.SelectedPractices)] = JArray.Parse(JsonConvert.SerializeObject(structure.SelectedPractices));
            }

            if (item[nameof(ModuleStructure.Modules)].Type != JTokenType.Null)
            {
                var listObj = item[nameof(ModuleStructure.Modules)].Value<object>();
                structure.Modules = JsonConvert.DeserializeObject<ICollection<ModuleInfoSelected>>(listObj.ToString());
                item[nameof(ModuleStructure.Modules)] = JArray.Parse(JsonConvert.SerializeObject(structure.Modules));
                item[nameof(ModuleStructure.SelectedModules)] = JArray.Parse(JsonConvert.SerializeObject(structure.SelectedModules));
            }

            if (item[nameof(ModuleStructure.Gia)].Type != JTokenType.Null)
            {
                var listObj = item[nameof(ModuleStructure.Gia)].Value<object>();
                structure.Gia = JsonConvert.DeserializeObject<ICollection<ModuleInfoSelected>>(listObj.ToString());
                item[nameof(ModuleStructure.Gia)] = JArray.Parse(JsonConvert.SerializeObject(structure.Gia));
                item[nameof(ModuleStructure.SelectedGia)] = JArray.Parse(JsonConvert.SerializeObject(structure.SelectedGia));
            }

            if (item[nameof(ModuleStructure.Facultative)].Type != JTokenType.Null)
            {
                var listObj = item[nameof(ModuleStructure.Facultative)].Value<object>();
                structure.Facultative = JsonConvert.DeserializeObject<ICollection<ModuleInfoSelected>>(listObj.ToString());
                item[nameof(ModuleStructure.Facultative)] = JArray.Parse(JsonConvert.SerializeObject(structure.Facultative));
                item[nameof(ModuleStructure.SelectedFacultative)] = JArray.Parse(JsonConvert.SerializeObject(structure.SelectedFacultative));
            }
            
            return item;
        }
    }
}