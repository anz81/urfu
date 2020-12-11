using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Loaders
{
    public class FileNameLoader : ObjectBlockContentLoader<string>
    {
        private readonly DisciplineWorkingProgram _wp;

        public FileNameLoader(DisciplineWorkingProgram wp)
        {
            _wp = wp;
        }

        protected override string LoadAnyContent(JToken blockContent)
        {
            return $"Дисциплина «{_wp.Discipline.title}», рабочая программа, версия {_wp.Version}".CleanFileName().ToDownloadFileName();
        }
    }
}