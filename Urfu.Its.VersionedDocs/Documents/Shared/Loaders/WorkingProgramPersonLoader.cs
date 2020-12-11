using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class WorkingProgramPersonLoader : ObjectBlockContentLoader<object>
    {        
        private readonly ApplicationDbContext _db;

        public WorkingProgramPersonLoader(ApplicationDbContext db)
        {            
            _db = db;
        }

        protected override object LoadAnyContent(JToken blockContent)
        {
            var item =  blockContent as JObject;
            if(item == null)
                return new WorkingProgramPersonInfo();

            if (item["TeacherId"].Type != JTokenType.Null)
            {
                var teacher = _db.Teachers.Find(item["TeacherId"].Value<string>());
                return new WorkingProgramPersonInfo
                {
                    TeacherId = teacher.pkey,
                    ShortName = PersonHelper.PrepareShortName(teacher.lastName, teacher.firstName, teacher.middleName)
                };
            }

            if (item["AuthorId"].Type != JTokenType.Null)
            {
                var author = _db.WorkingProgramAuthors.Find(item["AuthorId"].Value<int>());
                return new WorkingProgramPersonInfo
                {
                    AuthorId = author.Id,
                    ShortName = PersonHelper.PrepareShortName(author.LastName, author.FirstName, author.MiddleName)
                };
            }

            return item;
        }
    }
}