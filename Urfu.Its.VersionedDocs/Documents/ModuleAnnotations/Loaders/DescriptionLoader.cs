using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.ModuleAnnotations.Loaders
{
    public class DescriptionLoader : ObjectBlockContentLoader<string>
    {
        private readonly BasicCharacteristicOPSchemaModel _ohopData;

        public DescriptionLoader(BasicCharacteristicOPSchemaModel ohopData)
        {
            _ohopData = ohopData;
        }

        protected override string LoadAnyContent(JToken blockContent)
        {
            return _ohopData.PurposeAndFeature;
        }
    }
}