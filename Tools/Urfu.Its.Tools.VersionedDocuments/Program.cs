using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using CommandLine;
using CommandLine.Text;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Documents.CompetencePassports;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.Gia;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Documents.ModuleAnnotations;
using Urfu.Its.VersionedDocs.Documents.ModuleChangeList;
using Urfu.Its.VersionedDocs.Documents.Practices;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;

namespace Urfu.Its.Tools.VersionedDocuments
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    switch (options.Command?.ToLower())
                    {
                        case "updatetemplate":
                            UpdateTemplate(options);
                            break;
                        case "deletedata":
                            DeleteData(options);
                            break;
                        default:
                            throw new InvalidOperationException($"Команда '{options.Command}' не распознана");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        private static void DeleteData(Options options)
        {
            if (options.DocumentType != null)
            {
                var documentType = ParseDocumentType(options);
                if (!options.Force)
                    if (MessageBox.Show($"Будут удалены все шаблоны и данные документов типа '{documentType.ToString()}'. Продолжить?", "Подтверждение", MessageBoxButtons.YesNo) !=
                        DialogResult.Yes)
                        return;

                using (var db = new ApplicationDbContext())
                {
                    using (var tran = db.Database.BeginTransaction())
                    {
                        var blocksToRemove = new List<VersionedDocumentBlock>();
                        foreach (var link in db.VersionedDocumentBlockLinks.ToList())
                        {
                            var b = link.DocumentBlock;
                            do
                            {
                                if (b.Links.All(l => l.Document.Template.DocumentType == documentType))
                                {
                                    var buf = b;
                                    b = b.PreviousBlock;
                                    blocksToRemove.Add(buf);
                                }
                                else
                                    b = null;
                            } while (b != null);
                        }

                        var blockIdsToRemove = blocksToRemove.Select(b => b.Id).ToList();
                        foreach (var block in blocksToRemove)
                        {
                            var blockId = block.Id;
                            if (!db.VersionedDocumentBlocks.Any(b =>
                                b.PreviousBlockId == blockId && !blockIdsToRemove.Contains(b.Id)))
                                db.VersionedDocumentBlocks.Remove(block);
                        }

                        var toRemove = db.VersionedDocumentTemplates.Where(t => t.DocumentType == documentType).ToList();
                        foreach (var template in toRemove)
                        {
                            foreach (var document in template.Documents.ToList())
                            {
                                var mwp = db.ModuleWorkingPrograms.Find(document.Id);                                
                                if(mwp != null)
                                    db.ModuleWorkingPrograms.Remove(mwp);
                                var dwp = db.DisciplineWorkingPrograms.Find(document.Id);
                                if(dwp != null)
                                    db.DisciplineWorkingPrograms.Remove(dwp);
                                db.VersionedDocuments.Remove(document);
                            }
                            db.VersionedDocumentTemplates.Remove(template);
                        }

                        db.SaveChanges();
                        tran.Commit();
                    }
                }
            }
        }

        private static void UpdateTemplate(Options options)
        {
            var data = GetTemplateData(options);
            if (data == null)
                return;

            using (var db = new ApplicationDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    var updateTime = DateTime.Now;
                    var documentType = ParseDocumentType(options);

                    var logFileName = $"{updateTime:yyyyMMddHHmmss.fff}.log";
                    if (!Directory.Exists("Logs"))
                        Directory.CreateDirectory("Logs");

                    using (var logger = new CombinedLogger<VersionedDocumentUpdater>(new ConsoleLogger<VersionedDocumentUpdater>(), new VersionedDocumentsFileLogger<VersionedDocumentUpdater>(Path.Combine("Logs/", logFileName))))
                    {
                        var template = GetActualTemplate(db, options);
                        var templateCreated = false;
                        if(template == null)
                        {
                            template = new VersionedDocumentTemplate
                            {
                                CreatedAt = updateTime,
                                DocumentType = documentType,
                                Schema = GetActualSchema(options),
                                Documents = new List<VersionedDocument>(),
                                Data = data,
                                Version = 1
                            };
                            db.VersionedDocumentTemplates.Add(template);
                            templateCreated = true;
                        }

                        db.SaveChanges();

                        if(templateCreated)
                            logger.Info($"Template for type [{documentType}] is not exists. New template [{template.Id}] have created.");

                        var mode = options.Force ? UpdateMode.UpdateAllThatPossible : UpdateMode.TryUpdate;

                        var actualDescriptor = GetDescriptorFactory(documentType).CreateDocumentDescriptor();

                        IReadOnlyDictionary<VersionedDocument, string[]> documentErrors = new Dictionary<VersionedDocument, string[]>();
                        var oldTemplateDocuments = new List<VersionedDocument>();
                        Exception unsupportedChangesException = null;
                        
                        try
                        {
                            if (template.Documents.Any())
                            {
                                var builder = new ContainerBuilder();
                                var container = builder.Build();
                                var schemaService = new VersionedDocumentSchemaService();
                                var updater = new VersionedDocumentUpdater(new VersionedDocumentDescriptorService(container), logger, new FromDatabaseVersionedDocumentService(schemaService), schemaService);

                                updater.UpdateLinkedDocumentsForTemplate(template, actualDescriptor, out documentErrors, updateTime);                                
                                oldTemplateDocuments = documentErrors.Keys.ToList();                                
                            }
                            else
                            {
                                logger.Info("No documents to update.");
                            }
                        }
                        catch (VersionedDocumentUpdaterNotSupportedTheseChangesException ex)
                        {
                            unsupportedChangesException = ex;
                            oldTemplateDocuments = template.Documents.ToList();
                        }

                        var actualTemplateDocuments = template.Documents.Except(oldTemplateDocuments).ToList();
                        var newSchema = actualDescriptor.GenerateSchemaString();

                        if (mode == UpdateMode.TryUpdate)
                        {
                            if (unsupportedChangesException != null)
                            {
                                var message = $"While trying to update documents data template unsupported schema changes have detected. Deteils:{Environment.NewLine}{unsupportedChangesException.Message}";
                                logger.Error(message);
                                throw new Exception(message);
                            }

                            if (documentErrors.Any())
                            {
                                var errorsText = string.Join($"{Environment.NewLine}---{Environment.NewLine}",
                                    documentErrors.Select(d => $"Document {d.Key.Id}:{string.Join(Environment.NewLine, d.Value)}"));
                                var message = $"Some errors have occurred in [{documentErrors.Count}/{actualTemplateDocuments.Count}] documents while trying to update data. Details:{Environment.NewLine}{errorsText}";
                                logger.Error(message);
                                throw new Exception(message);
                            }

                            logger.Info($"Number of updated documents [{actualTemplateDocuments.Count}].");

                            template.Schema = newSchema;
                            template.Data = data;
                            template.CreatedAt = updateTime;
                            db.SaveChanges();
                            logger.Info($"Data and schema of template [{template.Id}] have updated without changing the version.");
                        }
                        else if (mode == UpdateMode.UpdateAllThatPossible)
                        {
                            if (unsupportedChangesException != null)
                            {
                                var message = $"While trying to update template unsupported schema changes have detected. Deteils:{Environment.NewLine}{unsupportedChangesException.Message}";
                                logger.Warning(message);

                                if (!templateCreated)
                                {
                                    var newVersionTemplate = new VersionedDocumentTemplate
                                    {
                                        CreatedAt = updateTime,
                                        DocumentType = documentType,
                                        Schema = newSchema,
                                        Version = template.Version + 1,
                                        Data = data,
                                        PreviousTemplate = template
                                    };

                                    db.VersionedDocumentTemplates.Add(newVersionTemplate);
                                    db.SaveChanges();

                                    logger.Info($"New template [{newVersionTemplate.Id}] have created.");
                                    logger.Info($"Documents [{oldTemplateDocuments.Count}] have remained on the previous version [{template.Version}] of the template [{template.Id}].");
                                }                                
                            }
                            else if (documentErrors.Any())
                            {
                                var errorsText = string.Join($"{Environment.NewLine}---{Environment.NewLine}",
                                    documentErrors.Select(d => $"Document {d.Key.Id}:{string.Join(Environment.NewLine, d.Value)}"));
                                var message = $"Some errors have been occurred in [{documentErrors.Count}/{actualTemplateDocuments.Count}] documents while trying to update data. Details:{Environment.NewLine}{errorsText}";
                                logger.Warning(message);

                                var newVersionTemplate = new VersionedDocumentTemplate
                                {
                                    CreatedAt = updateTime,
                                    DocumentType = documentType,
                                    Schema = newSchema,
                                    Documents = actualTemplateDocuments,
                                    Version = template.Version + 1,
                                    Data = data,
                                    PreviousTemplate = template
                                };

                                db.VersionedDocumentTemplates.Add(newVersionTemplate);
                                db.SaveChanges();

                                logger.Info($"New template [{newVersionTemplate.Id}] have created.");

                                logger.Info($"Updated documents [{actualTemplateDocuments.Count}] have moved to template [{newVersionTemplate.Id}] of new version [{newVersionTemplate.Version}].");
                            }
                            else
                            {
                                template.Schema = newSchema;
                                template.Data = data;
                                template.CreatedAt = updateTime;
                                db.SaveChanges();
                                logger.Info($"Data and schema of template [{template.Id}] have updated without changing the version.");
                            }
                        }                        
                        
                        logger.Info("Committing changes...");
                        tran.Commit();

                        logger.Info($"Update command in mode [{mode}] successfully completed.");
                    }                    
                }
            }
        }

        private static VersionedDocumentTemplate GetActualTemplate(ApplicationDbContext db, Options options)
        {
            var documentType = ParseDocumentType(options);
            var dbVersionedDocumentTemplates = db.VersionedDocumentTemplates.ToList();
            var templateId = dbVersionedDocumentTemplates.Where(t => t.DocumentType == documentType).OrderByDescending(t => t.Version).FirstOrDefault();
            return templateId;
        }

        private static byte[] GetTemplateData(Options options)
        {
            byte[] data;
            if (options.TemplatePath == null)
            {
                var dialog = new OpenFileDialog
                {
                    DefaultExt = "docx",
                    Filter = "Word 2007 Documents (*.docx)|*.docx"
                };

                if (dialog.ShowDialog() != DialogResult.OK)
                    return null;

                using (var file = dialog.OpenFile())
                {
                    data = new byte[file.Length];
                    file.Read(data, 0, data.Length);
                }
            }
            else
            {
                using (var stream = File.Open(options.TemplatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }

                //data = File.ReadAllBytes(options.TemplatePath);
            }
            return data;
        }

        private static string GetActualSchema(Options options)
        {
            var documentType = ParseDocumentType(options);
            var descriptorFactory = GetDescriptorFactory(documentType);

            var descriptor = descriptorFactory.CreateDocumentDescriptor();
            var schema = descriptor.GenerateSchema();
            return schema.ToString();
        }

        private static IVersionedDocumentDescriptorFactory GetDescriptorFactory(VersionedDocumentType documentType)
        {
            IVersionedDocumentDescriptorFactory descriptorFactory;
            switch (documentType)
            {
                case VersionedDocumentType.ModuleWorkingProgram:
                    descriptorFactory = new AllPropertiesAreBlocksDescriptorFactory<ModuleWorkingProgramFgosVoSchemaModel>();
                    break;
                case VersionedDocumentType.DisciplineWorkingProgram:
                    descriptorFactory =
                        new AllPropertiesAreBlocksDescriptorFactory<DisciplineWorkingProgramFgosVoSchemaModel>();
                    break;
                case VersionedDocumentType.GiaWorkingProgram:
                    descriptorFactory = new AllPropertiesAreBlocksDescriptorFactory<GiaWorkingProgramFgosVoSchemaModel>();
                    break;
                case VersionedDocumentType.PracticesWorkingProgram:
                    descriptorFactory = new AllPropertiesAreBlocksDescriptorFactory<PracticesWorkingProgramFgosVoSchemaModel>();
                    break;
                case VersionedDocumentType.ModuleChangeList:
                    descriptorFactory = new AllPropertiesAreBlocksDescriptorFactory<ModuleChangeListSchemaModel>();
                    break;
                case VersionedDocumentType.BasicCharacteristicOP:
                    descriptorFactory = new AllPropertiesAreBlocksDescriptorFactory<BasicCharacteristicOPSchemaModel>();
                    break;
                case VersionedDocumentType.CompetencePassport:
                    descriptorFactory = new AllPropertiesAreBlocksDescriptorFactory<CompetencePassportSchemaModel>();
                    break;
                case VersionedDocumentType.ModuleAnnotation:
                    descriptorFactory = new AllPropertiesAreBlocksDescriptorFactory<ModuleAnnotationSchemaModel>();
                    break;
                default:
                    throw new NotImplementedException(
                        $"Descriptor factory is not implemented for type '{documentType}'");
            }

            return descriptorFactory;
        }

        private static VersionedDocumentType ParseDocumentType(Options options)
        {
            if (int.TryParse(options.DocumentType, out var documentTypeId))
                return (VersionedDocumentType) documentTypeId;

            switch (options.DocumentType?.ToLower())
            {
                case "mwp":
                case "moduleworkingprogram":
                    return VersionedDocumentType.ModuleWorkingProgram;
                case "dwp":
                case "disciplineworkingprogram":
                    return VersionedDocumentType.DisciplineWorkingProgram;
                case "gwp":
                case "giaworkingprogram":
                    return VersionedDocumentType.GiaWorkingProgram;
                case "pwp":
                case "practicesworkingprogram":
                    return VersionedDocumentType.PracticesWorkingProgram;
                case "mcl":
                case "modulechangelist":
                    return VersionedDocumentType.ModuleChangeList;
                case "ohop":
                case "basiccharacteristicop":
                    return VersionedDocumentType.BasicCharacteristicOP;
                case "cp":
                case "competencepassport":
                    return VersionedDocumentType.CompetencePassport;
                case "ma":
                case "moduleannotation":
                    return VersionedDocumentType.ModuleAnnotation;
            }
            throw new InvalidOperationException($"Не распознан тип документа: {options.DocumentType}");
        }
    }

    public class Options
    {
        [Option('с', "command", HelpText = "Команда к выполнению. Доступные команды: createTemplate, deleteData.")]
        public string Command { get; set; }

        [Option('d', "documentType", HelpText = "Тип документа: mwp, dwp, gwp, pwp, mcl, dcl")]
        public string DocumentType { get; set; }

        [Option('t', "templatePath", HelpText = "Путь к файлу вордовского шаблонного документа.")]
        public string TemplatePath { get; set; }

        [Option('f', "force", DefaultValue = false)]
        public bool Force { get; set; }

       [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}