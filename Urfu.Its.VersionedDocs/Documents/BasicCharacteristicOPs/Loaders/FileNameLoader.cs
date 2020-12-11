using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Loaders
{
    public class FileNameLoader : ObjectBlockContentLoader<string>
    {
        private readonly BasicCharacteristicOP _ohop;

        public FileNameLoader(BasicCharacteristicOP ohop)
        {
            _ohop = ohop;
        }

        protected override string LoadAnyContent(JToken blockContent)
        {
            return $"{_ohop.Info.Profile.NAME}, ОХОП, версия {_ohop.Version}".CleanFileName().ToDownloadFileName();
        }
    }
}