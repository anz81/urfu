using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
    //System.Web.Script.Serialization;
using Autofac.Features.Indexed;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.ModuleChangeList;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.WorkingProgramView)]
    public class WorkingProgramsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;
        private readonly IIndex<VersionedDocumentType, IVersionedDocumentImplementationService> _documentImplementationServices;
        private readonly IObjectLogger<WorkingProgramsController> _logger;

        public WorkingProgramsController(ApplicationDbContext db, 
            IVersionedDocumentService documentService,
            IIndex<VersionedDocumentType, IVersionedDocumentImplementationService> documentImplementationServices,
            IObjectLogger<WorkingProgramsController> logger)
        {
            _db = db;
            _documentService = documentService;
            _documentImplementationServices = documentImplementationServices;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetStandards()
        {
            return Json(_db.Standards, new JsonSerializerSettings());
        }

        [HttpGet]
        public ActionResult GetVersions(string linkedEntityId, string standard, DocumentKind documentKind, int? year)
        {
            var documentType = GetDocumentType(documentKind, standard);
            var service = GetWorkingProgramService(documentType);
            var versions = service.GetDocumentsAndVersions(linkedEntityId, standard, year);

            return Json(versions.Select(v => new
            {
                DocumentId = v.Key.Id,
                Name = v.Value.VersionDisplayName
            }), new JsonSerializerSettings());
        }

        [HttpPost]
        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public ActionResult CreateNew(string linkedEntityId, string standard, DocumentKind documentKind, int? year, int? planNumber, int? planVersionNumber)
        {
            _logger.Debug("Попытка создания нового документа: linkedEntityId='{0}', standard='{1}', documentKind='{2}'", linkedEntityId, standard, documentKind);
            var documentType = GetDocumentType(documentKind, standard);
            var service = GetWorkingProgramService(documentType);
            var document = service.CreateDocument(linkedEntityId, standard, year, planNumber, planVersionNumber);
            
            _db.SaveChanges();
            _logger.Debug("Документ типа '{0}' с идентификатором {1} создан и сохранен", documentType, document.Id);

            string uid;
            string title;
            int version;

            switch (documentType)
            {
                case VersionedDocumentType.ModuleWorkingProgram:                    
                case VersionedDocumentType.GiaWorkingProgram:                    
                case VersionedDocumentType.PracticesWorkingProgram:
                    var mwp = _db.ModuleWorkingPrograms
                        .Include(w=>w.Module)
                        .FirstOrDefault(w => w.VersionedDocumentId == document.Id);
                    uid = mwp.ModuleId;
                    title = mwp.Module.title;
                    version = mwp.Version;
                    break;
                case VersionedDocumentType.DisciplineWorkingProgram:
                    var dwp = _db.DisciplineWorkingPrograms.Include(w=>w.Discipline)
                        .FirstOrDefault(w=>w.VersionedDocumentId == document.Id);
                    uid = dwp.DisciplineId;
                    title = dwp.Discipline.title;
                    version = dwp.Version;
                    break;
                case VersionedDocumentType.BasicCharacteristicOP:
                    var ohop = _db.BasicCharacteristicOPs.Include(p => p.Info).Include(p => p.Status)
                        .FirstOrDefault(b => b.VersionedDocumentId == document.Id);
                    uid = $"{ohop.InfoId}";
                    title = ohop.Info.Profile.NAME;
                    version = ohop.Version;
                    break;
                case VersionedDocumentType.CompetencePassport:
                    var passport = _db.CompetencePassports.Include(p => p.BasicCharacteristicOP).Include(p => p.Status)
                        .FirstOrDefault(b => b.VersionedDocumentId == document.Id);
                    uid = $"{passport.BasicCharacteristicOPId}";
                    title = passport.BasicCharacteristicOP.Info.Profile.NAME;
                    version = passport.Version;
                    break;
                case VersionedDocumentType.ModuleAnnotation:
                    var annotation = _db.ModuleAnnotations.Include(p => p.BasicCharacteristicOP).Include(p => p.Status)
                        .FirstOrDefault(b => b.VersionedDocumentId == document.Id);
                    uid = $"{annotation.BasicCharacteristicOPId}";
                    title = annotation.BasicCharacteristicOP.Info.Profile.NAME;
                    version = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _logger.Info("Создание версии {0}, '{1}', uid - '{2}', версия - '{3}'", 
                EnumHelper<VersionedDocumentType>.GetDisplayValue(documentType),
                title, uid, version);

            return Json(new
            {
                redirect = Url.Action("Index", "Document", new { id = document.Id })
            }, new JsonSerializerSettings());
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task<ActionResult> CreateBasedOn(int documentId, int? year)
        {
            _logger.Debug("Попытка создания нового документа на основе документа с идентификатором {0}", documentId);
            var document = await _db.VersionedDocuments
                .Include(d => d.Template)
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .FirstOrDefaultAsync(d => d.Id == documentId);
            if (document == null)
                throw new Exception($"Документ с идентификатором {documentId} не найден");

            var service = GetWorkingProgramService(document.Template.DocumentType);
            var newDocument = service.CreateDocumentBasedOn(document, year);
            
            _db.SaveChanges();
            _logger.Debug("Документ с идентификатором {0} создан на основе документа с идентификатором {1} и сохранен", newDocument.Id, documentId);

            string uid;
            string title;
            int version;

            var documentType = document.Template.DocumentType;
            switch (documentType)
            {
                case VersionedDocumentType.ModuleWorkingProgram:
                case VersionedDocumentType.GiaWorkingProgram:
                case VersionedDocumentType.PracticesWorkingProgram:
                    var mwp = _db.ModuleWorkingPrograms
                        .Include(w => w.Module)
                        .FirstOrDefault(w => w.VersionedDocumentId == document.Id);
                    uid = mwp.ModuleId;
                    title = mwp.Module.title;
                    version = mwp.Version;
                    break;
                case VersionedDocumentType.DisciplineWorkingProgram:
                    var dwp = _db.DisciplineWorkingPrograms.Include(w => w.Discipline)
                        .FirstOrDefault(w => w.VersionedDocumentId == document.Id);
                    uid = dwp.DisciplineId;
                    title = dwp.Discipline.title;
                    version = dwp.Version;
                    break;
                case VersionedDocumentType.BasicCharacteristicOP:
                    var ohop = _db.BasicCharacteristicOPs.Include(p => p.Info).Include(p => p.Status)
                        .FirstOrDefault(b => b.VersionedDocumentId == newDocument.Id);
                    uid = $"{ohop.InfoId}";
                    title = ohop.Info.Profile.NAME;
                    version = ohop.Version;
                    break;
                case VersionedDocumentType.CompetencePassport:
                    var passport = _db.CompetencePassports.Include(p => p.BasicCharacteristicOP).Include(p => p.Status)
                        .FirstOrDefault(b => b.VersionedDocumentId == newDocument.Id);
                    uid = $"{passport.BasicCharacteristicOPId}";
                    title = passport.BasicCharacteristicOP.Info.Profile.NAME;
                    version = passport.Version;
                    break;
                case VersionedDocumentType.ModuleAnnotation:
                    var annotation = _db.ModuleAnnotations.Include(p => p.BasicCharacteristicOP).Include(p => p.Status)
                        .FirstOrDefault(b => b.VersionedDocumentId == document.Id);
                    uid = $"{annotation.BasicCharacteristicOPId}";
                    title = annotation.BasicCharacteristicOP.Info.Profile.NAME;
                    version = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _logger.Info("Создание версии {0}, '{1}', uid - '{2}', версия - '{3}'",
                EnumHelper<VersionedDocumentType>.GetDisplayValue(documentType),
                title, uid, version);

            return Json(new
            {
                redirect = Url.Action("Index", "Document", new { id = newDocument.Id })
            }, new JsonSerializerSettings());
        }

        private IWorkingProgramService GetWorkingProgramService(VersionedDocumentType documentType)
        {
            return (IWorkingProgramService) _documentImplementationServices[documentType];
        }

        public async Task<ActionResult> GetInstitutes(int documentId, VersionedDocumentType documentType)
        {
            List<InstituteInfo> divisions = new List<InstituteInfo>();
            if (documentType == VersionedDocumentType.BasicCharacteristicOP)
            {
                var document = _db.BasicCharacteristicOPs.FirstOrDefault(b => b.VersionedDocumentId == documentId);
                Division division = _db.GetInstituteForChair(document?.Info?.Profile?.CHAIR_ID);
                divisions.Add(new InstituteInfo()
                {
                    Id = division.uuid,
                    Name = division.title
                });
            }
            else
            {
                divisions = await _db.Divisions.Where(d => d.typeCode == "institute" || d.typeCode == "faculty" || d.typeCode == "branch")
                    .OrderBy(d => d.title)
                    .Select(d => new InstituteInfo
                    {
                        Id = d.uuid,
                        Name = d.title
                    }).ToListAsync();
            }
            return Json(divisions, new JsonSerializerSettings());
        }

        public async Task<ActionResult> GetDirections(int documentId, VersionedDocumentType documentType)
        {
            var document = _db.VersionedDocuments.Include(d => d.BlockLinks.Select(l => l.DocumentBlock)).FirstOrDefault(d => d.Id == documentId);
            dynamic model = _documentService.CreateProxyModel(document, documentType == VersionedDocumentType.BasicCharacteristicOP ? "Info" : "Module", "Institute");

            var directions = await Task.Run(() =>
            {
                var instituteId = (string)model.Institute.Id;
                //var instituteDivision = _db.Divisions.Find(instituteId);
                var childrenDivisions1 = _db.Divisions.Where(d => d.parent == instituteId).ToList();
                var childrenDivisions1Ids = childrenDivisions1.Select(d => d.uuid).ToList();
                var childrenDivisions2 = _db.Divisions.Where(d => childrenDivisions1Ids.Contains(d.parent)).ToList();
                var allPossibleChairIds = childrenDivisions1.Concat(childrenDivisions2).Select(d=>d.uuid).ToList();

                var instituteProfiles = _db.Profiles.Where(p =>allPossibleChairIds.Contains(p.CHAIR_ID) && !p.remove);
                var institureDirectionIds = instituteProfiles.Select(p => p.DIRECTION_ID).Distinct().ToList();

                var directionsList = new List<Direction>();
                var result = new List<DirectionViewModel>();

                if (documentType == VersionedDocumentType.BasicCharacteristicOP)
                {
                    int infoId = (int)model.Info.Id;
                    var info = _db.BasicCharacteristicOPInfos.FirstOrDefault(i => i.Id == infoId);

                    directionsList = new List<Direction>() { info.Profile.Direction };
                }
                else
                {
                    var moduleId = (string)model.Module.Id;
                    var module = _db.Modules.Find(moduleId);
                    directionsList = module.Directions.Where(d => institureDirectionIds.Contains(d.uid)).ToList();
                }
                
                result = directionsList.Select(d => new DirectionViewModel
                    {
                        Id = d.uid,
                        Code = d.okso,
                        Title = d.title,
                        Qualifications = d.qualifications,
                        Standard = d.standard
                    }).ToList();
                return result;
            });
            
            return Json(directions, new JsonSerializerSettings());
        }

        public async Task<ActionResult> GetDocumentDirections(int documentId, VersionedDocumentType documentType)
        {
            var document = _db.VersionedDocuments.Include(d => d.BlockLinks.Select(l => l.DocumentBlock)).FirstOrDefault(d => d.Id == documentId);

            dynamic modelDirections = _documentService.CreateProxyModel(document, "Directions");
            var directionIds = new List<string>();
            foreach (var d in modelDirections.Directions)
                directionIds.Add(d.Id);

            var directions = await Task.Run(() =>
            {
                var result = _db.Directions
                    .Where(d=>directionIds.Contains(d.uid))
                    .Select(d => new DirectionViewModel
                    {
                        Id = d.uid,
                        Code = d.okso,
                        Title = d.title,
                        Qualifications = d.qualifications,
                        Standard = d.standard
                    }).ToList();

                return result;
            });

            return Json(directions, new JsonSerializerSettings());
        }

        public async Task<ActionResult> GetProfiles(int documentId, VersionedDocumentType documentType)
        {
            var document = _db.VersionedDocuments.Include(d=>d.BlockLinks.Select(l=>l.DocumentBlock)).FirstOrDefault(d=>d.Id == documentId);
            
            dynamic modelDirections = _documentService.CreateProxyModel(document, "Directions");
            var directionIds = new List<string>();
            foreach (var d in modelDirections.Directions)
                directionIds.Add(d.Id);

            var profiles = await _db.Profiles
                .Where(p=> directionIds.Contains(p.DIRECTION_ID) && !p.remove)
                .OrderBy(p=>p.CODE)
                .Select(d => new ProfileTrajectoriesViewModel
                {
                    Id = d.ID,
                    Name = d.NAME,
                    Code = d.CODE,
                    DirectionId = d.DIRECTION_ID
                }).ToListAsync();

            return Json(profiles, new JsonSerializerSettings());
        }

        public async Task<ActionResult> GetAuthors(int documentId, VersionedDocumentType documentType)
        {
            var teachers = await _db.Teachers
                .Select(d => new
                {
                    teacher = d,
                    cathedra = _db.Divisions.FirstOrDefault(d2 => d2.uuid == d.division)
                })
                .Select(d => new AuthorInfo
                {
                    TeacherId = d.teacher.pkey,
                    Cathedra = d.cathedra.typeTitle + " " + d.cathedra.title,
                    Degree = d.teacher.academicDegree + (d.teacher.academicDegree != null && d.teacher.academicTitle != null ? ", " : "") + d.teacher.academicTitle,
                    Fio = d.teacher.lastName + " " + d.teacher.firstName + " " + d.teacher.middleName,
                    ShortName = d.teacher.firstName.Substring(0, 1) + "." + d.teacher.middleName.Substring(0, 1) + ". " + d.teacher.lastName,
                    Post = d.teacher.post
                }).ToListAsync();

            var authors = await _db.WorkingProgramAuthors
                .Select(d => new AuthorInfo
                {
                    AuthorId = d.Id,
                    Cathedra = d.Workplace,
                    Degree = d.AcademicDegree + (d.AcademicDegree != null && d.AcademicTitle != null ? ", " : "") + d.AcademicTitle,
                    Fio = d.LastName + " " + d.FirstName + " " + d.MiddleName,
                    ShortName = d.FirstName.Substring(0, 1) + "." + d.MiddleName.Substring(0, 1) + ". " + d.LastName,
                    Post = d.Post
                })
                .ToListAsync();

            var allAuthors = teachers.Concat(authors).OrderBy(a => a.Fio).ToList();

            /*var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = serializer.Serialize(allAuthors),
                ContentType = "application/json"
            };*/
            return Json(allAuthors, new JsonSerializerSettings());
        }

        public async Task<ActionResult> GetRequisitesOrders(int documentId, VersionedDocumentType documentType)
        {
            var document = _db.VersionedDocuments.Include(d => d.BlockLinks.Select(l => l.DocumentBlock)).FirstOrDefault(d => d.Id == documentId);

            dynamic modelDirections = _documentService.CreateProxyModel(document, "Directions");
            var directionIds = new List<string>();
            foreach (var d in modelDirections.Directions)
                directionIds.Add(d.Id);

            var dbRequisites = await _db.RequisiteOrderFgoss.Include(r=>r.Direction).Where(r=>directionIds.Contains(r.DirectionId)).ToListAsync();
            var requisites = dbRequisites                
                .Select(r=>new RequisitesOrderFgosInfo
                {
                    Id = r.Id,
                    Number = r.Order,
                    Date = r.Date.ToString("dd.MM.yyyy"),
                    DirectionCode = r.Direction.okso
                }).ToList();

            return Json(requisites, new JsonSerializerSettings());
        }

        [ErrorFilter]
        public ActionResult CreateAuthor([ExcludeBind("Id")]WorkingProgramAuthor author)
        {
            _db.WorkingProgramAuthors.Add(author);
            _db.SaveChanges();

            var authorInfo = new AuthorInfo
            {
                AuthorId = author.Id,
                Cathedra = author.Workplace,
                Degree = author.AcademicDegree + (author.AcademicDegree != null && author.AcademicTitle != null ? ", " : "") + author.AcademicTitle,
                Fio = author.LastName + " " + author.FirstName + " " + author.MiddleName,
                ShortName = author.FirstName.Substring(0, 1) + "." + author.MiddleName.Substring(0, 1) + ". " + author.LastName,
                Post = author.Post
            };

            return Json(authorInfo, new JsonSerializerSettings());
        }

        public ActionResult CreateRequisiteOrder([ExcludeBind("Id")]RequisiteOrderFGOS r)
        {
            _db.RequisiteOrderFgoss.Add(r);
            _db.SaveChanges();

            r = _db.RequisiteOrderFgoss.Include(r2=>r2.Direction).FirstOrDefault(r2=>r2.Id == r.Id);

            var info = new RequisitesOrderFgosInfo
            {
                Id = r.Id,
                Number = r.Order,
                Date = r.Date.ToString("dd.MM.yyyy"),
                DirectionCode = r.Direction.okso,
                DirectionId = r.DirectionId
            };

            return Json(info, new JsonSerializerSettings());
        }

        public async Task<ActionResult> GetPlans(int documentId, VersionedDocumentType documentType, string famType, string directionId)
        {
            string moduleId;
            switch (documentType)
            {
                case VersionedDocumentType.DisciplineWorkingProgram:
                //case VersionedDocumentType.DisciplineWorkingProgramFgosVo3:
                //    throw new InvalidOperationException("Для рабочей программы дисциплины не доступен список УП. Действие отклонено.");
                //    break;
                default:
                    var wp = _db.ModuleWorkingPrograms.Find(documentId);
                    moduleId = wp.ModuleId;
                    break;
            }

            var plans = (await _db.Plans.Where(p => p.moduleUUID == moduleId && p.familirizationType == famType && p.directionId == directionId)
                .GroupBy(p=>p.versionUUID)
                .ToListAsync())
                .Select(g=>
                {
                    var p = g.First();
                    return new PlanViewModel
                    {
                        VersionId = p.versionUUID,
                        EduPlanNumber = p.eduplanNumber,
                        VersionNumber = p.versionNumber,                        
                        DisciplineId = p.disciplineUUID,
                        EduPlanId = p.eduplanUUID,
                        ModuleId = p.moduleUUID
                    };
                });

            return Json(plans, new JsonSerializerSettings());
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task RemoveModuleWorkingProgram(int id, bool confirmed)
        {
            var wp = await _db.ModuleWorkingPrograms.FirstOrDefaultAsync(w => w.VersionedDocumentId == id);
            if (wp == null)
                throw new InvalidOperationException($"Не найдена версия РП с идентификатором {id}.");

            if (wp.DisciplineWorkingPrograms.Any())
                throw new InvalidOperationException("С версией РПМ связаны документы РПД. Удаление невозможно.");

            if (wp.TargetChangeLists.Any() || wp.SourceChangeLists.Any())
                throw new InvalidOperationException("С версией РП связаны документы ЛИ. Удаление невозможно.");
            
            var moduleId = wp.ModuleId;
            var hasInheritedDocuments = _db.ModuleWorkingPrograms.Where(w => w.ModuleId == moduleId).Any(w => w.BasedOnId == id);
            if (hasInheritedDocuments)
                throw new InvalidOperationException("На выбранной версии РП созданы на основе другие документы. Удаление невозможно.");

            // TODO 21.03.2018 статусов еще нет. Нужна проверка на возможность удаления в зависимости от статусов РП
            //if (wp.Status != ModuleWorkingProgramStatus.Draft)
            //    throw new InvalidOperationException("Невозможно удалить. Статус не позволяет.");

            if (confirmed)
            {
                var documentType = wp.VersionedDocument.Template.DocumentType;
                string uid;
                string title;
                int version;

                switch (documentType)
                {
                    case VersionedDocumentType.ModuleWorkingProgram:
                    case VersionedDocumentType.GiaWorkingProgram:
                    case VersionedDocumentType.PracticesWorkingProgram:
                        /*var mwp = _db.ModuleWorkingPrograms
                            .Include(w => w.Module)
                            .FirstOrDefault(w => w.VersionedDocumentId == wp.VersionedDocument.Id);*/
                        var mwp = wp;
                        uid = mwp.ModuleId;
                        title = mwp.Module.title;
                        version = mwp.Version;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _logger.Debug("Попытка удаления РП с идентификатором {0}", id);
                var doc = wp.VersionedDocument;
                _db.ModuleWorkingPrograms.Remove(wp);
                var blocksToRemove = BlockDataHelper.GetIndependentBlocks(_db, doc);
                _db.VersionedDocuments.Remove(doc);
                _db.VersionedDocumentBlocks.RemoveRange(blocksToRemove);

                _db.SaveChanges();
                _logger.Debug("Удален РП с идентификатором {0}", id);

                _logger.Info("Удаление версии {0}, '{1}', uid - '{2}', версия - '{3}'",
                    EnumHelper<VersionedDocumentType>.GetDisplayValue(documentType), title, uid, version);
            }
        }

        public async Task<ActionResult> GetChangeListTargetVersions(int id)
        {
            var wp = await _db.ModuleWorkingPrograms
                .Include(w=>w.VersionedDocument)
                .Include(w=>w.VersionedDocument.Template)
                .FirstOrDefaultAsync(w => w.VersionedDocumentId == id);

            if (wp == null)
                throw new InvalidOperationException($"Не найдена РП по идентификатору {id}");

            var documentType = wp.VersionedDocument.Template.DocumentType;
            var service = GetWorkingProgramService(documentType);
            var versions = service.GetDocumentsAndVersions(wp.ModuleId);

            return Json(versions.Where(v=>v.Key.Id != id).Select(v => new
            {
                DocumentId = v.Key.Id,
                Name = v.Value.VersionDisplayName
            }), new JsonSerializerSettings());
        }

        public async Task<ActionResult> GetChangeLists(int id)
        {
            var lists = await _db.ModuleWorkingProgramChangeLists
                .Include(c=>c.Target)
                .Include(c=>c.Source)
                .Where(c => c.SourceId == id || c.TargetId == id).ToListAsync();

            var vms = lists.Select(l => new
            {
                l.VersionedDocumentId,
                l.SourceId,
                l.TargetId,
                SourceDisplayName = l.Source.Version.ToString(),
                TargetDisplayName = l.Target.Version.ToString()
            }).ToList();

            return Json(vms, new JsonSerializerSettings());
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task RemoveChangeList(int id, bool confirmed)
        {
            var wp = await _db.ModuleWorkingProgramChangeLists.FirstOrDefaultAsync(w => w.VersionedDocumentId == id);
            if (wp == null)
                throw new InvalidOperationException($"Не найден ЛИ с идентификатором {id}.");

            if (confirmed)
            {
                _logger.Debug("Попытка удаления ЛИ с идентификатором {0}", id);
                var doc = wp.VersionedDocument;
                _db.ModuleWorkingProgramChangeLists.Remove(wp);
                var blocksToRemove = BlockDataHelper.GetIndependentBlocks(_db, doc);
                _db.VersionedDocuments.Remove(doc);
                _db.VersionedDocumentBlocks.RemoveRange(blocksToRemove);

                _db.SaveChanges();
                _logger.Debug("Удален ЛИ с идентификатором {0}", id);
                _logger.Info("Удаление ЛИ: id - '{0}', sourceId - '{1}', targetId - '{2}'", id, wp.SourceId, wp.TargetId);
            }
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task CreateChangeList(int sourceId, int targetId)
        {
            var source = await _db.ModuleWorkingPrograms.FirstAsync(p=>p.VersionedDocumentId == sourceId);
            var documentType = source.VersionedDocument.Template.DocumentType;

            VersionedDocumentType changeListType;
            switch (documentType)
            {
                case VersionedDocumentType.ModuleWorkingProgram:
                    changeListType = VersionedDocumentType.ModuleChangeList;
                    break;
                default:
                    throw new NotImplementedException($"ЛИ для документов типа '{documentType}' не реализован.");
            }

            if (!_documentImplementationServices.TryGetValue(changeListType, out var service))
                throw new NotImplementedException($"Сервис для работы с документами типа '{changeListType}' не реализован.");

            //_documentService.ResaveDocument(_db.VersionedDocuments.Find(sourceId));
            //_documentService.ResaveDocument(_db.VersionedDocuments.Find(targetId));
            //_db.SaveChanges();

            var changeListService = (IChangeListService) service;
            var doc = changeListService.CreateDocument(sourceId, targetId);

            _db.SaveChanges();

            _logger.Info("Создание ЛИ: id - '{0}', sourceId - '{1}', targetId - '{2}'", doc.Id, sourceId, targetId);
        }

        private VersionedDocumentType GetDocumentType(DocumentKind documentKind, string standard)
        {
            try
            {
                VersionedDocumentType documentType;
                switch (standard)
                {
                    case StandardNames.FgosVo:
                        {
                            switch (documentKind)
                            {
                                case DocumentKind.Discipline:
                                    documentType = VersionedDocumentType.DisciplineWorkingProgram;
                                    break;
                                case DocumentKind.Module:
                                    documentType = VersionedDocumentType.ModuleWorkingProgram;
                                    break;
                                case DocumentKind.Gia:
                                    documentType = VersionedDocumentType.GiaWorkingProgram;
                                    break;
                                case DocumentKind.Practics:
                                    documentType = VersionedDocumentType.PracticesWorkingProgram;
                                    break;
                                case DocumentKind.Ohop:
                                    documentType = VersionedDocumentType.BasicCharacteristicOP;
                                    break;
                                case DocumentKind.CompetencePassport:
                                    documentType = VersionedDocumentType.CompetencePassport;
                                    break;
                                case DocumentKind.ModuleAnnotation:
                                    documentType = VersionedDocumentType.ModuleAnnotation;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(documentKind), documentKind, null);
                            }
                            break;
                        }
                    case StandardNames.FgosVo3PlusPlus:
                        {
                            switch (documentKind)
                            {
                                case DocumentKind.Ohop:
                                    documentType = VersionedDocumentType.BasicCharacteristicOP;
                                    break;
                                case DocumentKind.CompetencePassport:
                                    documentType = VersionedDocumentType.CompetencePassport;
                                    break;
                                case DocumentKind.ModuleAnnotation:
                                    documentType = VersionedDocumentType.ModuleAnnotation;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(documentKind), documentKind, null);
                            }
                            break;
                        }
                    case StandardNames.Suos:
                        {
                            switch (documentKind)
                            {
                                case DocumentKind.Ohop:
                                    documentType = VersionedDocumentType.BasicCharacteristicOP;
                                    break;
                                case DocumentKind.CompetencePassport:
                                    documentType = VersionedDocumentType.CompetencePassport;
                                    break;
                                case DocumentKind.ModuleAnnotation:
                                    documentType = VersionedDocumentType.ModuleAnnotation;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(documentKind), documentKind, null);
                            }
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(standard), standard, null);
                }

                _logger.Debug("Получен тип документа '{2}' по параметрам: documentKind='{0}', standard='{1}'",
                    documentKind, standard, documentType);

                return documentType;
            }
            catch (Exception ex)
            {
                _logger.Debug("Не удалось получить тип документа по заданным параметрам: documentKind='{0}', standard='{1}'", documentKind, standard);
                throw;
            }
        }

        public async Task<ActionResult> GetWorkingProgramResponsiblePersons(string disciplineId, string moduleId)
        {
            var roleName = ItsRoles.WorkingProgramView;
            var roleId = _db.Roles.First(r => r.Name == roleName).Id;
            var users = await _db.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId)).ToListAsync();

            var responsiblePersons =
                _db.WorkingProgramResponsiblePersons.Where(
                    d => d.DisciplineId == disciplineId && d.ModuleId == moduleId).ToList();

            var result = users.Select(u => new
            {
                IsSelected = responsiblePersons.Any(p=>p.UserId == u.Id),
                u.Id,
                u.LastName,
                u.FirstName,
                u.Patronymic,
                Fio = u.LastName + " " + u.FirstName + " " + u.Patronymic
            }).OrderBy(u=>u.Fio).ToList();

            return Json(result, new JsonSerializerSettings());
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task SetDisciplineWorkingProgramResponsiblePersons(string disciplineId, string moduleId, string[] userIds)
        {
            //var discipline = await _db.Disciplines.FirstOrDefaultAsync(d=>d.uid == disciplineId);
            var moduleResponsiblePersons = await _db.WorkingProgramResponsiblePersons.Where(p=>p.ModuleId == moduleId && p.DisciplineId == disciplineId).ToListAsync();
            foreach (var p in moduleResponsiblePersons)
                _db.WorkingProgramResponsiblePersons.Remove(p);

            foreach (var userId in userIds)
            {
                _db.WorkingProgramResponsiblePersons.Add(new WorkingProgramResponsiblePerson
                {
                    ModuleId = moduleId,
                    UserId = userId,
                    DisciplineId = disciplineId
                });
            }

            _db.SaveChanges();
        }
    }
}