using System;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Newtonsoft.Json;

namespace Urfu.Its.VersionedDocs.Documents.Practices.Processors
{
    public class PracticeSectionsProcessor : IBlockContentProcessor
    {
        private readonly IVersionedDocumentInspector _inspector;

        public PracticeSectionsProcessor(IVersionedDocumentInspector inspector)
        {
            _inspector = inspector;
        }

        public JToken ProcessContent(JToken data)
        {
            var items = (JArray)data;
            foreach (var item in items)
            {
                var structure = JsonConvert.DeserializeObject<PracticeSectionsStructure>(item.ToString());

                foreach (var info in structure.Sections)
                {
                    foreach (var sectionInfo in info.SectionInfo)
                    {
                        if (string.IsNullOrWhiteSpace(sectionInfo.Content) || string.IsNullOrWhiteSpace(sectionInfo.Name))
                        {
                            _inspector.Error($"Необходимо указать название и содержание всех разделов");
                            _inspector.StopProcessing();
                        }
                    }
                }
            }
            return items;
        }
    }
}