using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Web;
using Autofac;
using Newtonsoft.Json;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs
{
    public class BasicCharacteristicOPService : ModuleWorkingProgramServiceBase<BasicCharacteristicOPSchemaModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;
        private readonly VersionedDocumentType _documentType = VersionedDocumentType.BasicCharacteristicOP;
        private readonly IComponentContext _context;
        private readonly IPrincipal _user;

        public BasicCharacteristicOPService(ApplicationDbContext db, IVersionedDocumentSchemaService schemaService,
            IVersionedDocumentService documentService,
            IVersionedDocumentModelDescriptorFactory<BasicCharacteristicOPSchemaModel> descriptorFactory,
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
            var ohop = _db.BasicCharacteristicOPs.Find(document.Id);

            var editStatuses = _db.UpopStatuses.ToList().Where(s => s.CanEdit());

            return new BasicCharacteristicViewModel(ohop, _documentService)
            {
                AllowEdit = IsInEditableState(document) && (editStatuses.Contains(ohop.Status) || ohop.Status == null)
                    || (_user.IsInRole(ItsRoles.Admin) && _user.IsInRole(ItsRoles.WorkingProgramManager))
            };
        }

        public override IEnumerable<WorkingProgramSection> GetSections()
        {
            throw new NotImplementedException();
        }

        public override void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document)
        {
            builder.RegisterInstance(document);
            var ohop = _db.BasicCharacteristicOPs.Find(document.Id);
            builder.RegisterInstance(ohop);
            builder.RegisterInstance(ohop.Info);
            builder.RegisterInstance(new BasicCharacteristicOPDefaultValues(ohop.Info));
        }

        public override IReadOnlyDictionary<VersionedDocument, DocumentVersionInfo> GetDocumentsAndVersions(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null)
        {
            int infoId;
            bool isParsed = int.TryParse(linkedEntityId, out infoId);
            if (isParsed)
            {
                var versions = _db.BasicCharacteristicOPs
                    .Include(d => d.ModuleWorkingPrograms)
                    .Include(d => d.BasedOn)
                    .Where(d => d.InfoId == infoId)
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
            int infoId;
            bool isParsed = int.TryParse(linkedEntityId, out infoId);
            if (!isParsed)
                throw new InvalidOperationException("Не указана образовательная программа для ОХОП");

            var defaultModel = GetDefaultBlockValues(linkedEntityId);
            var document = CreateWorkingProgramDocumentCore(GetDocumentType(), defaultModel, new Dictionary<string, VersionedDocumentBlock>());

            var info = _db.BasicCharacteristicOPInfos.FirstOrDefault(b => b.Id == infoId);

            var userDirections = _db.DirectionsForUser(_user).Select(d => d.uid).ToList();

            if (!UserHasAccess(_user, document) && !userDirections.Contains(info?.Profile?.DIRECTION_ID))
                throw new InvalidOperationException("Нет прав на создание документа.");

            var versions = GetDocumentsAndVersions(linkedEntityId, standard);

            BasicCharacteristicOP basicCharacteristicOP = new BasicCharacteristicOP
            {
                InfoId = infoId,
                Version = 1 + (versions.Any() ? versions.Max(v => v.Value.VersionNumber) : 0),
                VersionedDocumentId = document.Id,
                VersionedDocument = document,
                UpopStatusId = UPOPStatus.GetDefaultStatus(_db).Id,
                StatusChangeTime = DateTime.Now
            };
            _db.BasicCharacteristicOPs.Add(basicCharacteristicOP);

            return document;
        }
        
        public override VersionedDocument CreateDocumentBasedOn(VersionedDocument document, int? year = null)
        {
            var wp = _db.BasicCharacteristicOPs.Find(document.Id);

            year = year.HasValue ? year.Value : wp.Info.Year;

            var info = _db.BasicCharacteristicOPInfos.FirstOrDefault(i => i.ProfileId == wp.Info.ProfileId && i.Year == year);
            if (info == null)
            {
                info = new BasicCharacteristicOPInfo()
                {
                    ProfileId = wp.Info.ProfileId,
                    Profile = wp.Info.Profile,
                    Year = year.Value
                };
                _db.BasicCharacteristicOPInfos.Add(info);
                _db.SaveChanges();
            }

            var versions = GetDocumentsAndVersions(info.Id);
            var newVersion = 1 + (versions.Any() ? versions.Max(v => v.Value.VersionNumber) : 0);

            var time = DateTime.Now;

            var newDocument = CloneDocument(document, time);
            if (wp.Info.Year != year)
            {
                var defaultValues = new BasicCharacteristicOPDefaultValues(info);
                var block = newDocument.BlockLinks.FirstOrDefault(l => l.DocumentBlock.Name == nameof(BasicCharacteristicOPSchemaModel.EduProgramInfo))?.DocumentBlock;
                block.Data = BlockDataHelper.PrepareData(defaultValues.EduProgramInfo());
            }

            _db.VersionedDocuments.Add(newDocument);

            var newWp = new BasicCharacteristicOP
            {
                Info = info,
                StatusChangeTime = time,
                UpopStatusId = UPOPStatus.GetDefaultStatus(_db).Id,
                Version = newVersion,
                VersionedDocument = newDocument,
                BasedOn = wp
            };

            _db.BasicCharacteristicOPs.Add(newWp);

            if (!UserHasAccess(_user, newDocument))
                throw new InvalidOperationException("Нет прав на создание документа.");

            _db.SaveChanges();

            return newDocument;
        }

        public IReadOnlyDictionary<VersionedDocument, DocumentVersionInfo> GetDocumentsAndVersions(int linkedEntityId)
        {
            var versions = _db.BasicCharacteristicOPs.Where(d => d.InfoId == linkedEntityId)
                .OrderByDescending(d => d.Version)
                .ToList()
                .ToDictionary(d => d.VersionedDocument, d => new DocumentVersionInfo(d.Version));

            return versions;
        }

        protected override IReadOnlyDictionary<string, object> GetDefaultBlockValues(string linkedEntityId)
        {
            int infoId;
            if (!int.TryParse(linkedEntityId, out infoId))
                return new Dictionary<string, object>();

            var info = _db.BasicCharacteristicOPInfos.FirstOrDefault(i => i.Id == infoId);
            if (info == null)
                return new Dictionary<string, object>();

            var defaultValues = new BasicCharacteristicOPDefaultValues(info);

            var values = new Dictionary<string, object>();          

            values.Add(nameof(BasicCharacteristicOPSchemaModel.UniversalCompetences), defaultValues.UniversalCompetences());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.GeneralCompetences), defaultValues.GeneralCompetences());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.FormAndDuration), defaultValues.FormAndDuration());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.Language), defaultValues.Language());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.RequisitesOrders), defaultValues.RequisitesOrders());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.PurposeAndFeature), defaultValues.PurposeAndFeature());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.EduProgramInfo), defaultValues.EduProgramInfo());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.Institute), defaultValues.Institute());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.Direction), defaultValues.Direction());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.Profile), defaultValues.Profile());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.Chair), defaultValues.Chair());
            values.Add(nameof(BasicCharacteristicOPSchemaModel.ModuleStructure), defaultValues.ModuleStructure(structure: new Shared.ModuleStructure(), addModulesFromDb: true));
            values.Add(nameof(BasicCharacteristicOPSchemaModel.ProfStandardsList), defaultValues.ProfStandardsList(codes: new List<string>().ToArray()));
            values.Add(nameof(BasicCharacteristicOPSchemaModel.RatifyingInfo), defaultValues.RatifyingInfo());
            
            return values;
        }

        protected override VersionedDocumentType GetDocumentType()
        {
            return VersionedDocumentType.BasicCharacteristicOP;
        }

        protected override string GetStandard()
        {
            throw new NotImplementedException();
        }

    }
}