using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
//using System.Web.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Autofac;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Services
{
    public abstract class WorkingProgramService<T> : IWorkingProgramService<T>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentSchemaService _schemaService;
        private readonly IVersionedDocumentDescriptorFactory _descriptorFactory;
        private readonly IVersionedDocumentDescriptorService _descriptorService;
        private readonly ILifetimeScope _scope;
        private readonly IPrincipal _user;

        protected WorkingProgramService(ApplicationDbContext db,
            IVersionedDocumentSchemaService schemaService,
            IVersionedDocumentDescriptorFactory descriptorFactory,
            IVersionedDocumentDescriptorService descriptorService,
            ILifetimeScope scope,
            IPrincipal user)
        {
            _db = db;
            _schemaService = schemaService;
            _descriptorFactory = descriptorFactory;
            _descriptorService = descriptorService;
            _scope = scope;
            _user = user;
        }

        private VersionedDocumentDescriptor _descriptor;
        private readonly object _lock = new object();

        public abstract IEnumerable<WorkingProgramSection> GetSections();

        public VersionedDocumentDescriptor GetDocumentDescriptor()
        {
            lock (_lock)
                if (_descriptor == null)
                    _descriptor = _descriptorFactory.CreateDocumentDescriptor();
            return _descriptor;
        }

        public abstract VersionedDocumentTemplate GetDocumentTemplate();

        public abstract VersionedDocument CreateDocument(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null);

        public abstract IReadOnlyDictionary<VersionedDocument, DocumentVersionInfo> GetDocumentsAndVersions(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null);

        public abstract DocumentPartViewModel GetNavigationViewModel(VersionedDocument document);

        public Type GetActualSchemaType()
        {
            return typeof(T);
        }

        public virtual bool IsInEditableState(VersionedDocument document)
        {
            var descriptor = GetDocumentDescriptor();
            var schemaString = descriptor.GenerateSchemaString();
            var isSchemaMatched = _schemaService.IsSchemaMatched(schemaString, document.Template.Schema);
            if (!isSchemaMatched)
                return false;

            //if (!_user.IsInRole(ItsRoles.WorkingProgramManager))
            //    return false;

            return true;
        }

        public virtual bool UserHasAccess(IPrincipal user, VersionedDocument document)
        {
            if (user.IsInRole(ItsRoles.Admin))
                return true;

            if (user.IsInRole(ItsRoles.WorkingProgramView))
                return true;

            return true;
        }

        public abstract VersionedDocument CreateDocumentBasedOn(VersionedDocument document, int? year = null);
            
        protected virtual VersionedDocument CreateWorkingProgramDocumentCore(VersionedDocumentType documentType, IReadOnlyDictionary<string, object> defaultBlockValues, IReadOnlyDictionary<string, VersionedDocumentBlock> sharedBlocks = null)
        {
            if(sharedBlocks == null)
                sharedBlocks = new Dictionary<string, VersionedDocumentBlock>();

            var descriptor = GetDocumentDescriptor();
            
            var lastTemplate = GetDocumentTemplate();

            if (lastTemplate == null)
                throw new InvalidOperationException($"Не найден шаблон документа для типа '{documentType}'.");

            if (!_schemaService.IsSchemaMatched(descriptor.GenerateSchemaString(), lastTemplate.Schema))
                throw new InvalidOperationException($"Схема документа '{documentType}' не соответствует схеме в шаблоне.");

            VersionedDocument document = new VersionedDocument();
            document.Template = lastTemplate;

            var time = DateTime.Now;

            var blockLinks = new List<VersionedDocumentBlockLink>();
            foreach (var blockDescriptor in descriptor.Blocks)
            {
                object dataContent = _descriptorService.BuildDefaultContent(blockDescriptor);

                if (defaultBlockValues != null)
                {
                    if(defaultBlockValues.TryGetValue(blockDescriptor.Name, out var initialValue))
                    {
                        dataContent = initialValue;
                        //Mapper.DynamicMap(initialValue, dataContent, initialValue.GetType(), dataContent.GetType());
                    }
                }

                if (!sharedBlocks.TryGetValue(blockDescriptor.Name, out var block))
                {
                    block = new VersionedDocumentBlock
                    {
                        Name = blockDescriptor.Name,
                        Data = BlockDataHelper.PrepareData(dataContent),
                        CreatedAt = time
                    };                    
                }

                blockLinks.Add(new VersionedDocumentBlockLink
                {
                    Document = document,
                    UpdateTime = time,
                    DocumentBlock = block
                });
            }
            document.BlockLinks = blockLinks;
            return document;
        }

        public abstract void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document);

        protected VersionedDocument CloneDocument(VersionedDocument document, DateTime? time = null)
        {
            var newDoc = new VersionedDocument
            {
                BlockLinks = new List<VersionedDocumentBlockLink>(),
                Template = document.Template               
            };

            if(time == null)
                time = DateTime.Now;
            
            foreach (var link in document.BlockLinks)
            {
                var newLink = new VersionedDocumentBlockLink
                {                    
                    Document = newDoc,
                    DocumentBlock = new VersionedDocumentBlock
                    {
                        Name = link.DocumentBlock.Name,
                        Data = link.DocumentBlock.Data,
                        CreatedAt = time.Value,
                        PreviousBlock = link.DocumentBlock,
                        Version = link.DocumentBlock.Version
                    },
                    UpdateTime = time.Value           
                };                
                newDoc.BlockLinks.Add(newLink);
            }

            return newDoc;
        }
    }    
}