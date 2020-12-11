using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Processors
{
    public class TechCardDisciplineCertificationProcessor : IBlockContentProcessor
    {
        public JToken ProcessContent(JToken data)
        {
            var obj = JsonConvert.DeserializeObject<TechCardDisciplineCertificationInfo>(data.ToString());
            obj.Labs.Controls = obj.Labs.Controls.OrderBy(c => c.Semester).ThenBy(c => c.Week).ToList();
            obj.Practices.Controls = obj.Practices.Controls.OrderBy(c => c.Semester).ThenBy(c => c.Week).ToList();
            obj.Lections.Controls = obj.Lections.Controls.OrderBy(c => c.Semester).ThenBy(c => c.Week).ToList();
            return JObject.FromObject(obj);
        }
    }
}