using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Autofac;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.ModuleChangeList;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Services
{
    public abstract class ModuleWorkingProgramServiceBase<T> : WorkingProgramService<T>
    {
        private readonly ApplicationDbContext _db;
        private readonly ILifetimeScope _scope;
        private readonly IPrincipal _user;

        protected ModuleWorkingProgramServiceBase(ApplicationDbContext db,
            IVersionedDocumentSchemaService schemaService,
            IVersionedDocumentModelDescriptorFactory<T> descriptorFactory, 
            ILifetimeScope scope,
            IVersionedDocumentDescriptorService descriptorService,
            IPrincipal user) : base(db, schemaService, descriptorFactory, descriptorService, scope, user)
        {
            _db = db;
            _scope = scope;
            _user = user;
        }

        protected abstract VersionedDocumentType GetDocumentType();

        protected abstract string GetStandard();

        public override VersionedDocument CreateDocument(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null)
        {
            var versions = GetDocumentsAndVersions(linkedEntityId);

            var defaultModel = GetDefaultBlockValues(linkedEntityId);
            var document = CreateWorkingProgramDocumentCore(GetDocumentType(), defaultModel);

            _db.VersionedDocuments.Add(document);

            var workingProgram = new ModuleWorkingProgram
            {
                VersionedDocumentId= document.Id, 
                VersionedDocument = document,
                Module = _db.Modules.Find(linkedEntityId),
                StandardName = GetStandard(),
                UpopStatusId = UPOPStatus.GetDefaultStatus(_db).Id,
                StatusChangeTime = DateTime.Now,
                Version = 1 + (versions.Any() ? versions.Max(v => v.Value.VersionNumber) : 0)
            };
            _db.ModuleWorkingPrograms.Add(workingProgram);

            if (!UserHasAccess(_user, document))
                throw new InvalidOperationException("Нет прав на создание документа.");

            return document;
        }

        protected abstract IReadOnlyDictionary<string, object> GetDefaultBlockValues(string linkedEntityId);

        public override IReadOnlyDictionary<VersionedDocument, DocumentVersionInfo> GetDocumentsAndVersions(string linkedEntityId, string standard = null, int? year = null, int? planNumber = null, int? planVersionNumber = null)
        {
            standard = GetStandard();
            var versions = _db.ModuleWorkingPrograms.Where(d => d.ModuleId == linkedEntityId && d.StandardName == standard)
                .OrderByDescending(d=>d.Version)
                .ToList()
                .ToDictionary(d => d.VersionedDocument, d => new DocumentVersionInfo(d.Version));

            return versions;
        }

        public override VersionedDocument CreateDocumentBasedOn(VersionedDocument document, int? year = null)
        {
            var wp = _db.ModuleWorkingPrograms.Find(document.Id);

            var versions = GetDocumentsAndVersions(wp.ModuleId);
            var newVersion = 1 + (versions.Any() ? versions.Max(v => v.Value.VersionNumber) : 0);

            var time = DateTime.Now;

            var newDocument = CloneDocument(document, time);
            _db.VersionedDocuments.Add(newDocument);

            var newWp = new ModuleWorkingProgram
            {
                Module = wp.Module,
                StandardName = wp.StandardName,
                StatusChangeTime = time,
                Version = newVersion,
                VersionedDocument = newDocument,
                BasedOn = wp
            };            

            _db.ModuleWorkingPrograms.Add(newWp);

            var disciplineService = _scope.Resolve<IWorkingProgramService<DisciplineWorkingProgramFgosVoSchemaModel>>();

            var maxVersions = wp.DisciplineWorkingPrograms.Select(p=>p.DisciplineId).Distinct().ToDictionary(disciplineId=>disciplineId, disciplineId=>
            {
                var disciplineVersions = disciplineService.GetDocumentsAndVersions(disciplineId);
                var maxCurrentVersion = disciplineVersions.Any() ? disciplineVersions.Max(v => v.Value.VersionNumber) : 0;
                return maxCurrentVersion;
            });

            var dwps = new List<DisciplineWorkingProgram>();
            
            foreach (var dwp in wp.DisciplineWorkingPrograms)
            {
                var disciplineDocument = dwp.VersionedDocument;

                var nextVersion = ++maxVersions[dwp.DisciplineId];
                
                var newDisciplineDocument = CloneDocument(disciplineDocument);
                _db.VersionedDocuments.Add(newDisciplineDocument);

                var newDwp = new DisciplineWorkingProgram
                {
                    Discipline = dwp.Discipline,
                    ModuleWorkingProgram = newWp,
                    Standard = dwp.Standard,
                    Version = nextVersion,
                    VersionedDocument = newDisciplineDocument,
                    BasedOn = dwp,
                };

                dwps.Add(newDwp);
            }

            newWp.DisciplineWorkingPrograms = dwps;

            if (!UserHasAccess(_user, newDocument))
                throw new InvalidOperationException("Нет прав на создание документа.");

            return newDocument;
        }

        public override bool IsInEditableState(VersionedDocument document)
        {
            var editableState = base.IsInEditableState(document);

            if (!editableState)
                return false;

            if (!UserHasAccess(_user, document))
                return false;

            //var wp = _db.ModuleWorkingPrograms.Find(document.Id);
            // TODO нужна проверка статусов, в которых еще возможно редактирование
            return true;
        }

        public override bool UserHasAccess(IPrincipal user, VersionedDocument document)
        {
            if (user.IsInRole(ItsRoles.Admin))
                return true;

            if (!base.UserHasAccess(user, document))
                return false;

            if (!user.IsInRole(ItsRoles.WorkingProgramManager))
                return false;

            if (!user.IsInRole(ItsRoles.AllDirections))
            {
                var modulesQuery = _db.ModulesForUser(user);
                var userName = user.Identity.Name;
                var divisions = (from ud in _db.UserDivisions
                    join d in _db.Divisions
                        on ud.DivisionId equals d.uuid
                    where ud.UserName == userName
                    select d).ToList();

                var divisionFullNames = divisions.SelectMany(d => new[]
                {
                    d.typeTitle + " «" + d.title + "»",
                    "«" + d.title + "»"
                }).ToList();

                modulesQuery = modulesQuery.Where(m => divisionFullNames.Contains(m.coordinator));

                var moduleIds = modulesQuery.Select(m => m.uuid);
                var docId = document.Id;
                if (document?.Template?.DocumentType == VersionedDocumentType.BasicCharacteristicOP
                        || document?.Template?.DocumentType == VersionedDocumentType.CompetencePassport
                        || document?.Template?.DocumentType == VersionedDocumentType.ModuleAnnotation)
                {
                    return true;
                }

                var currentModuleId = _db.ModuleWorkingPrograms.Find(docId).ModuleId;
                if (!moduleIds.Contains(currentModuleId))
                    return false;
            }

            return true;
        }
    }
}