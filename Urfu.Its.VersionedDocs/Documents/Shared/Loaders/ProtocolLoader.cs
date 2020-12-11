using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Shared.Loaders
{
    public class ProtocolLoader : ObjectBlockContentLoader<Protocol>
    {
        private readonly ApplicationDbContext _db;

        public ProtocolLoader(ApplicationDbContext db)
        {
            _db = db;
        }

        protected override Protocol LoadAnyContent(JToken blockContent)
        {
            var protocol = new Protocol();

            var item = blockContent as JObject;
            if (item == null)
                return protocol;

            protocol.ProtocolDate = item.Value<string>("ProtocolDate");
            protocol.ProtocolNumber = item.Value<string>("ProtocolNumber");
            if (protocol.ProtocolDate == null) return protocol;

            var date = DateTime.ParseExact(protocol.ProtocolDate, "dd.MM.yyyy", null);
            protocol.Day = date.Day.ToString();
            protocol.Month = date.Month.ToString();
            protocol.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames[date.Month - 1].ToLower();
            protocol.Year = date.Year.ToString();

            return protocol;
        }
    }
}