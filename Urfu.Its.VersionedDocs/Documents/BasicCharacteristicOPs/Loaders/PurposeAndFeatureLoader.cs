using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Loaders
{
    public class PurposeAndFeatureLoader : ObjectBlockContentLoader<object>
    {
        private readonly BasicCharacteristicOP _ohop;
        public PurposeAndFeatureLoader(BasicCharacteristicOP ohop)
        {
            _ohop = ohop;
        }

        protected override object LoadAnyContent(JToken blockContent)
        {
            var str = blockContent?.Value<string>();
            if (string.IsNullOrWhiteSpace(str))
            {
                return new BasicCharacteristicOPDefaultValues(_ohop.Info).PurposeAndFeature();
            }
            return str;
        }
    }    
}