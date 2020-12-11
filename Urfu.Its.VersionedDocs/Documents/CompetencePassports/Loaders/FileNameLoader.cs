using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.CompetencePassports.Loaders
{
    public class FileNameLoader : ObjectBlockContentLoader<string>
    {
        private readonly CompetencePassport _passport;

        public FileNameLoader(CompetencePassport passport)
        {
            _passport = passport;
        }

        protected override string LoadAnyContent(JToken blockContent)
        {
            return $"{_passport.BasicCharacteristicOP.Info.Profile.NAME}, Паспорт компетенций, версия {_passport.Version}"
                .CleanFileName().ToDownloadFileName();
        }
    }
}