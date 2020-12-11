using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.ModuleAnnotations.Loaders
{
    public class FileNameLoader : ObjectBlockContentLoader<string>
    {
        private readonly ModuleAnnotation _annotation;

        public FileNameLoader(ModuleAnnotation passport)
        {
            _annotation = passport;
        }

        protected override string LoadAnyContent(JToken blockContent)
        {
            return $"{_annotation.BasicCharacteristicOP.Info.Profile.NAME}, версия {_annotation.BasicCharacteristicOP.Version}, Аннотация модулей"
                .CleanFileName().ToDownloadFileName();
        }
    }
}