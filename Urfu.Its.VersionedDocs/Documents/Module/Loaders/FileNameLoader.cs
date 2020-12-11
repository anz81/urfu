using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Module.Loaders
{
    public class FileNameLoader : ObjectBlockContentLoader<string>
    {
        private readonly ModuleWorkingProgram _wp;

        public FileNameLoader(ModuleWorkingProgram wp)
        {
            _wp = wp;
        }

        protected override string LoadAnyContent(JToken blockContent)
        {
            return $"Модуль «{_wp.Module.title}», рабочая программа, версия {_wp.Version}".CleanFileName().ToDownloadFileName();
        }
    }
}