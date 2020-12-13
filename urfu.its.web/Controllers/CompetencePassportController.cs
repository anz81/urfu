using Ext.Utilities;
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
using Urfu.Its.Web.Model.Models.CompetencePassportModels;
using Urfu.Its.Web.Model.Models.SharedDocumentModels;
using Ext.Utilities.Linq;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.WorkingProgramView)]
    public class CompetencePassportController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly IObjectLogger<WorkingProgramsController> _logger;
        private readonly IVersionedDocumentService _documentService;

        private readonly List<int> _statuses = new List<int>()
        {
            1, //"Формируется",
            11, //"В обработке",
            9, //"Не подписан",
            10, //"Подписан"
        };

        public CompetencePassportController(IVersionedDocumentService documentService, IObjectLogger<WorkingProgramsController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }
        
        public ActionResult Index(string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var passports = GetFilteredCompetencePassport(filter);

                return JsonNet(
                    new
                    {
                        data = passports
                    }
                );
            }

            var directionsForUser = db.DirectionsForUser(User, considerDivisions: true).Where(d => d.standard == "СУОС" || d.standard == "ФГОС ВО 3++")
                .Select(d => d.uid).ToList();

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
                            }).OrderBy(o => o.VersionYear)
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

            ViewBag.DocumentKind = (int)DocumentKind.CompetencePassport;

            ViewBag.CanEdit = User.IsInRole(ItsRoles.WorkingProgramManager) || User.IsInRole(ItsRoles.Admin);

            ViewBag.CanApprove = User.IsInRole(ItsRoles.ApproveOhopRpdRpm);

            return View();
        }
        
        [Authorize(Roles = ItsRoles.ApproveOhopRpdRpm)]
        public ActionResult UpdateVersionStatus(int id, int status, string comment)
        {
            var passport = db.CompetencePassports.FirstOrDefault(b => b.VersionedDocumentId == id);
            if (passport == null)
                return Json(new { success = false, message = "Редактируемый документ не найден" });//, "text/html", Encoding.Unicode);

            var statusUpop = db.UpopStatuses.FirstOrDefault(s => s.Id == status);
            if (statusUpop == null)
                return Json(new { success = false, message = "Статус не найден" });//, "text/html", Encoding.Unicode);
            
            passport.Status = statusUpop;
            passport.StatusChangeTime = DateTime.Now;
            passport.Comment = comment;
            
            if (!statusUpop.CanEdit()) // Подписан или В обработке
            {
                passport = SaveDocx(passport);
            }

            db.Entry(passport).State = EntityState.Modified;
            db.SaveChanges();
            
            _logger.Info($"Изменен статус документа Пасспорт компетенций Id = {passport.VersionedDocumentId} Status = {passport.Status.Id} {passport.Status.Name} Год {passport.Year}" +
               $"Profile = {passport.BasicCharacteristicOP.Info.ProfileId} {passport.BasicCharacteristicOP.Info.Profile?.CODE} {passport.BasicCharacteristicOP.Info.Profile?.NAME} " +
               $"Версия ОХОП {passport.BasicCharacteristicOP.Version} {passport.BasicCharacteristicOP.Info.Year} год");

            return Json(new { success = true });//, "text/html", Encoding.Unicode);
        }

        public ActionResult SendVersion(int id)
        {
            var passport = db.CompetencePassports.Include(b => b.Status).FirstOrDefault(b => b.VersionedDocumentId == id);
            if (passport == null)
                return Json(new { success = false, message = "Редактируемый документ не найден" });//, "text/html", Encoding.Unicode);

            if (!passport.Status.CanEdit())
                return Json(new { success = false, message = "Документ уже подписан или находится в обработке" });//, "text/html", Encoding.Unicode);

            var statusUpop = db.UpopStatuses.FirstOrDefault(s => s.Id == 11 /*"В обработке"*/);
            if (statusUpop == null)
                return Json(new { success = false, message = "Статус не найден" });//, "text/html", Encoding.Unicode);

            passport = SaveDocx(passport);

            passport.Status = statusUpop;
            passport.StatusChangeTime = DateTime.Now;
            db.Entry(passport).State = EntityState.Modified;
            db.SaveChanges();

            _logger.Info($"Изменен статус документа Пасспорт компетенций (Отправлен на согласование) Id = {passport.VersionedDocumentId} Status = {passport.Status.Id} {passport.Status.Name} Год {passport.Year}" +
                $"Profile = {passport.BasicCharacteristicOP.Info.ProfileId} {passport.BasicCharacteristicOP.Info.Profile?.CODE} {passport.BasicCharacteristicOP.Info.Profile?.NAME} " +
                $"Версия ОХОП {passport.BasicCharacteristicOP.Version} {passport.BasicCharacteristicOP.Info.Year} год");

            return Json(new { success = true, status = statusUpop.Name, statusId = statusUpop.Id,
                statusDate = $"{passport.StatusChangeTime.ToShortDateString()} {passport.StatusChangeTime.ToShortTimeString()}" });//, "text/html", Encoding.Unicode);
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task Remove(int id)
        {
            var passport = await db.CompetencePassports.FirstOrDefaultAsync(p => p.VersionedDocumentId == id);
            if (passport == null)
                throw new InvalidOperationException($"Не найдена версия паспорта компетенций с идентификатором {id}.");
            
            var hasInheritedDocuments = db.CompetencePassports.Any(p => p.BasedOnId == id);
            if (hasInheritedDocuments)
                throw new InvalidOperationException("На основе выбранной версии паспорта компетенций созданы другие документы. Удаление невозможно.");

            // TODO 21.03.2018 статусов еще нет. Нужна проверка на возможность удаления в зависимости от статусов
            //if (wp.Status != ModuleWorkingProgramStatus.Draft)
            //    throw new InvalidOperationException("Невозможно удалить. Статус не позволяет.");

            var documentType = passport.VersionedDocument.Template.DocumentType;
            string title = passport.BasicCharacteristicOP.Info.Profile.NAME;
            int version = passport.Version;

            _logger.Debug("Попытка удаления паспорта компетенций с идентификатором {0}", id);
            var doc = passport.VersionedDocument;
            db.CompetencePassports.Remove(passport);
            var blocksToRemove = BlockDataHelper.GetIndependentBlocks(db, doc);
            db.VersionedDocuments.Remove(doc);
            db.VersionedDocumentBlocks.RemoveRange(blocksToRemove);

            db.SaveChanges();
            _logger.Debug("Удален паспорт компетенций с идентификатором {0}", id);

            _logger.Info("Удаление версии {0}, '{1}', id - '{2}', версия - '{3}'",
                EnumHelper<VersionedDocumentType>.GetDisplayValue(documentType), title, id, version);
        }

        public ActionResult DirectionModules(string id)
        {
            var modules = db.Directions.FirstOrDefault(d => d.uid == id).Modules.Where(m => m.state != "Формируется").ToList()
                .Select(m => new ModuleInfoWithDisciplines(m))
                .OrderBy(m => m.Name);

            return Json(modules, new JsonSerializerSettings());
        }

        public ActionResult AllEduResults(string ids)
        {
            var competenceIds = JsonConvert.DeserializeObject<List<int>>(ids);
            
            var eduResults = db.EduResults2.Where(r => competenceIds.Contains(r.CompetenceId)).ToList()
                .GroupBy(r => r.Competence).Select(r => new
                    {
                        CompetenceId = r.Key.Id,
                        EduResults = r.OrderBy(er => er.EduResultKindId).ThenBy(er => er.EduResultTypeId).ThenBy(er => er.SerialNumber)
                                        .Select(e => new EduResultsInfo(e))
                    });

            return Json(eduResults, new JsonSerializerSettings());
        }

        private IQueryable<CompetencePassportViewModel> GetFilteredCompetencePassport(string filter)
        {
            Expression<Func<CompetencePassport, CompetencePassportViewModel>> select = p => new CompetencePassportViewModel
            {
                version = p.Version,
                directionId = p.BasicCharacteristicOP.Info.Profile.DIRECTION_ID,
                directionOkso = p.BasicCharacteristicOP.Info.Profile.Direction.okso,
                directionTitle = p.BasicCharacteristicOP.Info.Profile.Direction.title,
                qualification = p.BasicCharacteristicOP.Info.Profile.QUALIFICATION,
                profile = p.BasicCharacteristicOP.Info.Profile.CODE,
                profileTitle = p.BasicCharacteristicOP.Info.Profile.NAME,
                profileId = p.BasicCharacteristicOP.Info.ProfileId,
                year = p.Year,
                status = p.Status == null ? "" : p.Status.Name,
                statusId = p.Status.Id,
                statusDateTime = p.StatusChangeTime,
                standard = p.BasicCharacteristicOP.Info.Profile.Direction.standard,
                versionedDocumentId = p.VersionedDocumentId,
                chairTitle = p.BasicCharacteristicOP.Info.Profile.Division.title,
                chairId = p.BasicCharacteristicOP.Info.Profile.Division.uuid,
                divisionId = p.BasicCharacteristicOP.Info.Profile.Division.parent,
                comment = p.Comment,
                versionOhopId = p.BasicCharacteristicOPId
            };

            var filterRules = FilterRules.Deserialize(filter);

            var filterId = filterRules?.Find(f => f.Property == "ohopId");

            int id;
            var onlyOhop = int.TryParse(filterId?.Value, out id);
            if (onlyOhop)
            {
                var passport = db.CompetencePassports.Where(p => p.BasicCharacteristicOPId == id).Select(select)
                    .OrderBy(p => p.version);
                return passport;
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

            var passports = db.CompetencePassports.Where(p => userDirections.Contains(p.BasicCharacteristicOP.Info.Profile.DIRECTION_ID)
                                && (!hasDivision || hasDivision && profiles.Contains(p.BasicCharacteristicOP.Info.ProfileId)) && p.Status != null)
                                .Select(select).Where(filterRules).OrderBy(o => o.directionOkso).ThenBy(o => o.profile).ThenBy(o => o.profileTitle).ThenBy(o => o.version);

            return passports;
        }

        private CompetencePassport SaveDocx(CompetencePassport passport)
        {
            var document = db.VersionedDocuments
                .Include(d => d.Template)
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .FirstOrDefault(d => d.Id == passport.VersionedDocumentId);

            var fileName = $"{passport.BasicCharacteristicOP.Info.Profile.CODE} {passport.BasicCharacteristicOP.Info.Year} версия ОХОП {passport.BasicCharacteristicOP.Version} " +
                $"пасспорт компетенций версия {passport.Version}";

            var docxStream = _documentService.Print(document, FileFormat.Docx);
            passport.FileStorageDocxId = Model.FileStorageHelper.SaveFile(docxStream, $"{fileName}.docx", Model.FileCategory.CompetencePassport, folder: $"{passport.BasicCharacteristicOP.Info.Year}",
                comment: $"Пасспорт компетенций {passport.BasicCharacteristicOP.Info.Profile.OksoAndTitle} {passport.Year} версия {passport.Version} " +
                    $"(Версия ОХОП {passport.BasicCharacteristicOP.Version} {passport.BasicCharacteristicOP.Info.Year} год)", id: passport.FileStorageDocxId);
            
            return passport;
        }
    }
}