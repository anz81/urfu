//using Ext.Utilities;
//using Ext.Utilities.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TemplateEngine;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using System.Linq.Expressions;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Ext.Utilities;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.WorkingProgramView)]
    public class ModuleAnnotationsController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly IObjectLogger<WorkingProgramsController> _logger;
        private readonly IVersionedDocumentService _documentService;

        private string giaType = "Итоговая государственная аттестация";
        private string practiceType = "Учебная и производственная практики";

        private readonly List<int> _statuses = new List<int>()
        {
            1, //"Формируется",
            11, //"В обработке",
            9, //"Не подписан",
            10, //"Подписан"
        };

        public ModuleAnnotationsController(IVersionedDocumentService documentService, IObjectLogger<WorkingProgramsController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }
        
        public ActionResult Index(string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var annotations = GetFilteredModuleAnnotations(filter);

                return JsonNet(
                    new
                    {
                        data = annotations
                    }
                );
            }

            var directionsForUser = db.DirectionsForUser(User, considerDivisions: true).Where(d => d.standard == "СУОС" || d.standard == "ФГОС ВО 3++")
                .Select(d => d.uid).ToList();

            var plans = db.Plans.Where(p => !p.remove).Select(p => new { p.learnProgramUUID, p.eduplanNumber, p.versionNumber }).Distinct().ToList();

            var divisions = db.BasicCharacteristicOPInfos.Where(i => directionsForUser.Contains(i.Profile.DIRECTION_ID) && i.BasicCharacteristicOPs.Any(o => o.UpopStatusId == 10 /*Подписан*/))
                .ToList()
                .GroupBy(i => db.GetInstituteForChair(i.Profile.CHAIR_ID))
                .Select(g => new
                {
                    DivisionId = g.Key.uuid,
                    DivisionName = $"{g.Key.shortTitle} ({g.Key.title})",

                    Directions = g.GroupBy(d => d.Profile.Direction).Select(d => new
                    {
                        DirectionId = d.Key.uid,
                        DirectionName = d.Key.OksoAndTitleStandard,

                        Profiles = d.Select(i => new
                        {
                            ProfileId = i.ProfileId,
                            ProfileName = i.Profile.OksoAndTitle,

                            Ohops = i.BasicCharacteristicOPs.Where(b => b.Status.IsSigned()).Select(o => new
                            {
                                OhopId = o.VersionedDocumentId,
                                VersionYear = $"{i.Year}, {o.Version} версия"
                            }).OrderBy(o => o.VersionYear),
                            
                            Plans = plans.Where(p => p.learnProgramUUID == i.ProfileId)
                            .Select(p => new
                            {
                                PlanNumber = p.eduplanNumber,
                                PlanVersionNumber = p.versionNumber,
                                NumberAndVersion = p.eduplanNumber + " (" + p.versionNumber + ")"
                            })//.Distinct()
                            .OrderBy(p => p.PlanNumber).ThenBy(p => p.PlanVersionNumber)//.ToList()
                            //.OrderBy(p => p.eduplanNumber).ThenBy(p => p.versionNumber).ToList()

                        }).OrderBy(p => p.ProfileName)
                    }).OrderBy(d => d.DirectionName)
                }).OrderBy(d => d.DivisionName);

            ViewBag.Divisions = JsonConvert.SerializeObject(divisions);

            var statuses = new List<UPOPStatus>();
            foreach (var _s in _statuses)
            {
                statuses.Add(db.UpopStatuses.FirstOrDefault(s => _s == s.Id));
            }
            ViewBag.Statuses = JsonConvert.SerializeObject(statuses);

            ViewBag.DocumentKind = (int)DocumentKind.ModuleAnnotation;

            ViewBag.CanEdit = User.IsInRole(ItsRoles.WorkingProgramManager) || User.IsInRole(ItsRoles.Admin);

            ViewBag.CanApprove = User.IsInRole(ItsRoles.ApproveOhopRpdRpm);

            return View();
        }
        
        public ActionResult CreateOrEdit(int ohopId, int planNumber, int planVersionNumber)
        {
            var annotation = db.ModuleAnnotations.FirstOrDefault(a => a.BasicCharacteristicOPId == ohopId && a.PlanNumber == planNumber && a.PlanVersionNumber == planVersionNumber);
            var ohop = db.BasicCharacteristicOPs.FirstOrDefault(b => b.VersionedDocumentId == ohopId);
            return Json(new
            {
                create = annotation == null,
                documentId = annotation?.VersionedDocumentId,
                standard = ohop?.Info?.Profile?.Direction?.standard
            }, new JsonSerializerSettings());
        }

        [Authorize(Roles = ItsRoles.ApproveOhopRpdRpm)]
        public ActionResult UpdateVersionStatus(int id, int status, string comment)
        {
            var annotation = db.ModuleAnnotations.FirstOrDefault(b => b.VersionedDocumentId == id);
            if (annotation == null)
                return Json(new { success = false, message = "Редактируемый документ не найден" });//, "text/html", Encoding.Unicode);

            var statusUpop = db.UpopStatuses.FirstOrDefault(s => s.Id == status);
            if (statusUpop == null)
                return Json(new { success = false, message = "Статус не найден" });//, "text/html", Encoding.Unicode);
            
            annotation.Status = statusUpop;
            annotation.StatusChangeTime = DateTime.Now;
            annotation.Comment = comment;
            
            if (!statusUpop.CanEdit()) // Подписан или В обработке
            {
                annotation = SaveDocx(annotation);
            }

            db.Entry(annotation).State = EntityState.Modified;
            db.SaveChanges();
            
            _logger.Info($"Изменен статус документа Аннотация модулей Id = {annotation.VersionedDocumentId} Status = {annotation.Status.Id} {annotation.Status.Name} Номер плана {annotation.PlanNumber} Версия плана {annotation.PlanVersionNumber}" +
               $"Profile = {annotation.BasicCharacteristicOP.Info.ProfileId} {annotation.BasicCharacteristicOP.Info.Profile?.CODE} {annotation.BasicCharacteristicOP.Info.Profile?.NAME} " +
               $"Версия ОХОП {annotation.BasicCharacteristicOP.Version} {annotation.BasicCharacteristicOP.Info.Year} год");

            return Json(new { success = true });//, "text/html", Encoding.Unicode);
        }

        public ActionResult SendVersion(int id)
        {
            var annotation = db.ModuleAnnotations.Include(b => b.Status).FirstOrDefault(b => b.VersionedDocumentId == id);
            if (annotation == null)
                return Json(new { success = false, message = "Редактируемый документ не найден" });//, "text/html", Encoding.Unicode);

            if (!annotation.Status.CanEdit())
                return Json(new { success = false, message = "Документ уже подписан или находится в обработке" });//, "text/html", Encoding.Unicode);

            var statusUpop = db.UpopStatuses.FirstOrDefault(s => s.Id == 11 /*"В обработке"*/);
            if (statusUpop == null)
                return Json(new { success = false, message = "Статус не найден" });//, "text/html", Encoding.Unicode);

            annotation = SaveDocx(annotation);

            annotation.Status = statusUpop;
            annotation.StatusChangeTime = DateTime.Now;
            db.Entry(annotation).State = EntityState.Modified;
            db.SaveChanges();

            _logger.Info($"Изменен статус документа Аннотация модулей (Отправлен на согласование) Id = {annotation.VersionedDocumentId} Status = {annotation.Status.Id} {annotation.Status.Name} Номер плана {annotation.PlanNumber} Версия плана {annotation.PlanVersionNumber}" +
               $"Profile = {annotation.BasicCharacteristicOP.Info.ProfileId} {annotation.BasicCharacteristicOP.Info.Profile?.CODE} {annotation.BasicCharacteristicOP.Info.Profile?.NAME} " +
               $"Версия ОХОП {annotation.BasicCharacteristicOP.Version} {annotation.BasicCharacteristicOP.Info.Year} год");

            return Json(new { success = true, status = statusUpop.Name, statusId = statusUpop.Id,
                statusDate = $"{annotation.StatusChangeTime.ToShortDateString()} {annotation.StatusChangeTime.ToShortTimeString()}" });//, "text/html", Encoding.Unicode);
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task Remove(int id)
        {
            var annotation = await db.ModuleAnnotations.FirstOrDefaultAsync(p => p.VersionedDocumentId == id);
            if (annotation == null)
                throw new InvalidOperationException($"Не найдена версия аннотации модулей с идентификатором {id}.");
            
            // TODO 21.03.2018 статусов еще нет. Нужна проверка на возможность удаления в зависимости от статусов
            //if (wp.Status != ModuleWorkingProgramStatus.Draft)
            //    throw new InvalidOperationException("Невозможно удалить. Статус не позволяет.");

            var documentType = annotation.VersionedDocument.Template.DocumentType;
            string title = annotation.BasicCharacteristicOP.Info.Profile.NAME;

            _logger.Debug("Попытка удаления аннотации модулей с идентификатором {0}", id);
            var doc = annotation.VersionedDocument;
            db.ModuleAnnotations.Remove(annotation);
            var blocksToRemove = BlockDataHelper.GetIndependentBlocks(db, doc);
            db.VersionedDocuments.Remove(doc);
            db.VersionedDocumentBlocks.RemoveRange(blocksToRemove);

            db.SaveChanges();
            _logger.Debug("Удален документ Аннотация модулей с идентификатором {0}", id);
        }

        public ActionResult RequiredModules(string profile, int planNumber, int planVersionNumber)
        {
            var modules = Plans(profile, planNumber, planVersionNumber)
                .Where(p => p.moduleGroupType == "Обязательная часть" && p.Module.type != giaType && p.Module.type != practiceType)
                .GroupBy(p => p.Module).Select(p => p.Key).ToList()
                .Select(m => new ModuleAnnotationRow(m))
                .OrderBy(m => m.Name).ToList();

            return Json(modules, new JsonSerializerSettings());
        }

        public ActionResult Modules(string profile, int planNumber, int planVersionNumber)
        {
            var modules = Plans(profile, planNumber, planVersionNumber)
                .Where(p => p.moduleGroupType == "Формируемая участниками образовательных отношений" && p.Module.type != giaType && p.Module.type != practiceType)
                .GroupBy(p => p.Module).Select(p => p.Key).ToList()
                .Select(m => new ModuleAnnotationRow(m))
                .OrderBy(m => m.Name).ToList();

            return Json(modules, new JsonSerializerSettings());
        }

        public ActionResult PracticeModules(string profile, int planNumber, int planVersionNumber)
        {
            var modules = Plans(profile, planNumber, planVersionNumber)
                .Where(p => p.Module.type == practiceType)
                .GroupBy(p => p.Module).Select(p => p.Key).ToList()
                .Select(m => new ModuleAnnotationRow(m))
                .OrderBy(m => m.Name).ToList();

            return Json(modules, new JsonSerializerSettings());
        }

        public ActionResult GiaModules(string profile, int planNumber, int planVersionNumber)
        {
            var modules = Plans(profile, planNumber, planVersionNumber)
                .Where(p => p.Module.type == giaType)
                .GroupBy(p => p.Module).Select(p => p.Key).ToList()
                .Select(m => new ModuleAnnotationRow(m))
                .OrderBy(m => m.Name).ToList();

            return Json(modules, new JsonSerializerSettings());
        }
        
        private IQueryable<Plan> Plans(string profile, int planNumber, int planVersionNumber)
        {
            return db.Plans.Where(p => p.learnProgramUUID == profile && p.eduplanNumber == planNumber && p.versionNumber == planVersionNumber && !p.remove);
        }

        private IQueryable<ModuleAnnotationViewModel> GetFilteredModuleAnnotations(string filter)
        {
            Expression<Func<ModuleAnnotation, ModuleAnnotationViewModel>> select = annotation => new ModuleAnnotationViewModel()
            {
                directionId = annotation.BasicCharacteristicOP.Info.Profile.DIRECTION_ID,
                directionOkso = annotation.BasicCharacteristicOP.Info.Profile.Direction.okso,
                directionTitle = annotation.BasicCharacteristicOP.Info.Profile.Direction.title,
                qualification = annotation.BasicCharacteristicOP.Info.Profile.QUALIFICATION,
                profile = annotation.BasicCharacteristicOP.Info.Profile.CODE,
                profileTitle = annotation.BasicCharacteristicOP.Info.Profile.NAME,
                profileId = annotation.BasicCharacteristicOP.Info.ProfileId,
                year = annotation.BasicCharacteristicOP.Info.Year,
                status = annotation.Status == null ? "" : annotation.Status.Name,
                statusId = annotation.Status.Id,
                statusDateTime = annotation.StatusChangeTime,
                standard = annotation.BasicCharacteristicOP.Info.Profile.Direction.standard,
                versionedDocumentId = annotation.VersionedDocumentId,
                chairTitle = annotation.BasicCharacteristicOP.Info.Profile.Division.title,
                chairId = annotation.BasicCharacteristicOP.Info.Profile.Division.uuid,
                divisionId = annotation.BasicCharacteristicOP.Info.Profile.Division.parent,
                comment = annotation.Comment,
                planNumber = annotation.PlanNumber,
                planVersionNumber = annotation.PlanVersionNumber,
                basicCharacteristicOPId = annotation.BasicCharacteristicOPId,
                basicCharacteristicOPVersion = annotation.BasicCharacteristicOP.Version
        };

            var filterRules = FilterRules.Deserialize(filter);

            var filterId = filterRules?.Find(f => f.Property == "ohopId");

            int id;
            var onlyOhop = int.TryParse(filterId?.Value, out id);
            if (onlyOhop)
            {
                var annotation = db.ModuleAnnotations.Where(a => a.BasicCharacteristicOPId == id).Select(select);
                return annotation;
            }

            var filterDivision = filterRules?.Find(f => f.Property == "divisionId");
            if (filterRules != null)
            {
                filterRules.Remove(filterDivision);
                filterRules.Remove(filterId);
            }

            var hasDivision = filterDivision?.Value != null;
            var profiles = new List<string>();
            if (hasDivision)
            {
                profiles = db.ProfilesForDivision(filterDivision?.Value).Select(p => p.ID).ToList();
            }

            var userDirections = db.DirectionsForUser(User, considerDivisions: true).Select(d => d.uid).ToList();

            var annotations = db.ModuleAnnotations.Where(m => userDirections.Contains(m.BasicCharacteristicOP.Info.Profile.DIRECTION_ID)
                                && (!hasDivision || hasDivision && profiles.Contains(m.BasicCharacteristicOP.Info.ProfileId)) && m.Status != null)
                                .Select(select).Where(filterRules).OrderBy(o => o.directionOkso).ThenBy(o => o.profile).ThenBy(o => o.profileTitle);

            return annotations;
        }

        private ModuleAnnotation SaveDocx(ModuleAnnotation annotation)
        {
            var document = db.VersionedDocuments
                .Include(d => d.Template)
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .FirstOrDefault(d => d.Id == annotation.VersionedDocumentId);

            var fileName = $"Аннотация модулей. {annotation.BasicCharacteristicOP.Info.Profile.CODE} {annotation.BasicCharacteristicOP.Info.Year} версия ОХОП {annotation.BasicCharacteristicOP.Version} " +
                $"номер плана: {annotation.PlanNumber} версия плана: {annotation.PlanVersionNumber}";

            var docxStream = _documentService.Print(document, FileFormat.Docx);
            annotation.FileStorageDocxId = Model.FileStorageHelper.SaveFile(docxStream, $"{fileName}.docx", Model.FileCategory.ModuleAnnotation, folder: $"{annotation.BasicCharacteristicOP.Info.Year}",
                comment: $"Аннотация модулей {annotation.BasicCharacteristicOP.Info.Profile.OksoAndTitle} номер плана: {annotation.PlanNumber} версия плана: {annotation.PlanVersionNumber} " +
                    $"(Версия ОХОП {annotation.BasicCharacteristicOP.Version} {annotation.BasicCharacteristicOP.Info.Year} год)", id: annotation.FileStorageDocxId);
            
            return annotation;
        }
    }
}