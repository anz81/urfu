using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class EducationalMethodicalCouncilLoader : ObjectBlockContentLoader<EducationalMethodicalCouncilInfo>
    {
        private readonly ApplicationDbContext _db;

        public EducationalMethodicalCouncilLoader(ApplicationDbContext db)
        {
            _db = db;
        }

        protected override EducationalMethodicalCouncilInfo LoadAnyContent(JToken blockContent)
        {
            var info = new EducationalMethodicalCouncilInfo();

            var item = blockContent as JObject;
            if (item == null)
                return info;

            info.ProtocolDate = item.Value<string>("ProtocolDate");
            info.ProtocolNumber = item.Value<string>("ProtocolNumber");

            if (item["Chairman"]["TeacherId"].Type != JTokenType.Null)
            {
                var teacher = _db.Teachers.Find(item["Chairman"]["TeacherId"].Value<string>());
                info.Chairman = new WorkingProgramPersonInfo
                {
                    TeacherId = teacher.pkey,
                    ShortName = PersonHelper.PrepareShortName(teacher.lastName, teacher.firstName,
                        teacher.middleName)
                };
            }

            if (item["Chairman"]["AuthorId"].Type != JTokenType.Null)
            {
                var author = _db.WorkingProgramAuthors.Find(item["Chairman"]["AuthorId"].Value<int>());
                info.Chairman = new WorkingProgramPersonInfo
                {
                    AuthorId = author.Id,
                    ShortName = PersonHelper.PrepareShortName(author.LastName, author.FirstName, author.MiddleName)
                };
            }

            return info;
        }
    }
}