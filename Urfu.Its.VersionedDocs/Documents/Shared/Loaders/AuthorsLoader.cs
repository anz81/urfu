using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class AuthorsLoader : ObjectBlockContentLoader<IEnumerable<object>>
    {        
        private readonly ApplicationDbContext _db;

        public AuthorsLoader(ApplicationDbContext db)
        {            
            _db = db;
        }

        protected override IEnumerable<object> LoadAnyContent(JToken blockContent)
        {
            var array =  blockContent as JArray;
            if(array == null)
                yield break;

            var teacherIds = array.Select(item => item.Value<string>("TeacherId")).Where(i=>i != null).ToList();
            var teachers = _db.Teachers.Where(t => teacherIds.Contains(t.pkey)).ToList();

            var customIds = array.Select(item => item.Value<int?>("AuthorId")).Where(i => i != null).ToList();
            var customAuthors = _db.WorkingProgramAuthors.Where(t => customIds.Contains(t.Id)).ToList();

            foreach (var item in array)
            {
                if (item["TeacherId"].Type != JTokenType.Null)
                {
                    var teacher = teachers.First(t => t.pkey == item["TeacherId"].Value<string>());
                    yield return new AuthorInfo
                    {                        
                        Post = teacher.post,
                        Fio = $"{teacher.lastName} {teacher.firstName} {teacher.middleName}",
                        ShortName = PersonHelper.PrepareShortName(teacher.lastName, teacher.firstName, teacher.middleName),
                        Degree = teacher.academicDegree == null && teacher.academicTitle == null 
                            ? null 
                            : $"{teacher.academicDegree}{(teacher.academicDegree != null && teacher.academicTitle != null ? ", " : string.Empty)}{teacher.academicTitle}",
                        Cathedra = $"{teacher.workPlace}",
                        TeacherId = teacher.pkey,
                        AuthorId = (int?) null
                    };
                }
                else if (item["AuthorId"].Type != JTokenType.Null)
                {
                    var author = customAuthors.First(t => t.Id == item["AuthorId"].Value<int>());
                    yield return new AuthorInfo
                    {
                        Post = author.Post,
                        Fio = $"{author.LastName} {author.FirstName} {author.MiddleName}",
                        ShortName = PersonHelper.PrepareShortName(author.LastName, author.FirstName, author.MiddleName),
                        Degree = $"{author.AcademicDegree}{(author.AcademicDegree != null && author.AcademicTitle != null ? ", " : string.Empty)}{author.AcademicTitle}",
                        Cathedra = $"{author.Workplace}",
                        TeacherId = null,
                        AuthorId = author.Id
                    };
                }
                else
                    yield return item;
            }            
        }
    }
}