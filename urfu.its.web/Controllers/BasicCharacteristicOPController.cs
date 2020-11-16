using Ext.Utilities;
using Ext.Utilities.Linq;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TemplateEngine;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Model.Models.OHOPModels;
using Urfu.Its.Web.Models;
using Urfu.Its.Web.Excel;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using System.Linq.Expressions;
using FileCategory = Urfu.Its.Web.Model.FileCategory;
using FileStorageHelper = Urfu.Its.Web.Model.FileStorageHelper;
using Microsoft.Extensions.Hosting.Internal;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.WorkingProgramView)]
    public class BasicCharacteristicOPController : BaseController
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

        public BasicCharacteristicOPController(IVersionedDocumentService documentService, IObjectLogger<WorkingProgramsController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        public ActionResult Index(string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var filterRules = FilterRules.Deserialize(filter);
                var filterDivision = filterRules?.Find(f => f.Property == "divisionId");
                if (filterRules != null)
                {
                    filterRules.Remove(filterDivision);
                }

                var hasDivision = filterDivision?.Value != null;

                var userDirections = db.DirectionsForUser(User, considerDivisions: true).Select(d => d.uid).ToList();

                var infoIds = new List<int>();
                if (hasDivision)
                    infoIds = db.BasicCharacteristicOPInfos.ToList().Where(b => db.GetInstituteForChair(b.Profile.CHAIR_ID).uuid == filterDivision?.Value).Select(b => b.Id).ToList();
                
                var ohops = db.BasicCharacteristicOPInfos.Where(b => userDirections.Contains(b.Profile.DIRECTION_ID)
                                    && (!hasDivision || hasDivision && infoIds.Contains(b.Id)))
                    .Select(b => new
                    {
                        id = b.Id,
                        directionId = b.Profile.DIRECTION_ID,
                        directionOkso = b.Profile.Direction.okso,
                        directionTitle = b.Profile.Direction.title,
                        qualification = b.Profile.QUALIFICATION,
                        divisionTitle = b.Profile.Division.title,
                        profile = b.Profile.CODE,
                        profileTitle = b.Profile.NAME,
                        profileId = b.ProfileId,
                        year = b.Year,
                        standard = b.Profile.Direction.standard,
                        documentKind = DocumentKind.Ohop
                    }).AsQueryable().Where(filterRules).OrderBy(o => o.directionOkso).ThenBy(o => o.profile).ThenBy(o => o.profileTitle).ThenBy(o => o.year);

                return JsonNet(
                    new
                    {
                        data = ohops,
                        total = ohops.Count()
                    }
                );
            }

            var directionsForUser = db.DirectionsForUser(User, considerDivisions: true).Where(d => d.standard == "СУОС" || d.standard == "ФГОС ВО 3++").OrderBy(d => d.okso).ToList();

            var directions = directionsForUser
                .Select(d => new
                {
                    DirectionId = d.uid,
                    DirectionName = d.OksoAndTitleStandard,

                    Profiles = db.Profiles.Where(p => p.DIRECTION_ID == d.uid && !p.remove).ToList().Select(p => new
                    {
                        ProfileId = p.ID,
                        ProfileName = p.OksoAndTitle
                    })
                });

            ViewBag.Directions = JsonConvert.SerializeObject(directions);

            var divisions = directionsForUser
                .Select(d => new
                {
                    Profiles = db.Profiles.Where(p => p.DIRECTION_ID == d.uid && !p.remove).ToList().Select(p => new
                    {
                        p.Direction,
                        Division = db.GetInstituteForChair(p.CHAIR_ID),

                        ProfileId = p.ID,
                        ProfileName = p.OksoAndTitle
                    })
                })
                .SelectMany(p => p.Profiles)
                .Where(d => d.Division != null)
                .GroupBy(p => p.Division)
                .Select(p => new
                {
                    DivisionId = p.Key.uuid,
                    DivisionName = $"{p.Key.shortTitle} ({p.Key.title})",
                    
                    Directions = p.GroupBy(d => d.Direction).Select(d => new
                    {
                        DirectionId = d.Key.uid,
                        DirectionName = d.Key.OksoAndTitleStandard,
                    
                        Profiles = d.Select(pr => new
                        {
                            pr.ProfileId,
                            pr.ProfileName
                        })
                    })
                });
            
            ViewBag.Divisions = JsonConvert.SerializeObject(divisions);

            ViewBag.Focus = focus;
            ViewBag.CanEdit = User.IsInRole(ItsRoles.WorkingProgramManager);
            return View();
        }

        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public ActionResult Create(string profile, int year)
        {
            if (db.BasicCharacteristicOPInfos.Any(i => i.ProfileId == profile && i.Year == year))
                return Json(new { success = false, message = "На указанные ОП и год уже создан документ" });//, "text/html", Encoding.Unicode);

            var info = new BasicCharacteristicOPInfo()
            {
                ProfileId = profile,
                Year = year
            };
            db.BasicCharacteristicOPInfos.Add(info);
            db.SaveChanges();

            return Json(new { success = true, id = info.Id });//, "text/html", Encoding.Unicode);
        }

        public ActionResult Versions(string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var ohops = GetFilteredVersionOhop(filter);

                return JsonNet(
                    new
                    {
                        data = ohops
                    }
                );
            }

            var directionsForUser = db.DirectionsForUser(User, considerDivisions: true).Where(d => d.standard == "СУОС" || d.standard == "ФГОС ВО 3++").OrderBy(d => d.okso).ToList();

            var directions = directionsForUser
               .Select(d => new
               {
                   DirectionId = d.uid,
                   DirectionName = d.OksoAndTitleStandard,

                   Profiles = db.Profiles.Where(p => p.DIRECTION_ID == d.uid && !p.remove).ToList().Select(p => new
                   {
                       ProfileId = p.ID,
                       ProfileName = p.OksoAndTitle
                   })
               });

            ViewBag.Directions = JsonConvert.SerializeObject(directions);

            var divisions = directionsForUser
                .Select(d => new
                {
                    Profiles = db.Profiles.Where(p => p.DIRECTION_ID == d.uid && !p.remove).ToList().Select(p => new
                    {
                        p.Direction,
                        Division = db.GetInstituteForChair(p.CHAIR_ID),

                        ProfileId = p.ID,
                        ProfileName = p.OksoAndTitle
                    })
                })
                .SelectMany(p => p.Profiles)
                .Where(d => d.Division != null)
                .GroupBy(p => p.Division)
                .Select(p => new
                {
                    DivisionId = p.Key.uuid,
                    DivisionName = $"{p.Key.shortTitle} ({p.Key.title})",

                    Directions = p.GroupBy(d => d.Direction).Select(d => new
                    {
                        DirectionId = d.Key.uid,
                        DirectionName = d.Key.OksoAndTitleStandard,

                        Profiles = d.Select(pr => new
                        {
                            pr.ProfileId,
                            pr.ProfileName
                        })
                    })
                });

            ViewBag.Divisions = JsonConvert.SerializeObject(divisions);

            var statuses = new List<UPOPStatus>();
            foreach (var _s in _statuses)
            {
                statuses.Add(db.UpopStatuses.FirstOrDefault(s => _s == s.Id));
            }
            ViewBag.Statuses = JsonConvert.SerializeObject(statuses);

            ViewBag.CanEdit = User.IsInRole(ItsRoles.WorkingProgramManager) || User.IsInRole(ItsRoles.Admin);

            ViewBag.CanApprove = User.IsInRole(ItsRoles.ApproveOhopRpdRpm);

            return View();
        }

        private IOrderedQueryable<VersionOHOPViewModel> GetFilteredVersionOhop(string filter)
        {
            Expression<Func<BasicCharacteristicOP, VersionOHOPViewModel>> select = b => new VersionOHOPViewModel
            {
                version = b.Version,
                directionId = b.Info.Profile.DIRECTION_ID,
                directionOkso = b.Info.Profile.Direction.okso,
                directionTitle = b.Info.Profile.Direction.title,
                qualification = b.Info.Profile.QUALIFICATION,
                profile = b.Info.Profile.CODE,
                profileTitle = b.Info.Profile.NAME,
                profileId = b.Info.ProfileId,
                year = b.Info.Year,
                status = b.Status == null ? "" : b.Status.Name,
                statusId = b.Status.Id,
                statusDateTime = b.StatusChangeTime,
                standard = b.Info.Profile.Direction.standard,
                versionedDocumentId = b.VersionedDocumentId,
                chairTitle = b.Info.Profile.Division.title,
                chairId = b.Info.Profile.Division.uuid,
                divisionId = b.Info.Profile.Division.parent,
                comment = b.Comment
            };
            
            var filterRules = FilterRules.Deserialize(filter);

            var filterId = filterRules?.Find(f => f.Property == "id");
            int id;
            var onlyOne = int.TryParse(filterId?.Value, out id);

            if (onlyOne)
            {
                var ohop = db.BasicCharacteristicOPs.Where(b => b.VersionedDocumentId == id).Select(select)
                    .OrderBy(b => b.directionOkso);
                return ohop;
            }

            var filterDivision = filterRules?.Find(f => f.Property == "divisionId");
            if (filterRules != null)
            {
                filterRules.Remove(filterDivision);
                filterRules.Remove(filterId);
            }

            var hasDivision = filterDivision?.Value != null;

            var infoIds = new List<int>();
            var profiles = new List<string>();
            if (hasDivision)
            {
                profiles = db.ProfilesForDivision(filterDivision?.Value).Select(p => p.ID).ToList();
            }

            var userDirections = db.DirectionsForUser(User, considerDivisions: true).Select(d => d.uid).ToList();

            var ohops = db.BasicCharacteristicOPs.Where(b => userDirections.Contains(b.Info.Profile.DIRECTION_ID)
                                && (!hasDivision || hasDivision && profiles.Contains(b.Info.ProfileId) /* infoIds.Contains(b.InfoId)*/) && b.Status != null)
                        .Select(select)
                        .Where(filterRules).OrderBy(o => o.directionOkso).ThenBy(o => o.profile).ThenBy(o => o.profileTitle).ThenBy(o => o.version);

            return ohops;
        }

        public ActionResult Edit(int id, int year)
        {
            var canEdit = db.BasicCharacteristicOPs.Include(b => b.Status).Where(b => b.InfoId == id).ToList().All(b => b.Status.CanEdit());
            if (!canEdit)
            {
                return Json(new
                {
                    success = false,
                    message = "Редактирование запрещено. Существуют версии в статусе Подписан или В обработке"
                });
            }

            var info = db.BasicCharacteristicOPInfos.FirstOrDefault(b => b.Id == id);
            if (info == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Редактируемый ОХОП не найден"
                });
            }

            if (db.BasicCharacteristicOPInfos.Any(b => b.Id != id && b.ProfileId == info.ProfileId && b.Year == year))
            {
                return Json(new
                {
                    success = false,
                    message = "ОХОП с такими параметрами уже существует"
                });
            }

            info.Year = year;
            db.Entry(info).State = EntityState.Modified;
            db.SaveChanges();

            // изменить блоки в документах охоп
            var defaultValues = new BasicCharacteristicOPDefaultValues(info);
            var blocks = info.BasicCharacteristicOPs.SelectMany(o =>
                o.VersionedDocument.BlockLinks.Where(l => l.DocumentBlock.Name == nameof(BasicCharacteristicOPSchemaModel.EduProgramInfo))
                    .Select(l => l.DocumentBlock));
            
            foreach (var block in blocks)
            {
                block.Data = BlockDataHelper.PrepareData(defaultValues.EduProgramInfo());
                block.CreatedAt = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new
            {
                success = true
            });
        }

        [ErrorFilter]
        [Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task RemoveModuleWorkingProgram(int id)
        {
            var wp = await db.BasicCharacteristicOPs.FirstOrDefaultAsync(w => w.VersionedDocumentId == id);
            if (wp == null)
                throw new InvalidOperationException($"Не найдена версия ОХОП с идентификатором {id}.");

            if (wp.ModuleWorkingPrograms.Any())
                throw new InvalidOperationException("С версией ОХОП связаны документы РПМ. Удаление невозможно.");

            if (wp.CompetencePassports.Any())
                throw new InvalidOperationException("С версией ОХОП связаны паспорта компетенций. Удаление невозможно.");


            var infoId = wp.InfoId;
            var hasInheritedDocuments = db.BasicCharacteristicOPs.Where(w => w.InfoId == infoId).Any(w => w.BasedOnId == id);
            if (hasInheritedDocuments)
                throw new InvalidOperationException("На выбранной версии ОХОП созданы на основе другие документы. Удаление невозможно.");

            // TODO 21.03.2018 статусов еще нет. Нужна проверка на возможность удаления в зависимости от статусов РП
            //if (wp.Status != ModuleWorkingProgramStatus.Draft)
            //    throw new InvalidOperationException("Невозможно удалить. Статус не позволяет.");

            var documentType = wp.VersionedDocument.Template.DocumentType;
            string title = wp.Info.Profile.NAME;
            int version = wp.Version;

            _logger.Debug("Попытка удаления ОХОП с идентификатором {0}", id);
            var doc = wp.VersionedDocument;
            db.BasicCharacteristicOPs.Remove(wp);
            var blocksToRemove = BlockDataHelper.GetIndependentBlocks(db, doc);
            db.VersionedDocuments.Remove(doc);
            db.VersionedDocumentBlocks.RemoveRange(blocksToRemove);

            db.SaveChanges();
            _logger.Debug("Удален ОХОП с идентификатором {0}", id);

            _logger.Info("Удаление версии {0}, '{1}', uid - '{2}', версия - '{3}'",
                EnumHelper<VersionedDocumentType>.GetDisplayValue(documentType), title, infoId, version);
          
        }

        [Authorize(Roles = ItsRoles.ApproveOhopRpdRpm)]
        public ActionResult UpdateVersionStatus(int id, int status, string comment)
        {
            var ohop = db.BasicCharacteristicOPs.FirstOrDefault(b => b.VersionedDocumentId == id);
            if (ohop == null)
                return Json(new { success = false, message = "Редактируемый документ не найден" });//, "text/html", Encoding.Unicode);

            var statusUpop = db.UpopStatuses.FirstOrDefault(s => s.Id == status);
            if (statusUpop == null)
                return Json(new { success = false, message = "Статус не найден" });//, "text/html", Encoding.Unicode);
            
            ohop.Status = statusUpop;
            ohop.StatusChangeTime = DateTime.Now;
            ohop.Comment = comment;
            
            if (!statusUpop.CanEdit()) // Подписан или В обработке
            {
                ohop = SaveDocxAndZip(ohop);
            }

            db.Entry(ohop).State = EntityState.Modified;
            db.SaveChanges();

            _logger.Info($"Изменен статус документа ОХОП Id = {ohop.VersionedDocumentId} Status = {ohop.Status.Id} {ohop.Status.Name}" +
                $"Profile = {ohop.Info.ProfileId} {ohop.Info.Profile?.CODE} {ohop.Info.Profile?.NAME}");

            return Json(new { success = true });//, "text/html", Encoding.Unicode);
        }

        public ActionResult SendVersion(int id)
        {
            var ohop = db.BasicCharacteristicOPs.Include(b => b.Status).FirstOrDefault(b => b.VersionedDocumentId == id);
            if (ohop == null)
                return Json(new { success = false, message = "Редактируемый документ не найден" });//, "text/html", Encoding.Unicode);

            if (!ohop.Status.CanEdit())
                return Json(new { success = false, message = "Документ уже подписан или находится в обработке" });//, "text/html", Encoding.Unicode);

            var statusUpop = db.UpopStatuses.FirstOrDefault(s => s.Id == 11 /*"В обработке"*/);
            if (statusUpop == null)
                return Json(new { success = false, message = "Статус не найден" });//, "text/html", Encoding.Unicode);

            ohop = SaveDocxAndZip(ohop);

            ohop.Status = statusUpop;
            ohop.StatusChangeTime = DateTime.Now;
            db.Entry(ohop).State = EntityState.Modified;
            db.SaveChanges();

            _logger.Info($"Изменен статус документа ОХОП (Отправлен на согласование) Id = {ohop.VersionedDocumentId} Status = {ohop.Status.Id} {ohop.Status.Name}" +
                $"Profile = {ohop.Info.ProfileId} {ohop.Info.Profile?.CODE} {ohop.Info.Profile?.NAME}");

            return Json(new { success = true, status = statusUpop.Name, statusId = statusUpop.Id,
                statusDate = $"{ohop.StatusChangeTime.ToShortDateString()} {ohop.StatusChangeTime.ToShortTimeString()}" });//, "text/html", Encoding.Unicode);
        }

        private BasicCharacteristicOP SaveDocxAndZip(BasicCharacteristicOP ohop)
        {
            var document = db.VersionedDocuments
                .Include(d => d.Template)
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .FirstOrDefault(d => d.Id == ohop.VersionedDocumentId);

            var fileName = $"{ohop.Info.Profile.CODE} {ohop.Info.Year} версия {ohop.Version}";

            var docxStream = _documentService.Print(document, FileFormat.Docx);
            ohop.FileStorageDocxId = FileStorageHelper.SaveFile(docxStream, $"{fileName}.docx", FileCategory.OHOP, folder: $"{ohop.Info.Year}",
                comment: $"ОХОП docx {ohop.Info.Profile.OksoAndTitle} {ohop.Info.Year} версия {ohop.Version}", id: ohop.FileStorageDocxId);

            var zipStream = _documentService.PrintZip(document, FileFormat.Docx);
            ohop.FileStorageZipId = FileStorageHelper.SaveFile(zipStream, $"{fileName}.zip", FileCategory.OHOP, folder: $"{ohop.Info.Year}",
                comment: $"ОХОП zip {ohop.Info.Profile.OksoAndTitle} {ohop.Info.Year} версия {ohop.Version}", id: ohop.FileStorageZipId);

            return ohop;
        }

        public async Task<ActionResult> GetCompetencesGroups(string profileId, string type)
        {
            var profile = db.Profiles.FirstOrDefault(p => p.ID == profileId);

            var competenceGroups = db.Competences.Where(c => c.Type == type 
                    && !c.IsDeleted && c.Standard == profile.Direction.standard
                    && c.AreaEducationId == profile.Direction.AreaEducationId
                    && c.QualificationName == profile.QUALIFICATION)
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.Content,
                    c.Type,
                    c.CompetenceGroupId,
                    c.CompetenceGroup
                })
                .GroupBy(c => c.CompetenceGroup)
                .Select(c => new
                {
                    c.Key.Id,
                    c.Key.Name,
                    Competences = c.Select(_c => new CompetenceInfoVM()
                    {
                        Id = _c.Id,
                        Content = _c.Content,
                        Type = _c.Type,
                        Code = _c.Code,
                        CompetenceGroupId = _c.CompetenceGroupId.HasValue ? _c.CompetenceGroupId.Value : 0,
                        CompetenceGroupName = _c.CompetenceGroup.Name
                    })
                }).ToList();

            return await Task.FromResult(Json(competenceGroups, new JsonSerializerSettings()));
        }

        public async Task<ActionResult> GetProfessionalCompetences(int id)
        {
            var profile = db.BasicCharacteristicOPInfos.Find(id).Profile;

            var competences = db.Competences.Where(c => c.Type == "ПК"
                    && !c.IsDeleted && c.Standard == profile.Direction.standard
                    && (c.AreaEducationId == profile.Direction.AreaEducationId || c.AreaEducationId == null)
                    && (c.QualificationName == profile.QUALIFICATION || c.QualificationName == null)
                    && (c.ProfileId == profile.ID || c.ProfileId == null))
                .Select(c => new CompetenceInfoVM()
                {
                    Id = c.Id,
                    Code = c.Code,
                    Content = c.Content,
                    Type = c.Type
                }).ToList();

            return await Task.FromResult(Json(competences, new JsonSerializerSettings()));
        }

        public ActionResult GetVariants(int documentId)
        {
            var doc = db.BasicCharacteristicOPs.FirstOrDefault(b => b.VersionedDocumentId == documentId);
            var variants = db.Variants.Where(v => v.Program.profileId == doc.Info.ProfileId && v.Program.State == VariantState.Approved && v.State == VariantState.Approved)
                .Select(v => new VariantInfoModel
                {
                    Id = v.Id.ToString(),
                    IdSource = IdSource.Variants,
                    Name = v.Name
                }).ToList(); 
            var trajectories = db.VariantUni.Where(t => t.ProfileId == doc.Info.ProfileId)
                .Select(t => new VariantInfoModel
                {
                    Id = t.TrajectoryUuid,
                    IdSource = IdSource.VariantUni,
                    Name = t.DocumentName
                });
            variants.AddRange(trajectories);
            variants = variants.OrderBy(v => v.Name).ToList();
            return Json(variants, new JsonSerializerSettings());
        }

        [HttpPost]
        public ActionResult GetApprovalAct(string act)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<ApprovalActTemplateModel>(act);
                var he = Startup._enviroment;

                var fullName = Path.Combine(he.ContentRootPath, @"ApprovalAct.docx");

                var input = System.IO.File.Open(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                var output = new MemoryStream();
                    
                var engine = new WordDocxTemplateReportingEngine();
                engine.Build(input, model, output, FileFormat.Docx);

                var length = Convert.ToInt32(output.Length);
                var bytes = output.ToArray();
                // запись массива байтов в файл

                input.Close();
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, $"акт согласования.docx");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Акт согласования не сформирован\n{ex.Message}" }, new JsonSerializerSettings());
            }
        }

        public ActionResult UploadScan(int info, string comment)
        {
            var file = Request.Form.Files[0];
            var data = db.BasicCharacteristicOPInfos.FirstOrDefault(i => i.Id == info);
            int? id = FileStorageHelper.SaveFile(file, FileCategory.OHOP, folder: $"{data.ProfileId}_{data.Year}", comment: comment,
                id: null /*файл не перезаписываем на случай, если пользователь не сохранит изменения*/);

            return Json(new { success = true, fileName = file.FileName, id }, new JsonSerializerSettings());
        }

        public ActionResult DownloadScan(int id)
        {
            var data = db.FileStorage.FirstOrDefault(s => s.Id == id);
            var bytes = FileStorageHelper.GetBytes(id);
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, data?.FileNameForUser);
        }

        //public ActionResult RemoveApprovalAct(int id)
        //{
        //    var success = FileStorageHelper.RemoveFile(id);
        //    return Json(new { success }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult ProfActivityArea()
        {
            var areas = db.ProfStandards.Include(p => p.ProfOrders).GroupBy(s => s.ProfActivityArea).Select(s => new ProfActivityAreaData
            {
                AreaCode = s.Key.Code,
                AreaTitle = s.Key.Title,

                Kinds = s.Select(sk => new ProfActivityKindData
                {
                    KindCode = sk.ProfActivityKind.Code,
                    KindTitle = sk.ProfActivityKind.Title,

                    StandardCode = sk.Code,
                    StandardTitle = sk.Title,

                    IsOldStandard = sk.ProfOrders.Count == 0 ? false 
                                        : !sk.ProfOrders.Any(o => o.Status != "Утратил силу" && o.Status != "Отменён"
                                                            || (o.OrderChanges.Count != 0 && o.OrderChanges.Any(c => c.Status != "Утратил силу" && c.Status != "Отменён")))
                })
            });
            
            var json = Json(
                new
                {
                    data = areas
                },
                new JsonSerializerSettings()
            );

            return json;
        }

        public ActionResult DownloadReport(string filter)
        {
            var reportVms = GetFilteredVersionOhop(filter).ToList();

            var chairs = reportVms.Select(r => r.chairId).Distinct();
            var divisions = db.Divisions.Where(d => chairs.Contains(d.uuid)).Select(d => d.parent).Distinct()
                .Select(d => db.Divisions.FirstOrDefault(i => i.uuid == d))
                .Select(d => new
                {
                    Id = d.uuid,
                    Title = d.shortTitle + " (" + d.title + ")"
                }).ToList();

            foreach (var version in reportVms)
            {
                var document = db.VersionedDocuments.FirstOrDefault(d => d.Id == version.versionedDocumentId);
                if (document == null)
                    continue;

                var model = _documentService.CreateModel<BasicCharacteristicOPSchemaModel>(document);
                if (model == null)
                    continue;
                
                version.divisionTitle = divisions.FirstOrDefault(d => d.Id == version.divisionId)?.Title;

                var rows = model.Variants.VariantInfos.SelectMany(i => i.ProfActivityRows);

                version.rowsCountTable = rows.Count();

                version.filledRowsCountTable1 = rows.Count(r =>
                                   !string.IsNullOrWhiteSpace(r.ProfObjects)
                                && !string.IsNullOrWhiteSpace(r.ProfTaskTypes)
                                && !string.IsNullOrWhiteSpace(r.AreaTitle)
                                && (r.NoProfStandard
                                        || !r.NoProfStandard && !string.IsNullOrWhiteSpace(r.AreaCode) && !string.IsNullOrWhiteSpace(r.Functions)
                                            && !string.IsNullOrWhiteSpace(r.KindCode) && !string.IsNullOrWhiteSpace(r.KindTitle)
                                            && !string.IsNullOrWhiteSpace(r.StandardCode) && !string.IsNullOrWhiteSpace(r.StandardTitle)));

                version.filledRowsCountTable4 = rows.Count(r => r.Competences.Count > 0);
            }

            var filename = $"Отчет_по_Версиям_ОХОП.xlsx";

            var stream = new VariantExport().Export(new { Rows = reportVms }, "OHOPReportTemplate.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename.ToDownloadFileName());
        }

        public ActionResult RequisitesOrders(int id)
        {
            var info = db.BasicCharacteristicOPInfos.FirstOrDefault(b => b.Id == id);
            var defaultValues = new BasicCharacteristicOPDefaultValues(info);

            return Json( 
                new 
                {
                    orders = defaultValues.RequisitesOrders()
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult FormAndDuration(int id)
        {
            var info = db.BasicCharacteristicOPInfos.FirstOrDefault(b => b.Id == id);
            var defaultValues = new BasicCharacteristicOPDefaultValues(info);

            return Json(
                new
                {
                    formAndDuration = defaultValues.FormAndDuration()
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult Language(int id)
        {
            var info = db.BasicCharacteristicOPInfos.FirstOrDefault(b => b.Id == id);
            var defaultValues = new BasicCharacteristicOPDefaultValues(info);

            return Json(
                new
                {
                    language = defaultValues.Language()
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult UniversalCompetences(int id)
        {
            var info = db.BasicCharacteristicOPInfos.FirstOrDefault(b => b.Id == id);
            var defaultValues = new BasicCharacteristicOPDefaultValues(info);

            return Json(
                new
                {
                    competences = defaultValues.UniversalCompetences()
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult GeneralCompetences(int id)
        {
            var info = db.BasicCharacteristicOPInfos.FirstOrDefault(b => b.Id == id);
            var defaultValues = new BasicCharacteristicOPDefaultValues(info);

            return Json(
                new
                {
                    competences = defaultValues.GeneralCompetences()
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult ProfStandardsList(int id, string codes)
        {
            var info = db.BasicCharacteristicOPInfos.FirstOrDefault(b => b.Id == id);
            var defaultValues = new BasicCharacteristicOPDefaultValues(info);
            var codesArray = JsonConvert.DeserializeObject<string[]>(codes);

            return Json(
                new
                {
                    profStandardsList = defaultValues.ProfStandardsList(codesArray)
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult RatifyData()
        {
            return Json(db.BasicCharacteristicOPRatifyData.ToList(), new JsonSerializerSettings());
        }
    }
}