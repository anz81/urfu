using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class InstituteLoader : ObjectBlockContentLoader<object>
    {
        private readonly ApplicationDbContext _db;
        private readonly VersionedDocumentBlock _block;
        private readonly Web.DataContext.Module _module;

        public InstituteLoader(ApplicationDbContext db, VersionedDocumentBlock block, Web.DataContext.Module module)
        {
            _db = db;
            _block = block;
            _module = module;
        }

        protected override object LoadAnyContent(JToken blockContent)
        {
//            if (_block.Version == 0)
//            {
//                var institutes = _db.Divisions.Where(d =>
//                    d.typeCode == "institute" || d.typeCode == "faculty" || d.typeCode == "branch").ToList();
//                var coordinator = _module.coordinator;
//                var instituteEntity = institutes.FirstOrDefault(i => string.Equals(i.typeTitle + " «" + i.title + "»",
//                    coordinator, StringComparison.InvariantCultureIgnoreCase));
//                if (instituteEntity == null)
//                    return new InstituteInfo();
//                var institute = new InstituteInfo
//                {
//                    Id = instituteEntity.uuid,
//                    Name = instituteEntity.title
//                };
//                return institute;
//            }
//            else
//            {
                var instituteData = blockContent as JObject;
                if (instituteData == null)
                    return new InstituteInfo();

                var instituteId = instituteData.GetValue("Id").Value<string>();
                var institute = new InstituteInfo
                {
                    Name = instituteId == null ? null : _db.Divisions.Find(instituteId).title,
                    Id = instituteId
                };
                return institute;
//            }
        }
    }
}