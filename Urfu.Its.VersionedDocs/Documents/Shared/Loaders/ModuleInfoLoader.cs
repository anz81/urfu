using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class ModuleInfoLoader : ObjectBlockContentLoader<object>
    {
        private readonly Web.DataContext.Module _module;

        public ModuleInfoLoader(Web.DataContext.Module module)
        {
            _module = module;
        }

        protected override object LoadAnyContent(JToken blockContent)
        {
            return new ModuleInfo
            {
                Name = _module.title,
                Id = _module.uuid,
                Code = _module.number,
                Capacity = _module.testUnits
            };
        }
    }    
}