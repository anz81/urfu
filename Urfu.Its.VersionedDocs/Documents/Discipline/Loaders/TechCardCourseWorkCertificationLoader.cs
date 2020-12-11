using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Loaders
{
    public class TechCardCourseWorkCertificationLoader : ObjectBlockContentLoader<TechCardCertificationItemInfo>
    {
        protected override TechCardCertificationItemInfo LoadAnyContent(JToken blockContent)
        {
            var certification = blockContent.ToObject<TechCardCertificationItemInfo>();
            certification.InitDefaults();
            return certification;
        }
    }
}