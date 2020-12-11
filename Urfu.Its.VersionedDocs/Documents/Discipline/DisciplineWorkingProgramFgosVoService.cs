using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Principal;
using Autofac;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.Discipline
{
    public class DisciplineWorkingProgramFgosVoService : WorkingProgramService<DisciplineWorkingProgramFgosVoSchemaModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentSchemaService _schemaService;
        private readonly string _moduleId;
        private readonly IComponentContext _context;
        private readonly IPrincipal _user;
        private readonly VersionedDocumentType _documentType = VersionedDocumentType.DisciplineWorkingProgram;

        public DisciplineWorkingProgramFgosVoService(ApplicationDbContext db, IVersionedDocumentSchemaService schemaService, string moduleId, IComponentContext context, IVersionedDocumentModelDescriptorFactory<DisciplineWorkingProgramFgosVoSchemaModel> descriptorFactory, IVersionedDocumentDescriptorService descriptorService, ILifetimeScope scope, IPrincipal user) 
            : base(db, schemaService, descriptorFactory, descriptorService, scope, user)
        {
            _db = db;
            _schemaService = schemaService;
            _moduleId = moduleId;
            _context = context;
            _user = user;
        }

        public override IEnumerable<WorkingProgramSection> GetSections()
        {
            yield break;
        }

        public override VersionedDocumentTemplate GetDocumentTemplate()
        {
            return _db.VersionedDocumentTemplates.Where(t=>t.DocumentType == _documentType).OrderByDescending(t=>t.Version).First();
        }

        public override VersionedDocument CreateDocument(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null)
        {
            if(string.IsNullOrWhiteSpace(_moduleId))
                throw new InvalidOperationException("Не указан модуль, с которым необходимо сделать привязку для нового РПД");

            var versions = GetDocumentsAndVersions(linkedEntityId);

            var workingProgram = new DisciplineWorkingProgram
            {
                DisciplineId = linkedEntityId,
                StandardName = StandardNames.FgosVo,
                Version = 1 + (versions.Any() ? versions.Max(v=>v.Value.VersionNumber) : 0),
            };
            _db.DisciplineWorkingPrograms.Add(workingProgram);

            var mwp = _db.ModuleWorkingPrograms.Where(p => p.ModuleId == _moduleId).OrderByDescending(p => p.Version).FirstOrDefault();

            var mwpService = _context.ResolveKeyed<IWorkingProgramService>(VersionedDocumentType.ModuleWorkingProgram);
            if (mwp != null)
            {
                var actualMwpSchema = mwpService.GetDocumentDescriptor().GenerateSchemaString();
                if (!_schemaService.IsSchemaMatched(mwp.VersionedDocument.Template.Schema, actualMwpSchema))
                    mwp = null;
            }

            VersionedDocument mwpDoc;

            if (mwp == null)
            {
                mwpDoc = mwpService.CreateDocument(_moduleId, standard);
                workingProgram.ModuleWorkingProgramId = mwpDoc.Id;                
            }
            else
            {
                mwpDoc = mwp.VersionedDocument;
                workingProgram.ModuleWorkingProgramId = mwp.VersionedDocumentId;
            }

            // блоки заполняются в цикле ниже
            var sharedBlocks = new Dictionary<string, VersionedDocumentBlock>
            {
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.Module)] = null,
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.Institute)] = null,
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.Directions)] = null,
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.Profiles)] = null,
                //[nameof(ModuleWorkingProgramFgosVoSchemaModel.Authors)] = null,
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.Head)] = null,
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.Council)] = null,
                //[nameof(ModuleWorkingProgramFgosVoSchemaModel.Direction)] = null,
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.RequisitesOrders)] = null,                
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.Fdps)] = null,          
                [nameof(ModuleWorkingProgramFgosVoSchemaModel.ControlEventsEstimationCriterias)] = null
            };
            foreach (var link in mwpDoc.BlockLinks)
            {
                var block = link.DocumentBlock;
                var blockName = block.Name;
                if (sharedBlocks.ContainsKey(blockName))
                    sharedBlocks[blockName] = block;
            }

            var document = CreateWorkingProgramDocumentCore(_documentType, null, sharedBlocks);
            _db.VersionedDocuments.Add(document);

            workingProgram.VersionedDocument = document;

            if (!UserHasAccess(_user, document))
                throw new InvalidOperationException("Нет прав на создание документа.");

            return document;
        }

        public override IReadOnlyDictionary<VersionedDocument, DocumentVersionInfo> GetDocumentsAndVersions(string linkedEntityId, string standard = null, int? year = null, int? planNumber = null, int? planVersionNumber = null)
        {
            var versions = _db.DisciplineWorkingPrograms
                .Include(d=>d.ModuleWorkingProgram)
                .Include(d=>d.BasedOn)
                .Where(d => d.DisciplineId == linkedEntityId && d.StandardName == StandardNames.FgosVo)
                .OrderByDescending(d => d.ModuleWorkingProgram.Version)
                .ThenByDescending(d => d.Version)
                .ToList()
                .ToDictionary(d => d.VersionedDocument, d => (DocumentVersionInfo) new DisciplineWorkingProgramVersionInfo(d.Version, d.ModuleWorkingProgram.Version, d.BasedOn?.Version));
            return versions;
        }

        public override DocumentPartViewModel GetNavigationViewModel(VersionedDocument document)
        {
            var wp = _db.DisciplineWorkingPrograms.Find(document.Id);
            return new DisciplineWorkingProgramViewModel(wp, _context.Resolve<IVersionedDocumentService>())
            {
                AllowEdit = IsInEditableState(document)
            };
        }

        public override bool IsInEditableState(VersionedDocument document)
        {
            var editableState = base.IsInEditableState(document);

            if (!editableState)
                return false;

            //var wp = _db.DisciplineWorkingPrograms.Find(document.Id);
            // TODO нужна проверка статусов, в которых еще возможно редактирование

            if (!UserHasAccess(_user, document))
                return false;

            return true;
        }

        public override bool UserHasAccess(IPrincipal user, VersionedDocument document)
        {
            if (_user.IsInRole(ItsRoles.Admin))
                return true;

            if (!base.UserHasAccess(user, document))
                return false;

            var documentId = document.Id;
            var dwp = _db.DisciplineWorkingPrograms.Find(documentId);
            var userId = _user.Identity.Name;
            var disciplineId = dwp?.DisciplineId;
            if (dwp == null || !_db.WorkingProgramResponsiblePersons.Any(p => p.DisciplineId == disciplineId && p.UserId == userId))
                return false;

            return true;
        }

        public override VersionedDocument CreateDocumentBasedOn(VersionedDocument document, int? year = null)
        {
            var wp = _db.DisciplineWorkingPrograms.Find(document.Id);            

            var versions = GetDocumentsAndVersions(wp.DisciplineId);
            var newVersion = 1 + (versions.Any() ? versions.Max(v => v.Value.VersionNumber) : 0);

            var newDocument = CloneDocument(document);
            _db.VersionedDocuments.Add(newDocument);

            var newWp = new DisciplineWorkingProgram
            {
                Discipline = wp.Discipline,
                ModuleWorkingProgram = wp.ModuleWorkingProgram,
                Standard = wp.Standard,
                Version = newVersion,
                VersionedDocument = newDocument,
                BasedOn = wp,
            };

            _db.DisciplineWorkingPrograms.Add(newWp);

            if (!UserHasAccess(_user, newDocument))
                throw new InvalidOperationException("Нет прав на создание документа.");

            return newDocument;
        }        

        public override void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document)
        {
            builder.RegisterInstance(document);
            var wp = _db.DisciplineWorkingPrograms.Find(document.Id);
            builder.RegisterInstance(wp);
            builder.RegisterInstance(wp.Discipline);
            builder.RegisterInstance(wp.ModuleWorkingProgram.Module);
            builder.RegisterInstance(wp.ModuleWorkingProgram);
        }
    }    
}