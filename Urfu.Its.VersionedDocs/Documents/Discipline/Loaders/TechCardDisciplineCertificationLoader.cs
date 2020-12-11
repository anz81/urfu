using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Loaders
{
    public class TechCardDisciplineCertificationLoader : ObjectBlockContentLoader<TechCardDisciplineCertificationInfo>
    {
        protected override TechCardDisciplineCertificationInfo LoadAnyContent(JToken blockContent)
        {
            var disciplineCertification = blockContent.ToObject<TechCardDisciplineCertificationInfo>();
            disciplineCertification.Lections.InitDefaults();
            disciplineCertification.Practices.InitDefaults();
            disciplineCertification.Labs.InitDefaults();
            return disciplineCertification;
        }
    }
}