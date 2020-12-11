using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Loaders
{
    public class DisciplineNameLoader : ObjectBlockContentLoader<string>
    {
        private readonly DisciplineWorkingProgram _program;

        public DisciplineNameLoader(DisciplineWorkingProgram program)
        {
            _program = program;
        }

        protected override string LoadAnyContent(JToken blockContent)
        {
            return _program.Discipline.title;
        }
    }
}