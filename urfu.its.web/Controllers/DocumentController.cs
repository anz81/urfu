using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Autofac;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using Newtonsoft;
using PagedList;
using TemplateEngine;
using Urfu.Its.Common;
using Urfu.Its.Integration;
using Urfu.Its.Integration.Models;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model;
using Urfu.Its.Web.Models;
//using ActionFilterAttribute = Microsoft.AspNetCore.Mvc.Filters;
using PagedList.Core;

namespace Urfu.Its.Web.Controllers
{
    [Route("Document")]
    [ControllerErrorLogger]
    [Authorize(Roles = ItsRoles.WorkingProgramView)]
    public class DocumentController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly IComponentContext _context;
        private readonly IVersionedDocumentService _documentService;
        private readonly IObjectLogger<DocumentController> _logger;

        public DocumentController(ApplicationDbContext db, IComponentContext context,
            IVersionedDocumentService documentService, IObjectLogger<DocumentController> logger)
        {
            _db = db;
            _context = context;
            _documentService = documentService;
            _logger = logger;
        }

        [HttpGet]
        [Route("ModuleWorkingPrograms")]
        public ActionResult ModuleWorkingPrograms(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var sortRules = SortRules.Deserialize(sort);

                var filterRules = FilterRules.Deserialize(filter);
                var modules = _db.ModuleWorkingProgramsForUser(User).Include(m => m.Module)
                    .Select(m => new
                {
                    m.VersionedDocumentId,
                    moduleId = m.Module.uuid,
                    m.Module.title,
                    m.Module.shortTitle,
                    m.Module.coordinator,
                    m.Module.specialities,
                    Status = m.Status.Name,
                    statusObj = m.Status,
                    statusChangeTime = m.StatusChangeTime,
                    version = m.Version,
                    type = m.VersionedDocument.Template.DocumentType
                });
                if (sortRules == null || sortRules.Count == 0)
                {
                    modules = modules.OrderBy(d => d.title).ThenBy(d => d.version);
                }
                else
                {
                    var sortRule = sortRules[0];
                    modules = modules.OrderBy(sortRule);
                }

                modules = modules.Where(filterRules);
                var paginated = modules.ToList().Select(m => new
                {
                    m.VersionedDocumentId,
                    moduleId = m.moduleId,
                    m.title,
                    m.shortTitle,
                    m.coordinator,
                    m.specialities,
                    //m.Module.yea

                    m.Status,
                    SedOp = m.statusObj?.CanSend2Upop() ?? true,
                    statusChangeTime = m.statusChangeTime, //.ToString("dd.MM.yy hh:mm"),
                    version = m.version,
                    type = m.type
                }).ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(
                    new
                    {
                        data = paginated,
                        total = modules.Count()
                    }
                );

            }
            return View();
        }

        [HttpGet]
        [Route("DisciplinesWorkingPrograms")]
        public ActionResult DisciplinesWorkingPrograms(int moduleWPId, string sort, string filter)
        {
            var sortRules = SortRules.Deserialize(sort);

            var filterRules = FilterRules.Deserialize(filter);
            var disciplines = _db.DisciplineWorkingPrograms.Where(d => d.ModuleWorkingProgramId == moduleWPId).Select(m => new
            {
                m.VersionedDocumentId,
                disciplineId = m.Discipline.uid,
                m.Discipline.title,
                m.ModuleWorkingProgramId,
                version = m.Version,
                type = m.VersionedDocument.Template.DocumentType
            });
            if (sortRules == null || sortRules.Count == 0)
            {
                disciplines = disciplines.OrderBy(d => d.title);
            }
            else
            {
                var sortRule = sortRules[0];
                disciplines = disciplines.OrderBy(sortRule);
            }

            disciplines = disciplines.Where(filterRules);

            return JsonNet(disciplines);
        }
        [HttpPost]
        [Route("SendToSed")]
        public ActionResult SendToSed(int wpId)
        {
            try
            {
                var workingProgram = _db.ModuleWorkingPrograms.Include(_=>_.Status).Include(_=>_.VersionedDocument.Template).FirstOrDefault(d => d.VersionedDocumentId == wpId);

                if (workingProgram == null)
                    return Json(new { success = false, message = $"РПМ с идентификатором {wpId} не найдена" });

                bool canSend2Upop = workingProgram.Status?.CanSend2Upop() ?? true;

                if (!canSend2Upop) 
                {
                    return Json(new { success = false, message = $"РПМ с идентификатором {wpId} уже отправлена" });
                }
             
                var workingProgramDocumentDto = CreateWorkingProgramDocumentDto(wpId, workingProgram);

                var service = new SedWorkingProgramRestService();
                service.SendDocument(workingProgramDocumentDto);

                workingProgram.UpopStatusId = GetSend2UpopStatus().Id;
                workingProgram.StatusChangeTime = DateTime.Now;
                

                _db.SaveChanges();

                var sedOp = workingProgram.Status.CanSend2Upop();

                return Json(new {
                    success = true,
                    message = $"РПМ отправлен в УПОП",
                    status = workingProgram.Status.Name,
                    statusName = workingProgram.Status.Name,
                    sedOp = sedOp,
                    //statusChangeTime = workingProgram.StatusChangeTime.ToString("dd.MM.yy hh:mm")
                });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"РПМ не отправлен в СЭД\n{ex.Message}" });
            }
        }

        [Route("GetSedStatus")]
        public ActionResult GetSedStatus(int wpId)
        {
            var workingProgram = _db.ModuleWorkingPrograms.Include(_ => _.Status).Include(_ => _.VersionedDocument.Template).FirstOrDefault(d => d.VersionedDocumentId == wpId);
            if (workingProgram == null)
                return Json(new { success = false, message = $"РПМ с идентификатором {wpId} не найдена" });

            if (workingProgram.Status.CanSend2Upop())
            {
                return Json(new { success = false, message = $"РПМ с идентификатором {wpId} не отправлялась" });
            }

            var service = new SedWorkingProgramRestService();
            var statusDocument = service.GetStatusDocument(workingProgram.ModuleId);

            var status = _db.UpopStatuses.FirstOrDefault(_ =>
               _.Name.Equals(statusDocument.Status, StringComparison.InvariantCultureIgnoreCase));
            if (status!= null)
            {
                workingProgram.UpopStatusId = status.Id;
                workingProgram.StatusChangeTime = DateTime.Now;
                _db.SaveChanges();
            }

            var sedOp = workingProgram.Status.CanSend2Upop();
            return Json(new {
                success = true,
                message = $"Статус документа обновлен",
                status = status?.Id,
                statusName = status?.Name,
                sedOp = sedOp,
               //statusChangeTime = workingProgram.StatusChangeTime.ToString("dd.MM.yy hh:mm")
            });
        }

        private WorkingProgramDocumentDto CreateWorkingProgramDocumentDto(int id, ModuleWorkingProgram workingProgram)
        {
            var document = _db.VersionedDocuments
                .Include(d => d.Template)
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .FirstOrDefault(d => d.Id == id);
            var moduleWPModel = _documentService.CreateModel(document, "FileName","Fdps", "Profiles");
            var filename = $"{moduleWPModel.GetPropertyValue<string>("FileName")}.docx";
            var bytes = ((MemoryStream) _documentService.Print(document, FileFormat.LockedDocx)).ToArray();
            var versionUuids = moduleWPModel.GetPropertyValue<ICollection<FamilirizationTypeDirectionPlanInfo>>("Fdps").Select(_=>_.PlanVersionId).ToList();
            var plansDto = _db.Plans.Where(_ => versionUuids.Contains(_.versionUUID)).Select(_ =>
                new WorkingProgramDocumentDto.WPPlanVersionDto()
                {
                    eduplan_uuid = _.eduplanUUID,
                    version_uuid = _.versionUUID
                }).ToList();
            var profilesId = moduleWPModel.GetPropertyValue<ICollection<ProfileTrajectoriesInfo>>("Profiles").Select(_=>_.Id).ToList();
            var workingProgramDocumentDto = new WorkingProgramDocumentDto
            {
                id = workingProgram.VersionedDocumentId,
                module_uuid = workingProgram.ModuleId,
                type = workingProgram.VersionedDocument.Template.DocumentType.ToString(),
                version = workingProgram.Version,
                edu_plans = plansDto,
                profiles = profilesId,
                file_name = filename,
                file_size = bytes.Length,
                file = bytes,
                started_by = User.Identity.Name,
                status = workingProgram.Status?.Name,
                disciplines = GetDisciplinesDto4Module(workingProgram)
            };
            return workingProgramDocumentDto;
        }

        private List<WorkingProgramDocumentDto.DisciplineWorkingProgramDto> GetDisciplinesDto4Module(
            ModuleWorkingProgram workingProgram)
        {
            var disciplinesDto = _db.DisciplineWorkingPrograms.Include(_ => _.VersionedDocument.Template)
                .Where(_ => _.ModuleWorkingProgramId == workingProgram.VersionedDocumentId).ToList().Select(
                    dWp =>
                    {
                        var document = _db.VersionedDocuments
                            .Include(d => d.Template)
                            .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                            .FirstOrDefault(d => d.Id == dWp.VersionedDocumentId);
                        dynamic disciplineWp = _documentService.CreateProxyModel(document, "FileName");
                        var filename = $"{disciplineWp.FileName}.docx";
                        var bytes = ((MemoryStream) _documentService.Print(document, FileFormat.LockedDocx)).ToArray();
                        var workingProgramDocumentDto = new WorkingProgramDocumentDto.DisciplineWorkingProgramDto
                        {
                            id = workingProgram.VersionedDocumentId,
                            discipline_uuid = dWp.DisciplineId,
                            module_uuid = workingProgram.ModuleId,
                            type = workingProgram.VersionedDocument.Template.DocumentType.ToString(),
                            version = workingProgram.Version,
                            file_name = filename,
                            file_size = bytes.Length,
                            file = bytes,
                        };
                        return workingProgramDocumentDto;
                    }).ToList();
            return disciplinesDto;

        }

        private UPOPStatus GetSend2UpopStatus()
        {
            return _db.UpopStatuses.FirstOrDefault(_ =>
                _.Name.Equals("Отправлено в УПОП", StringComparison.InvariantCultureIgnoreCase));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> Index(int id)
        {
            _logger.Debug("Попытка открытия документа с идентификатором {0}", id);
            var document = await _db.VersionedDocuments.FindAsync(id);
            if (document == null)
                throw new HttpException((int)HttpStatusCode.NotFound, $"Документ с идентификатором {id} не найден");

            if (new[]
            {
                VersionedDocumentType.ModuleWorkingProgram,
                //VersionedDocumentType.ModuleWorkingProgramFgosVo3,
                VersionedDocumentType.GiaWorkingProgram,
                //VersionedDocumentType.GiaWorkingProgramFgosVo3,
                VersionedDocumentType.PracticesWorkingProgram,
                //VersionedDocumentType.PracticesWorkingProgramFgosVo3
            }.Contains(document.Template.DocumentType))
            {
                var wp = _db.ModuleWorkingPrograms.Find(document.Id);
                ViewBag.ModuleId = wp.ModuleId;
            }

            var service = ResolveDocumentImplementationService(document.Template.DocumentType);
            var vm = service.GetNavigationViewModel(document);

            if (vm == null)
                throw new NotImplementedException();

            return View(vm.ViewName, vm);
        }

        [HttpGet]
        [Route("{id}/{section}")]
        public async Task<ActionResult> Section(int id, string section)
        {
            void FillLiteratureServiceLink()
            {
                var literatureService = new LiteratureService();

                var samAccountName = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "samaccountname")?.Value;
#if DEBUG
                samAccountName = "A.A.Absaliamov";
#endif
                try
                {
                    var abisRuslanUser = samAccountName == null
                        ? new AbisRuslanUser()
                        : literatureService.GetAbisRuslanUser(samAccountName);
                    string literatureServiceUrl = ConfigurationManager.AppSettings["LiteratureServiceUrl"];
                    var split = literatureServiceUrl.Split(new[] { '?' }, 2);
                    var address = split[0];
                    var queryString = split[1];
                    var query = HttpUtility.ParseQueryString(queryString);
                    query["USERID"] = abisRuslanUser.BARCODE;
                    query["PASSWORD"] = abisRuslanUser.PASSWORD;
                    literatureServiceUrl = $"{address}?{query}";
                    ViewBag.AdditionalData = new
                    {
                        LiteratureServiceUrl = literatureServiceUrl
                    };
                }
                catch (Exception ex)
                {
                    ViewBag.AdditionalData = new
                    {
                        LiteratureServiceUrl = (string)null,
                        LiteratureServiceError = ex.Message
                    };
                }
            }

            _logger.Debug("Попытка открытия раздела '{1}' документа с идентификатором {0}", id, section);
            var document = await _db.VersionedDocuments
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .Include(d => d.Template)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (document == null)
                throw new HttpException((int)HttpStatusCode.NotFound, $"Документ с идентификатором {id} не найден");

            var service = ResolveDocumentImplementationService(document.Template.DocumentType);
            var vm = service.GetNavigationViewModel(document);

            if (vm == null)
                throw new NotImplementedException();

            if (document.Template.DocumentType == VersionedDocumentType.DisciplineWorkingProgram)
            {
                switch (section)
                {
                    case "InformationSupport":
                        FillLiteratureServiceLink();

                        break;
                    case "Application1":
                        ViewBag.AdditionalData = new
                        {
                            TechCardServiceUrl = ConfigurationManager.AppSettings["TechCardServiceUrl"]
                        };
                        break;
                }
            }

            if (document.Template.DocumentType == VersionedDocumentType.GiaWorkingProgram && section.Equals("Manuals"))
                FillLiteratureServiceLink();

            if (document.Template.DocumentType == VersionedDocumentType.PracticesWorkingProgram && section.Equals("Manuals"))
                FillLiteratureServiceLink();

            var sectionVm = vm.Sections.FirstOrDefault(s => s.SystemName == section);
            if (sectionVm == null)
            {
                _logger.Debug("Раздел '{0}' документа типа '{1}' не найден в модели представления. Перенаправление на список разделов.", section, document.Template.DocumentType);
                var queryParameters = new RouteValueDictionary();
                foreach (string key in Request.QueryString.Keys)
                    queryParameters[key] = Request.QueryString[key];
                queryParameters.Add("id", document.Id);
                return RedirectToAction("Index", queryParameters);
            }

            //object model = _documentService.CreateProxyModel(document);
            //ViewBag.Data = model;
            ViewBag.DataString = _documentService.CreateSerializedModel(document);
            ViewBag.Schema = service.GetDocumentDescriptor();
            ViewBag.EmptyData = Activator.CreateInstance(service.GetActualSchemaType());

            return View(sectionVm.ViewName, sectionVm);
        }

        [HttpPost]
        [Route("{id}/Save")]
        [ErrorFilter]
        //[Authorize(Roles = ItsRoles.WorkingProgramManager)]
        public async Task<ActionResult> Save(int id, string blocksData)
        {
            _logger.Debug("Попытка сохранения документа с идентификатором {0}", id);
            _logger.Debug("Данные для сохранения с клиента: {0}", blocksData);
            var document = await _db.VersionedDocuments
                .Include(d => d.Template)
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .FirstOrDefaultAsync(d => d.Id == id);
            if (document == null)
                throw new HttpException((int)HttpStatusCode.NotFound, $"Документ с идентификатором {id} не найден");

            var result = new JObject();

            var changedDocumentData = _documentService.ApplyDocumentChanges(document, blocksData, out var inspections);
            result.Add("inspections", JArray.FromObject(inspections));
            result.Add("changedDocumentData", JObject.Parse(changedDocumentData));

            var success = !inspections.SelectMany(i => i.Errors).Any();
            result.Add("success", success);

            if (success)
                _db.SaveChanges();

            string blockNames = string.Join(", ", JObject.Parse(changedDocumentData).Properties().Select(p => p.Name));

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
                        .FirstOrDefault(b => b.VersionedDocumentId == document.Id);
                    uid = $"{ohop.InfoId}";
                    title = ohop.Info.Profile.NAME;
                    version = ohop.Version;
                    break;
                case VersionedDocumentType.CompetencePassport:
                    var passport = _db.CompetencePassports.Include(p => p.Status)
                        .FirstOrDefault(b => b.VersionedDocumentId == document.Id);
                    uid = $"{passport.VersionedDocumentId}";
                    title = passport.BasicCharacteristicOP.Info.Profile.NAME;
                    version = passport.Version;
                    break;
                case VersionedDocumentType.ModuleAnnotation:
                    var annotation = _db.ModuleAnnotations.Include(p => p.Status)
                        .FirstOrDefault(b => b.VersionedDocumentId == document.Id);
                    uid = $"{annotation.VersionedDocumentId}";
                    title = annotation.BasicCharacteristicOP.Info.Profile.NAME;
                    version = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _logger.Info("Редактирование по кнопке 'Сохранить': блоки - [{0}], {1}, '{2}', версия - {3}, uuid - '{4}'",
                blockNames, EnumHelper<VersionedDocumentType>.GetDisplayValue(documentType),
                title, version, uid);

            return Content(result.ToString(), "application/json", Encoding.UTF8);
        }

        [HttpGet]
        [Route("{id}/Print")]
        public async Task<ActionResult> Print(int id, string format)
        {
            _logger.Debug("Попытка формирования печатной формы документа с идентификатором {0}", id);
            var document = await _db.VersionedDocuments
                .Include(d => d.Template)
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .FirstOrDefaultAsync(d => d.Id == id);
            if (document == null)
                throw new HttpException((int)HttpStatusCode.NotFound, $"Документ с идентификатором {id} не найден");

            FileFormat fileFormat;
            string fileExtension = format;
            switch (format)
            {
                case "docx":
                    fileFormat = FileFormat.Docx;
                    break;
                case "lockedDocx":
                    fileFormat = FileFormat.LockedDocx;
                    fileExtension = "docx";
                    break;
                case "pdf":
                    fileFormat = FileFormat.Pdf;
                    break;
                default:
                    throw new InvalidOperationException($"Формат '{format}' не поддерживается");
            }

            dynamic model = _documentService.CreateProxyModel(document, "FileName");
            string fileName = $"{model.FileName}.{fileExtension}";

            if (document.Template.DocumentType == VersionedDocumentType.BasicCharacteristicOP)
            {
                var ohop = _db.BasicCharacteristicOPs.Include(b => b.Status).FirstOrDefault(b => b.VersionedDocumentId == id);
                if (ohop != null && ohop?.FileStorageDocxId != null && ohop?.Status?.CanEdit() == false)
                {
                    // берем документ из хранилища в случае, если он не может редактироваться и есть в хранилище
                    var fileStream = FileStorageHelper.GetStream(ohop.FileStorageDocxId.Value);
                    return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            if (document.Template.DocumentType == VersionedDocumentType.CompetencePassport)
            {
                var passport = _db.CompetencePassports.Include(b => b.Status).FirstOrDefault(b => b.VersionedDocumentId == id);
                if (passport != null && passport?.FileStorageDocxId != null && passport?.Status?.CanEdit() == false)
                {
                    // берем документ из хранилища в случае, если он не может редактироваться и есть в хранилище
                    var fileStream = FileStorageHelper.GetStream(passport.FileStorageDocxId.Value);
                    return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            if (document.Template.DocumentType == VersionedDocumentType.ModuleAnnotation)
            {
                var annotation = _db.ModuleAnnotations.Include(b => b.Status).FirstOrDefault(b => b.VersionedDocumentId == id);
                if (annotation != null && annotation?.FileStorageDocxId != null && annotation?.Status?.CanEdit() == false)
                {
                    // берем документ из хранилища в случае, если он не может редактироваться и есть в хранилище
                    var fileStream = FileStorageHelper.GetStream(annotation.FileStorageDocxId.Value);
                    return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }

            var printedDocument = _documentService.Print(document, fileFormat);
            
            printedDocument.Position = 0;

            return File(printedDocument, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        [Route("{id}/Archive")]
        public async Task<ActionResult> Archive(int id)
        {
            _logger.Debug("Попытка формирования архива ОХОП документа с идентификатором {0}", id);
            var document = await _db.VersionedDocuments
                .Include(d => d.Template)
                .Include(d => d.BlockLinks.Select(l => l.DocumentBlock))
                .FirstOrDefaultAsync(d => d.Id == id);
            if (document == null)
                throw new HttpException((int)HttpStatusCode.NotFound, $"Документ с идентификатором {id} не найден");

            dynamic model = _documentService.CreateProxyModel(document, "FileName");
            string fileName = $"{model.FileName}.zip".ToDownloadFileName();

            if (document.Template.DocumentType == VersionedDocumentType.BasicCharacteristicOP)
            {
                var ohop = _db.BasicCharacteristicOPs.Include(b => b.Status).FirstOrDefault(b => b.VersionedDocumentId == id);
                if (ohop != null && ohop?.FileStorageZipId != null && ohop?.Status?.CanEdit() == false)
                {
                    // берем архив из хранилища в случае, если он не может редактироваться и есть в хранилище
                    var fileStream = FileStorageHelper.GetStream(ohop.FileStorageZipId.Value);
                    return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }

            var zip = _documentService.PrintZip(document, FileFormat.Docx);
            
            return new FileContentResult(zip.ToArray(), "application/zip")
            {
                FileDownloadName = fileName
            };
        }

        private IVersionedDocumentImplementationService ResolveDocumentImplementationService(
            VersionedDocumentType documentType)
        {
            var service = _context.ResolveKeyed<IVersionedDocumentImplementationService>(documentType);
            return service;
        }
    }

    public class ControllerErrorLoggerAttribute : ActionFilterAttribute
    {
        public IComponentContext Context { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var logger = (IObjectLogger)Context.Resolve(typeof(IObjectLogger<>).MakeGenericType(filterContext.Controller.GetType()));
            if (filterContext.Exception != null)
            {
                logger.Error(filterContext.Exception);
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                base.OnActionExecuted(filterContext);
            }
        }
    }

    public class ErrorFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                Logger.Debug(filterContext.Exception.ToString());
                filterContext.Result = new ContentResult
                {
                    Content = filterContext.Exception.Message,
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "text/plain; charset=UTF-8"
                };
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.HttpContext.Response.ContentType = "text/plain; charset=UTF-8";
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                base.OnActionExecuted(filterContext);
            }
        }
    }
}