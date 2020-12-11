using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Autofac;
using Autofac.Features.Indexed;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;

namespace Urfu.Its.VersionedDocs.Documents.ModuleChangeList
{
    public class ModuleChangeListService : IChangeListService<ModuleChangeListSchemaModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentModelDescriptorFactory<ModuleChangeListSchemaModel> _descriptorFactory;
        private readonly IIndex<VersionedDocumentType, IVersionedDocumentImplementationService> _documentImplementationServices;
        private readonly IWorkingProgramService<DisciplineWorkingProgramFgosVoSchemaModel> _disciplineService;
        private readonly IWorkingProgramService<ModuleWorkingProgramFgosVoSchemaModel> _moduleService;
        private VersionedDocumentDescriptor _descriptor;
        private readonly object _lock = new object();

        public ModuleChangeListService(ApplicationDbContext db, 
            IVersionedDocumentModelDescriptorFactory<ModuleChangeListSchemaModel> descriptorFactory,
            IIndex<VersionedDocumentType, IVersionedDocumentImplementationService> documentImplementationServices,
            IWorkingProgramService<DisciplineWorkingProgramFgosVoSchemaModel> disciplineService,
            IWorkingProgramService<ModuleWorkingProgramFgosVoSchemaModel> moduleService) 
        {
            _db = db;
            _descriptorFactory = descriptorFactory;
            _documentImplementationServices = documentImplementationServices;
            _disciplineService = disciplineService;
            _moduleService = moduleService;
        }

        public VersionedDocumentDescriptor GetDocumentDescriptor()
        {
            lock (_lock)
                if (_descriptor == null)
                    _descriptor = _descriptorFactory.CreateDocumentDescriptor();
            return _descriptor;
        }

        public VersionedDocumentTemplate GetDocumentTemplate()
        {
            var documentType = VersionedDocumentType.ModuleChangeList;
            return _db.VersionedDocumentTemplates.Where(t => t.DocumentType == documentType).OrderByDescending(t => t.Version).FirstOrDefault();
        }

        public VersionedDocument CreateDocument(int sourceId, int targetId)
        {
            var source = _db.ModuleWorkingPrograms.FirstOrDefault(c => c.VersionedDocumentId == sourceId);
            var target = _db.ModuleWorkingPrograms.FirstOrDefault(c => c.VersionedDocumentId == targetId);

            var changeListModel = CreateChangeListModel(source, target);
            var jModel = JObject.FromObject(changeListModel);

            var updateTime = DateTime.Now;

            var doc = new VersionedDocument
            {
                Template = GetDocumentTemplate(),
                BlockLinks = new List<VersionedDocumentBlockLink>()
            };
            
            var descriptor = GetDocumentDescriptor();
            foreach (var blockDescriptor in descriptor.Blocks)
            {
                var content = jModel[blockDescriptor.Name];
                var blockData = BlockDataHelper.PrepareData(content);

                var block = new VersionedDocumentBlock
                {
                    Name = blockDescriptor.Name,
                    CreatedAt = updateTime,
                    Version = 1,
                    Data = blockData
                };

                var link = new VersionedDocumentBlockLink
                {
                    UpdateTime = updateTime,
                    DocumentBlock = block
                };

                doc.BlockLinks.Add(link);
            }
            _db.VersionedDocuments.Add(doc);

            var changeList = new ModuleWorkingProgramChangeList
            {
                Source = source,
                Target = target,
                VersionedDocument = doc
            };
            _db.ModuleWorkingProgramChangeLists.Add(changeList);            

            return doc;
        }

