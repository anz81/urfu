using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.ModuleAnnotations.Loaders
{
    public class PlanLoader : ObjectBlockContentLoader<PlanShortInfo>
    {
        private readonly ModuleAnnotation _annotation;

        public PlanLoader(ModuleAnnotation annotation)
        {
            _annotation = annotation;
        }

        protected override PlanShortInfo LoadAnyContent(JToken blockContent)
        {
            return new PlanShortInfo()
            {
                Number = _annotation.PlanNumber,
                Version = _annotation.PlanVersionNumber
            };
        }
    }
}