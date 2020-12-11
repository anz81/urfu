using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data.Entity.Core.Metadata.Edm;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
//using System.Web.Hosting;
using Autofac;
using Autofac.Features.Indexed;
using AutoMapper.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using QuickGraph;
using QuickGraph.Algorithms;
using TemplateEngine;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Loggers;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model;
using Microsoft.Extensions.Hosting.Internal;

namespace Urfu.Its.VersionedDocs.Services
{
    public class VersionedDocumentService : IVersionedDocumentService
    {
        private readonly ITemplateReportingEngine _templateEngine;
        private readonly IVersionedDocumentSchemaService _schemaService;

        private readonly IIndex<VersionedDocumentType, IVersionedDocumentImplementationService>
            _documentImplementationServices;

        private readonly ILifetimeScope _scope;
        private readonly IObjectLogger<VersionedDocumentService> _logger;
        private readonly IObjectLogger<VersionedDocumentService> _debugLogger;

        private readonly Regex _blockNameRegex = new Regex("([a-zA-Z_]*).*", RegexOptions.Compiled);

        public VersionedDocumentService(
            ITemplateReportingEngine templateEngine,
            IVersionedDocumentSchemaService schemaService,
            IIndex<VersionedDocumentType, IVersionedDocumentImplementationService> documentImplementationServices,
            ILifetimeScope scope,
            IObjectLogger<VersionedDocumentService> logger)
        {
            _templateEngine = templateEngine;
            _schemaService = schemaService;
            _documentImplementationServices = documentImplementationServices;
            _scope = scope;
            _logger = logger;
            _debugLogger = new VersionedDocumentsTraceLogger<VersionedDocumentService>(); // _logger;
        }

        public object CreateProxyModel(VersionedDocument document, params string[] loadBlocks)
        {
            _logger.Debug("Создание прокси модели документа с идентификатором {0}...", document.Id);
            var sw = Stopwatch.StartNew();
            var modelJson = CreateSerializedModel(document, loadBlocks);
            sw.Stop();
            var sw2 = Stopwatch.StartNew();
            var model = _schemaService.Create(document.Template.Schema, modelJson);
            sw2.Stop();
            return model;
        }

        public object CreateModel(VersionedDocument document, params string[] loadBlocks)
        {
            _logger.Debug("Создание типизированной модели документа с идентификатором {0}...", document.Id);
            // TODO реализация не тестировалась вообще, наверняка есть косяки
            var implementationService = _documentImplementationServices[document.Template.DocumentType];
            var modelJson = CreateSerializedModel(document, loadBlocks);
            var type = implementationService.GetActualSchemaType();
            var obj = JsonConvert.DeserializeObject(modelJson, type);
            return obj;
        }

        public Stream Print(VersionedDocument document, FileFormat fileFormat)
        {
            _logger.Debug("Формирование печатной формы документа с идентификатором {0}...", document.Id);
            var template = document.Template;

            var model = CreateProxyModel(document);

            var stream = new MemoryStream();
            using (var templateStream = new MemoryStream(template.Data))
                _templateEngine.Build(templateStream, model, stream, fileFormat);
            stream.Seek(0, SeekOrigin.Begin);

            var resultStream = AddDefaultPages(stream, document);
            resultStream.Seek(0, SeekOrigin.Begin);

            return resultStream;
        }

        public MemoryStream PrintZip(VersionedDocument document, FileFormat fileFormat)
        {
            _logger.Debug("Формирование архива с документом с идентификатором {0}...", document.Id);

            var stream = CreateZip(document, fileFormat);

            return stream;
        }

