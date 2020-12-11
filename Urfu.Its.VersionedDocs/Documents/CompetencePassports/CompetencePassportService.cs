using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Principal;
using Autofac;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.CompetencePassports
{
    public class CompetencePassportService : ModuleWorkingProgramServiceBase<CompetencePassportSchemaModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;
        private readonly VersionedDocumentType _documentType = VersionedDocumentType.CompetencePassport;
        private readonly IComponentContext _context;
        private readonly IPrincipal _user;

        public CompetencePassportService(ApplicationDbContext db, IVersionedDocumentSchemaService schemaService,
            IVersionedDocumentService documentService,
            IVersionedDocumentModelDescriptorFactory<CompetencePassportSchemaModel> descriptorFactory,
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
            var passport = _db.CompetencePassports.Find(document.Id);

            var editStatuses = _db.UpopStatuses.ToList().Where(s => s.CanEdit());

            return new ViewModels.CompetencePassportViewModel(passport, _documentService)
            {
                AllowEdit = IsInEditableState(document) && (editStatuses.Contains(passport.Status) || passport.Status == null)
            };
        }

        public override IEnumerable<WorkingProgramSection> GetSections()
        {
            throw new NotImplementedException();
        }

        public override void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document)
        {
            builder.RegisterInstance(document);
            var passport = _db.CompetencePassports.Find(document.Id);
            var ohopDocument = passport.BasicCharacteristicOP.VersionedDocument;
            var model = (BasicCharacteristicOPSchemaModel)_documentService.CreateModel(ohopDocument);
            builder.RegisterInstance(passport);
            builder.RegisterInstance(passport.BasicCharacteristicOP);
            builder.RegisterInstance(passport.BasicCharacteristicOP.Info);
            builder.RegisterInstance(model);
        }

        public override IReadOnlyDictionary<VersionedDocument, DocumentVersionInfo> GetDocumentsAndVersions(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null)
        {
            int ohopId;
            bool isParsed = int.TryParse(linkedEntityId, out ohopId);
            if (isParsed)
            {
                var versions = _db.CompetencePassports
                    .Where(p => p.BasicCharacteristicOPId == ohopId && p.Year == year.Value)
                    .OrderByDescending(d => d.Version)
                    .ToList()
                    .ToDictionary(d => d.VersionedDocument, d => new DocumentVersionInfo(d.Version));
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
                throw new InvalidOperationException("Не указан ОХОП для паспорта компетенций");

            if (!year.HasValue)
                throw new InvalidOperationException("Не указан год паспорта компетенций");

            var defaultModel = GetDefaultBlockValues(linkedEntityId);
            var document = CreateWorkingProgramDocumentCore(GetDocumentType(), defaultModel, new Dictionary<string, VersionedDocumentBlock>());

            var ohop = _db.BasicCharacteristicOPs.FirstOrDefault(b => b.VersionedDocumentId == ohopId);

            var userDirections = _db.DirectionsForUser(_user).Select(d => d.uid).ToList();

            if (!UserHasAccess(_user, document) && !userDirections.Contains(ohop?.Info?.Profile?.DIRECTION_ID))
                throw new InvalidOperationException("Нет прав на создание документа.");

            var versions = GetDocumentsAndVersions(linkedEntityId, standard, year);

            CompetencePassport cp = new CompetencePassport
            {
                BasicCharacteristicOPId = ohopId,
                Year = year.Value,
                Version = 1 + (versions.Any() ? versions.Max(v => v.Value.VersionNumber) : 0),
                VersionedDocumentId = document.Id,
                VersionedDocument = document,
                UpopStatusId = UPOPStatus.GetDefaultStatus(_db).Id,
                StatusChangeTime = DateTime.Now
            };
            _db.CompetencePassports.Add(cp);

            return document;
        }
        
        public override VersionedDocument CreateDocumentBasedOn(VersionedDocument document, int? year = null)
        {
            var cp = _db.CompetencePassports.Find(document.Id);

            var versions = GetDocumentsAndVersions(cp.BasicCharacteristicOPId.ToString(), standard: null, year: cp.Year);
            var newVersion = 1 + (versions.Any() ? versions.Max(v => v.Value.VersionNumber) : 0);

            var time = DateTime.Now;

            var newDocument = CloneDocument(document, time);
            _db.VersionedDocuments.Add(newDocument);

            var newCp = new CompetencePassport
            {
                BasicCharacteristicOPId = cp.BasicCharacteristicOPId,
                StatusChangeTime = time,
                UpopStatusId = UPOPStatus.GetDefaultStatus(_db).Id,
                Version = newVersion,
                VersionedDocument = newDocument,
                BasedOnId = cp.VersionedDocumentId,
                Year = cp.Year,
            };

            _db.CompetencePassports.Add(newCp);

            if (!UserHasAccess(_user, newDocument))
                throw new InvalidOperationException("Нет прав на создание документа.");

            _db.SaveChanges();

            return newDocument;
        }

        protected override IReadOnlyDictionary<string, object> GetDefaultBlockValues(string linkedEntityId)
        {
            int ohopId;
            bool isParsed = int.TryParse(linkedEntityId, out ohopId);
            if (!isParsed)
                throw new InvalidOperationException("Не указан ОХОП для паспорта компетенций");

            var ohop = _db.BasicCharacteristicOPs.FirstOrDefault(b => b.VersionedDocumentId == ohopId);
            if (ohop == null)
                throw new InvalidOperationException("Не найден ОХОП для паспорта компетенций");


            var defaultValues = new BasicCharacteristicOPDefaultValues(ohop.Info);

            var values = new Dictionary<string, object>();

            values.Add(nameof(CompetencePassportSchemaModel.EduProgramInfo), defaultValues.EduProgramInfo());
            values.Add(nameof(CompetencePassportSchemaModel.Institute), defaultValues.Institute());
            values.Add(nameof(CompetencePassportSchemaModel.Direction), defaultValues.Direction());
            values.Add(nameof(CompetencePassportSchemaModel.Profile), defaultValues.Profile());

            return values;
        }

        protected override VersionedDocumentType GetDocumentType()
        {
            return VersionedDocumentType.CompetencePassport;
        }

        protected override string GetStandard()
        {
            throw new NotImplementedException();
        }

    }
}