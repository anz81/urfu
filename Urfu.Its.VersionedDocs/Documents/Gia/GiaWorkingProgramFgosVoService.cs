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

namespace Urfu.Its.VersionedDocs.Documents.Gia
{
    public class GiaWorkingProgramFgosVoService : ModuleWorkingProgramServiceBase<GiaWorkingProgramFgosVoSchemaModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;

        public GiaWorkingProgramFgosVoService(ApplicationDbContext db, IVersionedDocumentSchemaService schemaService,
            IVersionedDocumentModelDescriptorFactory<GiaWorkingProgramFgosVoSchemaModel> descriptorFactory,
            ILifetimeScope scope, IVersionedDocumentDescriptorService descriptorService, IVersionedDocumentService documentService, IPrincipal user) 
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
            return VersionedDocumentType.GiaWorkingProgram;
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
            return new GiaWorkingProgramViewModel(wp, _documentService)
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