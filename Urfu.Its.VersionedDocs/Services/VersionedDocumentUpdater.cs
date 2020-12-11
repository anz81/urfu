using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Services
{
    public class VersionedDocumentUpdater : IVersionedDocumentUpdater
    {
        private readonly IVersionedDocumentDescriptorService _descriptorService;
        private readonly IObjectLogger<VersionedDocumentUpdater> _logger;
        private readonly IVersionedDocumentService _documentService;
        private readonly IVersionedDocumentSchemaService _schemaService;

        public VersionedDocumentUpdater(IVersionedDocumentDescriptorService descriptorService,
            IObjectLogger<VersionedDocumentUpdater> logger,
            IVersionedDocumentService documentService,
            IVersionedDocumentSchemaService schemaService)
        {
            _descriptorService = descriptorService;
            _logger = logger;
            _documentService = documentService;
            _schemaService = schemaService;
        }

        public void UpdateLinkedDocumentsForTemplate(VersionedDocumentTemplate template,
            VersionedDocumentDescriptor newDescriptor,
            out IReadOnlyDictionary<VersionedDocument, string[]> documentErrors, DateTime updateTime)
        {
            var descriptorFactory = new JSchemaVersionedDocumentDescriptorFactory(JSchema.Parse(template.Schema));
            var templateSchemaDescriptor = descriptorFactory.CreateDocumentDescriptor();
            var newSchema = newDescriptor.GenerateSchemaString();
            var changes = _descriptorService.GetChanges(templateSchemaDescriptor, newDescriptor).ToList();

            var removedChanges = changes.Where(c => c.Action == VersionedDocumentSchemaChangeAction.ItemRemoved).ToList();
            var typeChanges = changes.Where(c => c.Action == VersionedDocumentSchemaChangeAction.ItemTypeChanged).ToList();
            var addedChanges = changes.Where(c => c.Action == VersionedDocumentSchemaChangeAction.ItemAdded).ToList();

            if (typeChanges.Any())
            {
                _logger?.Error($"Type changes of schema elements have been detected.{Environment.NewLine}{string.Join($";{Environment.NewLine}", typeChanges.Select(c => $"Type change {FormatItemPath(c.Chain, c.NewItem)}: {c.OldItem.Kind} -> {c.NewItem.Kind}"))}");
                throw new VersionedDocumentUpdaterNotSupportedTheseChangesException("Type changes detected. Cannot perform updates to documents.");
            }

            if (removedChanges.Any())
            {
                _logger?.Error($"New schema have not some elements.{Environment.NewLine}{string.Join($";{Environment.NewLine}", removedChanges.Select(c => $"Deleted element {FormatItemPath(c.Chain, c.OldItem)}"))}");
                throw new VersionedDocumentUpdaterNotSupportedTheseChangesException("Deleted elements detected. Cannot perform updates to documents.");
            }

            var documentErrorsDic = new Dictionary<VersionedDocument, string[]>();
            documentErrors = documentErrorsDic;

            var documents = template.Documents;

            if (addedChanges.Any())
            {
                _logger?.Info($"New schema have new elements:{Environment.NewLine}{string.Join($";{Environment.NewLine}", addedChanges.Select(c => FormatItemPath(c.Chain, c.NewItem)))}");
            }
            else
            {
                _logger?.Info("No schema changes.");
                return;
            }
            
            _logger?.Info($"Number of documents to update [{documents.Count}].");

            foreach (var document in documents)
            {
                var errors = new List<string>();

                // TODO Ошибки нужно постараться конкретизировать
                foreach (var change in addedChanges)
                {
                    Debug.Assert(change.Action == VersionedDocumentSchemaChangeAction.ItemAdded);

                    try
                    {
                        AddItem(document, change.Chain, change.NewItem, updateTime);
                    }
                    catch (Exception ex)
                    {
                        errors.Add("{" + FormatItemPath(change.Chain, change.NewItem) + "}: " + ex);
                    }
                }

                var model = _documentService.CreateSerializedModel(document);
                if (!_schemaService.ValidateBySchema(model, newSchema, out var validationErrors))
                {
                    errors.AddRange(validationErrors.Select(v=> "Ошибка [" + v.ErrorType + "] валидации схемы по пути [" + v.Path + "]. " + v.Message));
                }

                if (errors.Any())
                    documentErrorsDic.Add(document, errors.ToArray());
            }            
        }

        private static string FormatItemPath(IVersionedDocumentBlockItemDescriptor[] chain, IVersionedDocumentBlockItemDescriptor item)
        {
            return $"{string.Join(".", chain.Select(p => p.Name))}{(chain.Any() ? "." : "")}{item.Name}";
        }

        private void AddItem(VersionedDocument document, IVersionedDocumentBlockItemDescriptor[] chain,
            IVersionedDocumentBlockItemDescriptor newItem, DateTime updateTime)
        {
            if (!chain.Any())
            {
                var blockDescriptor = (VersionedDocumentBlockDescriptor) newItem;
                var defaultContent = _descriptorService.BuildDefaultContent(blockDescriptor);

                var versionedDocumentBlock = new VersionedDocumentBlock
                {
                    Data = BlockDataHelper.PrepareData(defaultContent),
                    CreatedAt = updateTime,
                    Name = blockDescriptor.Name
                };

                document.BlockLinks.Add(new VersionedDocumentBlockLink
                {
                    DocumentBlock = versionedDocumentBlock,
                    Document = document,
                    UpdateTime = updateTime
                });
            }
            else
            {
                foreach (var link in document.BlockLinks)
                {
                    var blockName = link.DocumentBlock.Name;
                    var blockDescriptor = (VersionedDocumentBlockDescriptor)chain.First();
                    if (blockDescriptor.Name != blockName) continue;

                    var blockData = JObject.Parse(link.DocumentBlock.Data);
                    
                    // TODO тут возможно в паттерне ошибка. Пока работает не трогаю
                    var pattern = $"^{string.Join(@"(\[\d+\])?\.", new[]{ BlockDataHelper.ContentProperty }.Concat(chain.Skip(1).Select(c => c.Name)))}$";
                    var regex = new Regex(pattern);
                    
                    var items = blockData.Property(BlockDataHelper.ContentProperty).Descendants().Where(d => d.Type != JTokenType.Property && regex.IsMatch(d.Path)).ToList();

                    var dataItem = _descriptorService.BuildDefaultContent(newItem);
                    var dataToken = dataItem != null ? JToken.FromObject(dataItem) : null;

                    foreach (var arrayItem in items)
                    {
                        if (arrayItem is JArray array)
                            foreach (var subItem in array)
                                subItem[newItem.Name] = dataToken;
                        else
                            arrayItem[newItem.Name] = dataToken;
                    }

                    link.DocumentBlock.Data = blockData.ToString();
                    link.UpdateTime = updateTime;

                    return;
                }
            }
        }
    }
}