using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class DirectionsLoader : ObjectBlockContentLoader<IEnumerable<DirectionInfo>>
    {
        private readonly ApplicationDbContext _db;
        
        public DirectionsLoader(ApplicationDbContext db)
        {
            _db = db;            
        }

        protected override IEnumerable<DirectionInfo> LoadAnyContent(JToken blockContent)
        {
            var array = blockContent as JArray;
            if (array == null)
                return Enumerable.Empty<DirectionInfo>();

            var directionIds = array.Select(i => i.Value<string>("Id")).ToList();
            
            var directions = _db.Directions.Where(d=>directionIds.Contains(d.uid))
                .Select(direction=> new DirectionInfo
                {
                    Id = direction.uid,
                    Code = direction.okso,
                    Title = direction.title,
                    Qualifications = direction.qualifications,
                    Standard = direction.standard
                })
                .ToList();

            return directions;
        }
    }
}