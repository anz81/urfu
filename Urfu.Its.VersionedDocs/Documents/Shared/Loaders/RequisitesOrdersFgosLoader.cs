using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class RequisitesOrdersFgosLoader : ObjectBlockContentLoader<IEnumerable<RequisitesOrderFgosInfo>>
    {
        private readonly ApplicationDbContext _db;

        public RequisitesOrdersFgosLoader(ApplicationDbContext db)
        {
            _db = db;
        }

        protected override IEnumerable<RequisitesOrderFgosInfo> LoadAnyContent(JToken blockContent)
        {
            var array = blockContent as JArray;
            if (array == null)
                yield break;

            var requisiteIds = array.Select(item => item.Value<int>("Id")).ToList();
            var requisites = _db.RequisiteOrderFgoss.Include(r=>r.Direction).Where(r=>requisiteIds.Contains(r.Id)).ToList();
            foreach (var item in array)
            {
                var id = item.Value<int>("Id");
                var r = requisites.FirstOrDefault(d => d.Id == id);
                if (r != null)
                {
                    yield return new RequisitesOrderFgosInfo
                    {
                        Id = id,
                        Number = r.Order,
                        Date = r.Date.ToString("dd.MM.yyyy"),
                        DirectionCode = r.Direction.okso
                    };
                }
                else
                    throw new InvalidOperationException($"Реквизиты с идентификатором '{id}' не найдены");
            }
        }
    }
}