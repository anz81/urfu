using System;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Processors
{
    public class SectionsProcessor : IBlockContentProcessor
    {
        private readonly IVersionedDocumentInspector _inspector;

        public SectionsProcessor(IVersionedDocumentInspector inspector)
        {
            _inspector = inspector;
        }

        public JToken ProcessContent(JToken data)
        {
            var items = (JArray)data;
            var index = 0;
            foreach (var section in items)
            {
                if (section[nameof(DisciplineSectionInfo.ItemId)]?.Value<string>() == null)
                    section[nameof(DisciplineSectionInfo.ItemId)] = Guid.NewGuid().ToString();
                var sectionCode = "Р" + ++index;
                section[nameof(DisciplineSectionInfo.Code)] = sectionCode;
                if (string.IsNullOrWhiteSpace(section[nameof(DisciplineSectionInfo.Name)]?.Value<string>()))
                {
                    _inspector.Error($"Необходимо указать название раздела '{sectionCode}'");
                    _inspector.StopProcessing();
                }
                if (string.IsNullOrWhiteSpace(section[nameof(DisciplineSectionInfo.Content)]?.Value<string>()))
                {
                    _inspector.Error($"Необходимо указать содержание раздела '{sectionCode}'");
                    _inspector.StopProcessing();
                }
            }
            return items;
        }
    }
}