        private Stream AddDefaultPages(Stream printedDocument, VersionedDocument document)
        {
            try
            {
                if (document.Template.DocumentType == VersionedDocumentType.BasicCharacteristicOP)
                {
                    var model = (BasicCharacteristicOPSchemaModel)CreateModel(document);
                    var fileRelativePathsToConcat = new List<string>();
                    var he = new HostingEnvironment();
                    var app2FileIds = model.ApprovalActs.Where(a => a.FileId.HasValue).Select(a => a.FileId.Value).ToList();
                    if (app2FileIds.Count == 0) // если приложенных файлов нет, то добавляем страницу Приложение 2 по умолчанию в основной документ
                        fileRelativePathsToConcat.Add(Path.Combine(he.ContentRootPath, @"OHOP App2.docx"));

                    var app3FileIds = model.Files.Select(f => f.FileId);
                    if (app3FileIds.Count() == 0) // если приложенных файлов нет, то добавляем страницу Приложение 3 по умолчанию в основной документ
                        fileRelativePathsToConcat.Add(Path.Combine(he.ContentRootPath, @"OHOP App3.docx"));

                    // добавление страниц (Приложение 2, Приложение 3) по умолчанию в случае, если не приложены файлы 
                    var engine = new WordDocxTemplateReportingEngine();
                    foreach (var fullPath in fileRelativePathsToConcat)
                    {
                        using (var stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var result = new MemoryStream();
                            engine.Concat(printedDocument, stream, result);
                            printedDocument = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Ошибка при добавлении страниц по умолчанию в документ {document.Id}");
                Logger.Error(ex);
            }
            return printedDocument;
        }

        private MemoryStream CreateZip(VersionedDocument document, FileFormat fileFormat)
        {
            dynamic model = CreateProxyModel(document, "FileName");
            string fileName = model.FileName;

            var printedDocument = Print(document, fileFormat);
            printedDocument.Position = 0;

            var zipArchiveStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Update, true))
            {
                // добавление основного файла ОХОП в архив
                var fileEntry = zipArchive.CreateEntry($"{fileName}.docx", CompressionLevel.Fastest);
                using (var entryStream = fileEntry.Open())
                {
                    printedDocument.CopyTo(entryStream);
                }

                try
                {
                    if (document.Template.DocumentType == VersionedDocumentType.BasicCharacteristicOP)
                    {
                        var ohopModel = (BasicCharacteristicOPSchemaModel)CreateModel(document);

                        var app2FileIds = ohopModel.ApprovalActs.Where(a => a.FileId.HasValue).Select(a => a.FileId.Value).ToList();
                        var app3FileIds = ohopModel.Files.Select(f => f.FileId);

                        // добавление папок с приложенными документами
                        FileStorageHelper.AddFolderToZipArchive(zipArchive, app2FileIds, folder: "Приложение 2");
                        FileStorageHelper.AddFolderToZipArchive(zipArchive, app3FileIds, folder: "Приложение 3");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Info($"Ошибка при формировании архива ОХОП с документом {document.Id}");
                    Logger.Error(ex);
                }
            }

            zipArchiveStream.Position = 0;

            return zipArchiveStream;
        }

        public bool IsSchemaActual(VersionedDocument document)
        {
            var actualSchema = _documentImplementationServices[document.Template.DocumentType].GetDocumentDescriptor()
                .GenerateSchemaString();
            var documentSchema = document.Template.Schema;
            return _schemaService.IsSchemaMatched(actualSchema, documentSchema);
        }

        public string ApplyDocumentChanges(VersionedDocument document, string serializedDocumentData,
            out VersionedDocumentBlockInspectionInfo[] inspections)
        {
            if (!IsSchemaActual(document))
                throw new InvalidOperationException(
                    $"Схема документа с идентификатором '{document.Id}' устарела. Изменить документ невозможно.");

            var documentImplementationService = _documentImplementationServices[document.Template.DocumentType];

            if (!documentImplementationService.IsInEditableState(document))
                throw new InvalidOperationException("Документ нельзя редактировать.");

            var documentDescriptor = documentImplementationService.GetDocumentDescriptor();
            var schema = documentDescriptor.GenerateSchema();

            _logger.Debug("Чтение данных документа с идентификтором {0}", document.Id);
            var existingBlocks = document.BlockLinks
                .ToList()
                .Select(b => new
                {
                    Link = b,
                    Content = b.DocumentBlock.GetContent(),
                    b.DocumentBlock.Version
                })
                .Select(b => new
                {
                    BlockId = b.Link.DocumentBlockId,
                    b.Link,
                    b.Content,
                    b.Link.DocumentBlock.Name,
                    b.Version
                }).ToList();

            var time = DateTime.Now;

            var newDocumentData = JObject.Parse(serializedDocumentData); // статические данные документа, которые пришли с клиента для сохранения
            var changedDocumentData = new JObject(); // данные документа, которые действительно были изменены в процессе сохранения
            var actualDocumentData = new JObject(existingBlocks.Select(b => new JProperty(b.Name, b.Content))); // актуальные данные документа с учетом измененных данных блоков после сохранения каждого из них

            var inspectionList = new List<VersionedDocumentBlockInspectionInfo>();

            try
            {
                using (var scope = _scope.BeginLifetimeScope(b =>
                {
                    documentImplementationService.RegisterDocumentDependencies(b, document);
                }))
                {
                    var blocks = PrepareBlocksToSave(documentDescriptor, newDocumentData,
                        existingBlocks.Select(b => new JProperty(b.Name, b.Content)).ToArray()).ToList();

                    foreach (var block in blocks)
                    {
                        var blockDescriptor = block.Key;
                        var blockData = block.Value;
                        var existingItem = existingBlocks.First(b => b.Name == blockDescriptor.Name);

                        JToken processedContent = null;
                        if (blockDescriptor.ProcessorType != null)
                        {
                            using (var blockScope = scope.BeginLifetimeScope(b =>
                            {
                                b.RegisterType(blockDescriptor.ProcessorType)
                                    .WithParameter("newDocumentData", newDocumentData)
                                    .WithParameter("actualDocumentData", actualDocumentData)
                                    .WithParameter("changedDocumentData", changedDocumentData)
                                    .AsSelf();
                                b.RegisterInstance(blockDescriptor).AsSelf();
                                b.RegisterInstance(blockData).AsSelf();
                            }))
                            {
                                try
                                {
                                    var processor =
                                        (IBlockContentProcessor)blockScope.Resolve(blockDescriptor.ProcessorType);
                                    processedContent = processor.ProcessContent(
                                        blockData.Value.Type == JTokenType.Null ? null : blockData.Value.DeepClone());
                                }
                                finally
                                {
                                    var inspector = blockScope.Resolve<IVersionedDocumentInspector>();
                                    var errors = inspector.GetErrors().ToList();
                                    var warnings = inspector.GetWarnings().ToList();
                                    if (errors.Any() || warnings.Any())
                                        inspectionList.Add(new VersionedDocumentBlockInspectionInfo(blockDescriptor,
                                            blockData.Value, errors, warnings));
                                }
                            }
                        }

                        var fixedContent = FixContent(blockDescriptor, processedContent ?? blockData.Value);
                        actualDocumentData[blockDescriptor.Name] = fixedContent;

                        if (!JToken.DeepEquals(existingItem.Content, fixedContent))
                        {
                            changedDocumentData.Add(new JProperty(blockDescriptor.Name, fixedContent));
                            existingItem.Link.UpdateTime = time;
                            var newBlock = new VersionedDocumentBlock
                            {
                                Name = existingItem.Name,
                                Version = existingItem.Version + 1,
                                Data = BlockDataHelper.PrepareData(fixedContent),
                                CreatedAt = time,
                                PreviousBlockId = existingItem.BlockId
                            };
                            // Обновляем все ссылки, в которых этот блок присутствует. *Расшареные блоки
                            foreach (var link in existingItem.Link.DocumentBlock.Links)
                            {
                                link.DocumentBlock = newBlock;
                            }
                        }
                    }
                }
            }
            catch (VersionedDocumentInspectorStopProcessingException)
            {
                // Это не облажание, так и должно быть
            }

            if (!_schemaService.ValidateBySchema(actualDocumentData.ToString(), schema.ToString(), out var schemaErrors))
            {
                var errorsString = string.Join(Environment.NewLine, schemaErrors.Select((e, i) => $"{i + 1}. {e.Message}"));
                _logger.Debug($"{schemaErrors.Length} ошибок валидации документа с идентификатором {{0}} по схеме при попытке сохранения: {Environment.NewLine} {errorsString}", document.Id);

                var additionalInspections = ConvertValidationErrorsToInspections(schemaErrors, documentDescriptor, actualDocumentData).ToList();
                inspectionList.AddRange(additionalInspections);

                Debug.Fail("Ошибки валидации документа по схеме");
            }

            inspections = inspectionList.ToArray();

            return changedDocumentData.ToString();
        }

        public bool ValidateBySchema(VersionedDocument document, out ValidationError[] validationErrors)
        {
            var model = CreateSerializedModel(document);
            var schema = document.Template.Schema;

            return _schemaService.ValidateBySchema(model, schema, out validationErrors);
        }

        public void ResaveDocument(VersionedDocument document)
        {
            var docJson = CreateSerializedModel(document);
            ApplyDocumentChanges(document, docJson, out var inspections);
            if (inspections.SelectMany(i => i.Errors).Any())
                throw new InvalidOperationException($"В процессе пересохранения документа с идентификатором {document.Id} произошли ошибки");
        }

        public string CreateSerializedModel(VersionedDocument document, params string[] loadBlocks)
        {
            var documentImplementationService = _documentImplementationServices[document.Template.DocumentType];
            using (var scope = _scope.BeginLifetimeScope(b =>
                documentImplementationService.RegisterDocumentDependencies(b, document)))
            {
                var descriptor = documentImplementationService.GetDocumentDescriptor();
                var loadedDocumentData = new JObject();

                var checkEditStateWatcher = Stopwatch.StartNew();
                var isInEditableState = documentImplementationService.IsInEditableState(document);
                checkEditStateWatcher.Stop();
                _debugLogger.Debug($"Cheching editable state: {checkEditStateWatcher.Elapsed}");

                if (isInEditableState)
                {
                    var prepareBloockToLoadWatcher = Stopwatch.StartNew();
                    var blockDescriptors = PrepareBlocksToLoad(descriptor, loadBlocks).ToList();
                    prepareBloockToLoadWatcher.Stop();
                    _debugLogger.Debug($"Preparing blocks to load: {prepareBloockToLoadWatcher.Elapsed}");

                    var existingBlocksLoadWatcher = Stopwatch.StartNew();
                    var blockNamesToLoad = blockDescriptors.Select(b => b.Name).ToList();
                    var blocks = document.BlockLinks
                        .Select(l => l.DocumentBlock)
                        .Where(b => blockNamesToLoad.Contains(b.Name))
                        .ToList();
                    var blockContents = blocks.ToDictionary(b => b.Name, b => b.GetContent());
                    existingBlocksLoadWatcher.Stop();
                    _debugLogger.Debug($"Loading existing blocks: {existingBlocksLoadWatcher.Elapsed}");

                    var blockLoadersWatcher = Stopwatch.StartNew();
                    foreach (var blockDescriptor in blockDescriptors)
                    {
                        var blockContent = blockContents[blockDescriptor.Name];

                        var dataToMerge = new JObject(new JProperty(blockDescriptor.Name, blockContent));
                        if (blockDescriptor.NonVersionedDataLoaderType != null)
                        {
                            var block = blocks.First(b => b.Name == blockDescriptor.Name);
                            using (var blockScope = scope.BeginLifetimeScope(b =>
                            {
                                b.RegisterType(blockDescriptor.NonVersionedDataLoaderType)
                                    .WithParameter("loadedDocumentData", loadedDocumentData)
                                    .AsSelf();
                                b.RegisterInstance(block).As<VersionedDocumentBlock>();
                                b.RegisterInstance(blockDescriptor).AsSelf();
                            }))
                            {
                                var loader = (IBlockContentLoader)blockScope.Resolve(blockDescriptor.NonVersionedDataLoaderType);
                                if (loader.IsLoadRequired(blockContent))
                                {
                                    var loadedBlockContent = loader.LoadContent(blockContent);
                                    var loadedBlockData = new JObject(new JProperty(blockDescriptor.Name, loadedBlockContent));
                                    dataToMerge = loadedBlockData;
                                }
                            }
                        }

                        loadedDocumentData.Merge(dataToMerge);
                    }
                    blockLoadersWatcher.Stop();
                    _debugLogger.Debug($"Loading blocks data: {blockLoadersWatcher.Elapsed}");
                }
                else
                {
                    var sw = Stopwatch.StartNew();
                    foreach (var block in document.BlockLinks.Select(l => l.DocumentBlock))
                    {
                        if (loadBlocks.Any() && !loadBlocks.Contains(block.Name))
                            continue;
                        var blockData = block.GetContent();
                        loadedDocumentData.Merge(new JObject(new JProperty(block.Name, blockData)));
                    }
                    sw.Stop();
                    _debugLogger.Debug($"Loading document data from database: {sw.Elapsed}");
                }

                var serializerWatcher = Stopwatch.StartNew();
                var serializedModel = loadedDocumentData.ToString();
                serializerWatcher.Stop();
                _debugLogger.Debug($"Serializing model: {serializerWatcher.Elapsed}");
                return serializedModel;
            }
        }

        private IEnumerable<KeyValuePair<VersionedDocumentBlockDescriptor, JProperty>> PrepareBlocksToSave(
            VersionedDocumentDescriptor documentDescriptor, JObject documentData,
            IReadOnlyCollection<JProperty> existingBlocks)
        {
            _logger.Debug("Подготовка списка блоков к сохранению...");
            var blockList = documentData.Properties()
                .Select(b => documentDescriptor.Blocks.First(d => d.Name == b.Name)).ToList();

            _logger.Debug(
                $"Исходный список блоков [{blockList.Count}] к сохранению: {string.Join(", ", blockList.Select(b => b.Name))}");

            var blocksToSave = blockList.Union(blockList.SelectMany(d => d.GetDependentBlocks(documentDescriptor)))
                .ToDictionary(d => d,
                    d => documentData.Property(d.Name) ?? existingBlocks.First(b => b.Name == d.Name));
            _logger.Debug(
                $"К исходному списку блоков добавлены зависимые блоки: {string.Join(", ", blocksToSave.Keys.Except(blockList).Select(d => d.Name))}");

            _logger.Debug("Создание графа зависимостей блоков...");
            var graph = new AdjacencyGraph<VersionedDocumentBlockDescriptor, Edge<VersionedDocumentBlockDescriptor>>();
            graph.AddVertexRange(blocksToSave.Keys);
            foreach (var block in blocksToSave.Keys)
                graph.AddEdgeRange(block.GetDependentBlocks(documentDescriptor, false)
                    .Select(d => new Edge<VersionedDocumentBlockDescriptor>(block, d)));

            _logger.Debug("Выполнение сортировки по зависимостям...");
            var orderedBlocks = graph.TopologicalSort()
                .ToDictionary(d => d, d => blocksToSave.First(b => b.Key == d).Value);

            _logger.Debug(
                $"Блоки отсортированы по зависимостям для сохранения: {string.Join(", ", orderedBlocks.Keys.Select(d => d.Name))}");

            return orderedBlocks;
        }

        private IEnumerable<VersionedDocumentBlockDescriptor> PrepareBlocksToLoad(
            VersionedDocumentDescriptor documentDescriptor, string[] loadBlocks)
        {
            _logger.Debug("Подготовка списка блоков к загрузке...");

            var blockList =
                (loadBlocks.Any()
                    ? documentDescriptor.Blocks.Where(b => loadBlocks.Contains(b.Name))
                    : documentDescriptor.Blocks).ToList();

            _logger.Debug(
                $"Исходный список блоков [{blockList.Count}] к загрузке: {string.Join(", ", blockList.Select(b => b.Name))}");

            var blocksToLoad = blockList
                .Union(blockList.SelectMany(d => d.GetParentDependentBlocks(documentDescriptor)))
                .ToList();
            _logger.Debug(
                $"К исходному списку блоков добавлены зависимые блоки: {string.Join(", ", blocksToLoad.Except(blockList).Select(d => d.Name))}");

            _logger.Debug("Создание графа зависимостей блоков...");
            var graph = new AdjacencyGraph<VersionedDocumentBlockDescriptor, Edge<VersionedDocumentBlockDescriptor>>();
            graph.AddVertexRange(blocksToLoad);
            foreach (var block in blocksToLoad)
                graph.AddEdgeRange(block.GetParentDependentBlocks(documentDescriptor, false)
                    .Select(d => new Edge<VersionedDocumentBlockDescriptor>(block, d)));

            _logger.Debug("Выполнение сортировки по зависимостям...");
            var orderedBlocks = graph.TopologicalSort().Reverse().ToList();

            _logger.Debug(
                $"Блоки отсортированы по зависимостям для загрузки: {string.Join(", ", orderedBlocks.Select(d => d.Name))}");

            return orderedBlocks;
            //return orderedBlocks.ToDictionary(b => b, b => existingBlocks.First(p => p.Name == b.Name));
        }

        /// <summary>
        /// Процедура удаления лишних (не существующих в схеме) свойств элемента данных
        /// </summary>
        /// <param name="itemDescriptor"></param>
        /// <param name="itemData"></param>
        /// <returns></returns>
        private JToken FixContent(IVersionedDocumentBlockItemDescriptor itemDescriptor, JToken itemData)
        {
            switch (itemDescriptor.Kind)
            {
                case VersionedDocumentBlockItemKind.Object:
                    {
                        var clone = itemData.DeepClone() as JObject;
                        if (clone == null)
                            return null;

                        foreach (var property in clone.Properties().ToList())
                        {
                            if (itemDescriptor.Properties.All(p => p.Name != property.Name))
                                property.Remove();
                        }

                        foreach (var propertyDescriptor in itemDescriptor.Properties)
                        {
                            var property = (JProperty)clone.Property(propertyDescriptor.Name);
                            if (property == null)
                            {
                                Debug.Fail("Данные блока не соответствуют схеме документа");
                                throw new InvalidOperationException("Данные блока не соответствуют схеме документа");
                            }

                            property.Value = FixContent(propertyDescriptor, property.Value);
                        }

                        return clone;
                    }
                case VersionedDocumentBlockItemKind.Array:
                    var array = itemData as JArray;
                    if (array == null)
                        return null;

                    for (var index = 0; index < array.Count; index++)
                    {
                        var item = array[index];
                        array[index] = FixContent(itemDescriptor.Items, item);
                    }

                    return array;
                default:
                    return itemData;
            }
        }

        private IEnumerable<VersionedDocumentBlockInspectionInfo> ConvertValidationErrorsToInspections(
            IList<ValidationError> schemaErrors, VersionedDocumentDescriptor documentDescriptor,
            JObject actualDocumentData)
        {
            var dic = schemaErrors.ToDictionary(e => e,
                e =>
                {
                    var blockName = _blockNameRegex.Match(e.Path).Groups[1].Value;
                    return documentDescriptor.Blocks.FirstOrDefault(d => d.Name == blockName);
                });

            var inspectations = dic.GroupBy(d => d.Value).Select(g => new VersionedDocumentBlockInspectionInfo(g.Key,
                actualDocumentData[g.Key.Name],
                new[] { $"Возникли ошибки [{g.Count()}] структуры данных при сохранении блока '{g.Key.Name}'" },
                new string[0]));

            return inspectations;
        }
    }

    public class VersionedDocumentBlockInspectionInfo
    {
        public VersionedDocumentBlockInspectionInfo(VersionedDocumentBlockDescriptor blockDescriptor,
            JToken blockDataData, IEnumerable<string> errors, IEnumerable<string> warnings)
        {
            BlockDescriptor = blockDescriptor;
            BlockData = blockDataData;
            Errors = errors.ToArray();
            Warnings = warnings.ToArray();
        }

        [JsonIgnore] public VersionedDocumentBlockDescriptor BlockDescriptor { get; }

        public string BlockName => BlockDescriptor.Name;

        public string BlockDisplayName => BlockDescriptor.Name;

        [JsonIgnore] public JToken BlockData { get; }

        public string[] Errors { get; }

        public string[] Warnings { get; }
    }
}