        protected ModuleChangeListSchemaModel CreateChangeListModel(ModuleWorkingProgram source, ModuleWorkingProgram target)
        {
            if (source.VersionedDocument.TemplateId != target.VersionedDocument.TemplateId)
                throw new InvalidOperationException("Документы основаны на разных шаблонах. Формирование листа изменений не возможно.");

            var actualTemplate = _moduleService.GetDocumentTemplate();
            if(actualTemplate.Id != source.VersionedDocument.TemplateId)
                throw new InvalidOperationException("Шаблоны документов устарели. Формирование листа изменений не возможно.");

            var dwpActualTemplate = _disciplineService.GetDocumentTemplate();
            var dwpActualTemplateId = dwpActualTemplate.Id;
            if (source.DisciplineWorkingPrograms
                .Concat(target.DisciplineWorkingPrograms)
                .Any(w => w.VersionedDocument.TemplateId != dwpActualTemplateId))
            {
                throw new InvalidOperationException("Шаблоны документов РПД устарели. Формирование листа изменений не возможно.");
            }

            if (source.ModuleId != target.ModuleId)
                throw new InvalidOperationException("Документ связаны с разными модулями. Формирование листа изменений не возможно.");

            var model = new ModuleChangeListSchemaModel
            {
                FileName = $"Лист изменений модуля «{target.Module.title}»",                
                Name = target.Module.title,
                Disciplines = target.DisciplineWorkingPrograms
                    .Concat(source.DisciplineWorkingPrograms)
                    .GroupBy(p=>p.DisciplineId)
                    .Select(g=>g.First())
                    .Select(d=>new DisciplineChangesInfo
                {
                    Name = d.Discipline.title,
                    DisciplineId = d.DisciplineId
                }).Distinct().ToList()
            };

            var modelType = typeof(ModuleChangeListSchemaModel);
            var disciplineChangesModelType = typeof(DisciplineChangesInfo);

            {
                var sourceBlocks = source.VersionedDocument.BlockLinks.Select(b => b.DocumentBlock).ToList();
                var targetBlocks = target.VersionedDocument.BlockLinks.Select(b => b.DocumentBlock).ToList();
                foreach (var targetBlock in targetBlocks)
                {
                    var sourceBlock = sourceBlocks.First(b => b.Name == targetBlock.Name);
                    if (targetBlock.Version != sourceBlock.Version)
                    {
                        var prop = modelType.GetProperty(targetBlock.Name);
                        if (prop != null && prop.PropertyType == typeof(bool))
                        {
                            prop.SetValue(model, true);
                        }
                    }
                }
            }

            foreach (var disciplineChanges in model.Disciplines)
            {
                var disciplineId = disciplineChanges.DisciplineId;
                var sourceDiscipline = source.DisciplineWorkingPrograms.FirstOrDefault(d => d.DisciplineId == disciplineId);
                var targetDiscipline = target.DisciplineWorkingPrograms.FirstOrDefault(d => d.DisciplineId == disciplineId);
                if (sourceDiscipline == null || targetDiscipline == null)
                {
                    foreach (var propertyInfo in disciplineChangesModelType.GetProperties().Where(p=>p.PropertyType == typeof(bool)))
                    {
                        propertyInfo.SetValue(disciplineChanges, true);
                    }
                    
                    break;
                }                
                
                var sourceBlocks = sourceDiscipline.VersionedDocument.BlockLinks.Select(b => b.DocumentBlock).ToList();
                var targetBlocks = targetDiscipline.VersionedDocument.BlockLinks.Select(b => b.DocumentBlock).ToList();
                foreach (var targetBlock in targetBlocks)
                {
                    var sourceBlock = sourceBlocks.First(b => b.Name == targetBlock.Name);
                    if (targetBlock.Version != sourceBlock.Version)
                    {
                        var prop = disciplineChangesModelType.GetProperty(targetBlock.Name);
                        if (prop != null && prop.PropertyType == typeof(bool))
                        {
                            prop.SetValue(disciplineChanges, true);
                        }
                    }
                }                
            }            

            return model;
        }

        public DocumentPartViewModel GetNavigationViewModel(VersionedDocument document)
        {
            throw new NotSupportedException();
        }

        public Type GetActualSchemaType()
        {
            return typeof(ModuleChangeListSchemaModel);
        }

        public bool IsInEditableState(VersionedDocument document)
        {
            return false;
        }

        public bool UserHasAccess(IPrincipal user, VersionedDocument document)
        {
            return true;
        }

        public void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document)
        {
            builder.RegisterInstance(document);            
            var wp = _db.ModuleWorkingProgramChangeLists.Find(document.Id);
            builder.RegisterInstance(wp);            
        }        
    }
}