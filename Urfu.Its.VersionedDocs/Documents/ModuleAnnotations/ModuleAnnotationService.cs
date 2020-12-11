using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Autofac;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.ModuleAnnotations
{
    public class ModuleAnnotationService : ModuleWorkingProgramServiceBase<ModuleAnnotationSchemaModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;
        private readonly VersionedDocumentType _documentType = VersionedDocumentType.ModuleAnnotation;
        private readonly IComponentContext _context;
        private readonly IPrincipal _user;

        public ModuleAnnotationService(ApplicationDbContext db, IVersionedDocumentSchemaService schemaService,
            IVersionedDocumentService documentService,
            IVersionedDocumentModelDescriptorFactory<ModuleAnnotationSchemaModel> descriptorFactory,
            ILifetimeScope scope, IVersionedDocumentDescriptorService descriptorService, IPrincipal user) 
            : base(db, schemaService, descriptorFactory, scope, descriptorService, user)
        {
            _db = db;
            _documentService = documentService;
            _user = user;
        }

        public override VersionedDocumentTemplate GetDocumentTemplate()
        {
            return _db.VersionedDocumentTemplates.Where(t => t.DocumentType == _documentType).OrderByDescending(t => t.Version).First();
        }

        public override DocumentPartViewModel GetNavigationViewModel(VersionedDocument document)
        {
            var annotation = _db.ModuleAnnotations.Find(document.Id);

            var editStatuses = _db.UpopStatuses.ToList().Where(s => s.CanEdit());

            return new ViewModels.ModuleAnnotationViewModel(annotation, _documentService)
            {
                AllowEdit = IsInEditableState(document) && (editStatuses.Contains(annotation.Status) || annotation.Status == null)
            };
        }

        public override IEnumerable<WorkingProgramSection> GetSections()
        {
            throw new NotImplementedException();
        }

        public override void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document)
        {
            builder.RegisterInstance(document);
            var annotation = _db.ModuleAnnotations.Find(document.Id);
            var ohopDocument = annotation.BasicCharacteristicOP.VersionedDocument;
            var model = (BasicCharacteristicOPSchemaModel)_documentService.CreateModel(ohopDocument);
            builder.RegisterInstance(annotation);
            builder.RegisterInstance(annotation.BasicCharacteristicOP);
            builder.RegisterInstance(annotation.BasicCharacteristicOP.Info);
            builder.RegisterInstance(model);
        }

        public override IReadOnlyDictionary<VersionedDocument, DocumentVersionInfo> GetDocumentsAndVersions(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null)
        {
            int ohopId;
            bool isParsed = int.TryParse(linkedEntityId, out ohopId);
            if (isParsed)
            {
                var versions = _db.ModuleAnnotations
                    .Where(a => a.BasicCharacteristicOPId == ohopId && a.PlanNumber == planNumber && a.PlanVersionNumber == planVersionNumber)
                    .ToList()
                    .ToDictionary(d => d.VersionedDocument, d => new DocumentVersionInfo(1));
                return versions;
            }
            else
                throw new ArgumentException();
        }

        public override VersionedDocument CreateDocument(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null)
        {
            int ohopId;
            bool isParsed = int.TryParse(linkedEntityId, out ohopId);
            if (!isParsed)
                throw new InvalidOperationException("Не указан ОХОП для аннотации модулей");

            if (!planNumber.HasValue || !planVersionNumber.HasValue)
                throw new InvalidOperationException("Не указан учебный план для аннотации модулей");

            var defaultModel = GetDefaultBlockValues(linkedEntityId);
            var document = CreateWorkingProgramDocumentCore(GetDocumentType(), defaultModel, new Dictionary<string, VersionedDocumentBlock>());

            var ohop = _db.BasicCharacteristicOPs.FirstOrDefault(b => b.VersionedDocumentId == ohopId);

            var userDirections = _db.DirectionsForUser(_user).Select(d => d.uid).ToList();

            if (!UserHasAccess(_user, document) && !userDirections.Contains(ohop?.Info?.Profile?.DIRECTION_ID))
                throw new InvalidOperationException("Нет прав на создание документа.");
            
            ModuleAnnotation annotation = new ModuleAnnotation
            {
                BasicCharacteristicOPId = ohopId,
                PlanNumber = planNumber.Value,
                PlanVersionNumber = planVersionNumber.Value,
                VersionedDocumentId = document.Id,
                VersionedDocument = document,
                UpopStatusId = UPOPStatus.GetDefaultStatus(_db).Id,
                StatusChangeTime = DateTime.Now
            };
            _db.ModuleAnnotations.Add(annotation);

            return document;
        }
        
        public override VersionedDocument CreateDocumentBasedOn(VersionedDocument document, int? year = null)
        {
            throw new Exception("Аннотации модулей не могут создаваться на основе другого документа");
        }

        protected override IReadOnlyDictionary<string, object> GetDefaultBlockValues(string linkedEntityId)
        {
            int ohopId;
            bool isParsed = int.TryParse(linkedEntityId, out ohopId);
            if (!isParsed)
                throw new InvalidOperationException("Не указан ОХОП для аннотации модулей");

            var ohop = _db.BasicCharacteristicOPs.FirstOrDefault(b => b.VersionedDocumentId == ohopId);
            if (ohop == null)
                throw new InvalidOperationException("Не найден ОХОП для аннотации модулей");

            var defaultValues = new BasicCharacteristicOPDefaultValues(ohop.Info);

            var values = new Dictionary<string, object>();

            values.Add(nameof(ModuleAnnotationSchemaModel.Institute), defaultValues.Institute());
            values.Add(nameof(ModuleAnnotationSchemaModel.Direction), defaultValues.Direction());
            values.Add(nameof(ModuleAnnotationSchemaModel.Profile), defaultValues.Profile());

            return values;
        }

        protected override VersionedDocumentType GetDocumentType()
        {
            return VersionedDocumentType.ModuleAnnotation;
        }

        protected override string GetStandard()
        {
            throw new NotImplementedException();
        }

    }
}