using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Processors
{
    public class TechCardCourseWorksCertificationProcessor : IBlockContentProcessor
    {
        public JToken ProcessContent(JToken data)
        {
            var obj = JsonConvert.DeserializeObject<TechCardCertificationItemInfo>(data.ToString());
            obj.Controls = obj.Controls.OrderBy(c => c.Semester).ThenBy(c => c.Week).ToList();            
            return JObject.FromObject(obj);
        }
    }
}