using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Loaders
{
    public class CommonCharacteristicLoader : ObjectBlockContentLoader<object>
    {
        private readonly BasicCharacteristicOP _ohop;

        public CommonCharacteristicLoader(BasicCharacteristicOP ohop)
        {
            _ohop = ohop;
        }

        protected override object LoadAnyContent(JToken blockContent)
        {
            var str = blockContent?.Value<string>();
            if (!_ohop.Status.CanEdit())
            {
                return str ?? "";
            }

            if (str == null)
            {
                str = "Основная образовательная программа реализуется совместно с …";
            }
            return str;
        }
    }    
}