using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using TemplateEngine;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Services
{
    public class FromDatabaseVersionedDocumentService : IVersionedDocumentService
    {
        private readonly IVersionedDocumentSchemaService _schemaService;

        public FromDatabaseVersionedDocumentService(IVersionedDocumentSchemaService schemaService)
        {
            _schemaService = schemaService;
        }

        public string CreateSerializedModel(VersionedDocument document, params string[] loadBlocks)
        {
            var loadedDocumentData = new JObject();
            foreach (var block in document.BlockLinks.Select(l => l.DocumentBlock))
            {
                if (loadBlocks.Any() && !loadBlocks.Contains(block.Name))
                    continue;
                loadedDocumentData.Merge(new JObject(new JProperty(block.Name, BlockDataHelper.GetContent(block.Data))));
            }

            return loadedDocumentData.ToString();
        }

        public object CreateProxyModel(VersionedDocument document, params string[] loadBlocks)
        {
            throw new NotImplementedException();
        }

        public object CreateModel(VersionedDocument document, params string[] loadBlocks)
        {
            throw new NotImplementedException();
        }

        public Stream Print(VersionedDocument document, FileFormat fileFormat)
        {
            throw new NotImplementedException();
        }

        public MemoryStream PrintZip(VersionedDocument document, FileFormat fileFormat)
        {
            throw new NotImplementedException();
        }

        public bool IsSchemaActual(VersionedDocument document)
        {
            throw new NotImplementedException();
        }

        public string ApplyDocumentChanges(VersionedDocument document, string serializedDocumentData,
            out VersionedDocumentBlockInspectionInfo[] inspections)
        {
            throw new NotImplementedException();
        }

        public bool ValidateBySchema(VersionedDocument document, out ValidationError[] validationErrors)
        {
            var model = CreateSerializedModel(document);
            var schema = document.Template.Schema;

            return _schemaService.ValidateBySchema(model, schema, out validationErrors);
        }

        public void ResaveDocument(VersionedDocument document)
        {
            throw new NotImplementedException();
        }
    }
}