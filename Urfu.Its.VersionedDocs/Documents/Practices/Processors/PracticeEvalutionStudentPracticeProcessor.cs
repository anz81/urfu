using System;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Newtonsoft.Json;
using System.Linq;

namespace Urfu.Its.VersionedDocs.Documents.Practices.Processors
{
    public class PracticeEvalutionStudentPracticeProcessor : IBlockContentProcessor
    {
        private readonly IVersionedDocumentInspector _inspector;

        public PracticeEvalutionStudentPracticeProcessor(IVersionedDocumentInspector inspector)
        {
            _inspector = inspector;
        }

        public JToken ProcessContent(JToken data)
        {
            var items = (JArray)data;
            foreach (var item in items)
            {
                var structure = JsonConvert.DeserializeObject<PracticeEvalutionStudentPracticeStructure>(item.ToString());
                foreach (var i in structure.Items)
                {
                    i.TechCardDisciplineCertification.EduLoad.Controls = i.TechCardDisciplineCertification.EduLoad.Controls.OrderBy(c => c.Semester).ThenBy(c => c.Week).ToList();
                }
            }
            return items;
        }
    }
}