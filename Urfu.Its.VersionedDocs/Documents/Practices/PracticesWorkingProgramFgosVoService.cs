using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Autofac;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;

namespace Urfu.Its.VersionedDocs.Documents.Practices
{
    public class PracticesWorkingProgramFgosVoService : ModuleWorkingProgramServiceBase<PracticesWorkingProgramFgosVoSchemaModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;

        public PracticesWorkingProgramFgosVoService(ApplicationDbContext db,
            IVersionedDocumentSchemaService schemaService, IVersionedDocumentService documentService,
            IVersionedDocumentModelDescriptorFactory<PracticesWorkingProgramFgosVoSchemaModel> descriptorFactory,
            ILifetimeScope scope, IVersionedDocumentDescriptorService descriptorService, IPrincipal user) 
            : base(db, schemaService, descriptorFactory, scope, descriptorService, user)
        {
            _db = db;
            _documentService = documentService;
        }

        protected override string GetStandard()
        {
            return StandardNames.FgosVo;
        }

        protected override IReadOnlyDictionary<string, object> GetDefaultBlockValues(string linkedEntityId)
        {
            return null;
        }

        protected override VersionedDocumentType GetDocumentType()
        {
            return VersionedDocumentType.PracticesWorkingProgram;
        }

        public override IEnumerable<WorkingProgramSection> GetSections()
        {
            throw new NotImplementedException();
        }

        public override VersionedDocumentTemplate GetDocumentTemplate()
        {
            var documentType = GetDocumentType();
            return _db.VersionedDocumentTemplates.Where(t => t.DocumentType == documentType).OrderByDescending(t => t.Version).First();
        }

        public override DocumentPartViewModel GetNavigationViewModel(VersionedDocument document)
        {
            var wp = _db.ModuleWorkingPrograms.Find(document.Id);
            return new PracticesWorkingProgramViewModel(wp, _documentService)
            {
                AllowEdit = IsInEditableState(document)
            };
        }

        public override void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document)
        {
            builder.RegisterInstance(document);
            var wp = _db.ModuleWorkingPrograms.Find(document.Id);
            builder.RegisterInstance(wp);
            builder.RegisterInstance(wp.Module);            
        }
    }
}