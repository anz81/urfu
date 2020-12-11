using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AutoMapper.Internal;
//using Microsoft.Ajax.Utilities;
using PagedList.Core;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Excel;
using Urfu.Its.Web.Models;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Urfu.Its.Web.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.VariantsView)]
    public class VariantController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private bool isBase;

        // GET: /Variant/
        public ActionResult Index(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var variants = db.VariantsForUser(User).Include(v => v.Program.Direction).Where(v => !v.IsBase).Select(v => new
                {
                    v.Id,
                    DirectionOkso = v.Program.Direction.okso,
                    DirectionTitle = v.Program.Direction.title + " (" + v.Program.Direction.standard + ")",
                    ProgramName = v.Program.Name,
                    Name = v.Name,
                    v.DocumentName,
                    State = v.State,
                    ProgramYear = v.Program.Year,
                    CreateDate = v.CreateDate
                });

                SortRules sortRules = SortRules.Deserialize(sort);
                variants = variants.OrderBy(sortRules.FirstOrDefault(), v => v.DirectionOkso);

                variants = variants.Where(FilterRules.Deserialize(filter));

                var paginated = variants.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = variants.Count()
                });
            }

            return View();
        }
        [HttpPost]
        public ActionResult SetDocumentName(int variantId, string documentName)
        {
            var variant = db.Variants.FirstOrDefault(_ => _.Id == variantId);
            if (variant != null)
            {
                variant.DocumentName = documentName;
                db.SaveChanges();
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            return NotFound("variant not found");

        }
        // GET: /Variant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Variant variant = db.Variants.Find(id);
            if (variant == null)
            {
                return NotFound();
            }
            return View(variant);
        }

        // GET: /Variant/Create
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Create()
        {
            ViewBag.directionId = new SelectList(db.DirectionsForUser(User).Where(d => db.EduPrograms.Any(p => p.directionId == d.uid && p.State == VariantState.Approved && p.Variant.State == VariantState.Approved)).OrderBy(d => d.okso).ThenBy(d => d.title), "uid", "OksoAndTitleStandard");

            ViewBag.CopyFromVariantId = new SelectList(Enumerable.Empty<Variant>(), "Id", "Name");
            ViewBag.EduProgramId = new SelectList(Enumerable.Empty<EduProgram>(), "Id", "DirectionAndName");
            return View();
        }

        // POST: /Variant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Create(/*[Bind(Include = "Id,Name,directionId,EduProgramId,CopyFromVariantId")] */CreateVariantViewModel variant)
        {
            Variant copyFrom = null;
            if (variant.CopyFromVariantId.HasValue)
            {
                copyFrom = db.Variants.Find(variant.CopyFromVariantId);
                if (variant.directionId == null)
                    variant.directionId = copyFrom.Program.directionId;
            }

            if (copyFrom == null && variant.directionId == null)
            {
                ModelState.AddModelError(string.Empty, "Требуется направление или траектория для копирования");
            }

            if (copyFrom != null && copyFrom.Program.directionId != variant.directionId)
            {
                ModelState.AddModelError(string.Empty, "Направление траектории не совпадает с выбранным направлением");
            }

            var eduProgram = db.EduPrograms.FirstOrDefault(p => p.Id == variant.EduProgramId);
            if (eduProgram.State != VariantState.Approved || !eduProgram.PlanNumber.HasValue || eduProgram.PlanVersionNumber == 0)
            {
                ModelState.AddModelError(string.Empty, "Не утвержден выбор версии УП на версии ОП");
            }

            if (ModelState.IsValid)
            {
                var entity = db.FillVariantWithDefaults(copyFrom, variant);
                db.Variants.Add(entity);

                db.SaveChanges();
                Logger.Info("Создан варинат '{0}' с направлением '{1}'", entity.Name, db.Directions.Find(entity.Program.directionId).okso);
                return RedirectToAction("BasicContentEdit", new { variantId = entity.Id });
            }

            ViewBag.directionId = new SelectList(db.DirectionsForUser(User).Where(d => db.EduPrograms.Any(p => p.directionId == d.uid && p.State == VariantState.Approved && p.Variant.State == VariantState.Approved)).OrderBy(d => d.okso).ThenBy(d => d.title), "uid", "OksoAndTitle");

            ViewBag.CopyFromVariantId = new SelectList(Enumerable.Empty<Variant>(), "Id", "Name");
            ViewBag.EduProgramId = new SelectList(Enumerable.Empty<EduProgram>(), "Id", "DirectionAndName");
            return View(variant);
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult AddTeacher(string load, string moduleId, string eduplanUUID, int VariantId, string catalogDisciplineUUID)
        {
            var variant = db.Variants.Find(VariantId);
            var module = db.Modules.Find(moduleId);
            db.AddRandomTeacher(load, moduleId, eduplanUUID, VariantId, catalogDisciplineUUID);
            ViewBag.Title = "Дисциплины модуля " + module.title;
            ViewBag.VariantId = VariantId;
            var list = module.Plans.Where(p => p.qualification == variant.Program.qualification &&
                                                 p.familirizationType == variant.Program.familirizationType &&
                                                 p.familirizationCondition == variant.Program.familirizationCondition &&
                                                 p.versionNumber == variant.Program.PlanVersionNumber &&
                                                 p.eduplanNumber == variant.Program.PlanNumber &&
                                                 p.directionId == variant.Program.directionId &&
                                                 p.active
                ).ToList();
            foreach (var item in list)
            {
                item.Teachers = db.GetTachers(VariantId, item);
            }
            return View("Plans", list);
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Teachers(string moduleId, string eduplanUUID, int VariantId, string catalogDisciplineUUID)
        {
            var variant = db.Variants.Find(VariantId);
            var module = db.Modules.Find(moduleId);

            var plan = db.Plans.First(p => p.moduleUUID == moduleId && p.eduplanUUID == eduplanUUID && p.catalogDisciplineUUID == catalogDisciplineUUID);

            ViewBag.Title = "Преподаватели дисциплины " + plan.disciplineTitle + " в модуле " + module.numberAndTitle + " траектории " + variant.Name + " по направлению " + variant.Program.Direction.OksoAndTitle;
            ViewBag.Variant = variant;
            ViewBag.VariantId = VariantId;
            ViewBag.ModuleId = moduleId;
            ViewBag.eduplanUUID = eduplanUUID;
            ViewBag.catalogDisciplineUUID = catalogDisciplineUUID;


            var list = db.GetTachers(VariantId, plan);

            return View(list);
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult AttachTeacher(string moduleId, string eduplanUUID, int VariantId, string catalogDisciplineUUID)
        {
            var variant = db.Variants.Find(VariantId);
            var module = db.Modules.Find(moduleId);

            var plan = db.Plans.First(p => p.moduleUUID == moduleId && p.eduplanUUID == eduplanUUID && p.catalogDisciplineUUID == catalogDisciplineUUID);

            ViewBag.Title = "Добавление преподавателя дисциплины " + plan.disciplineTitle + " в модуле " + module.numberAndTitle + " траектории " + variant.Name + " по направлению " + variant.Program.Direction.OksoAndTitle;
            ViewBag.VariantId = VariantId;
            ViewBag.ModuleId = moduleId;
            ViewBag.eduplanUUID = eduplanUUID;
            ViewBag.catalogDisciplineUUID = catalogDisciplineUUID;
            ViewBag.loadKeys = plan.GetLoadKeys();


            var list = db.GetTachers(VariantId, plan).Select(pt => pt.Teacher).Distinct()/*.Concat(db.Teachers).Take(20)*/;

            return View(list);
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult AttachTeacherExecute(string pkey, string load, string moduleId, string eduplanUUID, int VariantId, string catalogDisciplineUUID)
        {
            //var variant = db.Variants.Find(VariantId);
            //var module = db.Modules.Find(moduleId);

            var plan = db.Plans.First(p => p.moduleUUID == moduleId && p.eduplanUUID == eduplanUUID && p.catalogDisciplineUUID == catalogDisciplineUUID);
            bool exists =
                db.PlanTeachers.Any(
                    p =>
                        p.moduleId == moduleId &&
                        p.eduplanUuid == eduplanUUID &&
                        p.catalogDisciplineUuid == catalogDisciplineUUID &&
                        p.load == load &&
                        p.TeacherPkey == pkey &&
                        p.variantId == VariantId);
            var variant = db.Variants.Find(VariantId);
            if (!exists && plan.GetLoadKeys().Contains(load) && db.VariantsForUser(User).Any(v => v.Id == VariantId))
            {
                var pt = new PlanTeacher()
                {
                    variantId = VariantId,
                    TeacherPkey = pkey,
                    catalogDisciplineUuid = catalogDisciplineUUID,
                    eduplanUuid = eduplanUUID,
                    load = load,
                    moduleId = moduleId,
                    Selectable = true
                };
                db.PlanTeachers.Add(pt);
                db.SaveChanges();


                variant.OnChanged();
            }


            return RedirectToAction("Teachers", new { moduleId, eduplanUUID, VariantId, catalogDisciplineUUID });
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult TeacherSearch(string query, string moduleId, string eduplanUUID, int VariantId, string catalogDisciplineUUID)
        {
            ViewBag.VariantId = VariantId;
            ViewBag.ModuleId = moduleId;
            ViewBag.eduplanUUID = eduplanUUID;
            ViewBag.catalogDisciplineUUID = catalogDisciplineUUID;
            if (string.IsNullOrWhiteSpace(query) || query.Trim().Length == 1)
            {
                var plan = db.Plans.First(p => p.moduleUUID == moduleId && p.eduplanUUID == eduplanUUID && p.catalogDisciplineUUID == catalogDisciplineUUID);
                // TODO что-то не то. Кто разбирается поправьте, плиз.
                return View(db.GetTachers(VariantId, plan));
            }

            var tokens = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var teachers = db.Teachers
                .Where(teacher =>
                    tokens.All(
                        t => teacher.firstName.Contains(t) ||
                             teacher.middleName.Contains(t) ||
                             teacher.lastName.Contains(t) ||
                             teacher.workPlace.Contains(t) ||
                             teacher.post.Contains(t))
                )
                .Take(20);
            return View(teachers);
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public void SetTeacherSelectable(int id, bool value)
        {
            var pt = db.PlanTeachers.Find(id);
            pt.Selectable = value;
            db.SaveChanges();
            db.Variants.Find(pt.variantId).OnChanged();
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult DetachTeacher(int id)
        {
            var pt = db.PlanTeachers.Find(id);
            db.PlanTeachers.Remove(pt);
            db.SaveChanges();
            db.Variants.Find(pt.variantId).OnChanged();
            return RedirectToAction("Teachers", new { VariantId = pt.variantId, pt.moduleId, pt.eduplanUuid, catalogDisciplineUUID = pt.catalogDisciplineUuid });
        }

        [Authorize(Roles = ItsRoles.VariantsView)]
        public ActionResult PlansView(string moduleId, int variantId)
        {
            var list = GetPlans(moduleId, variantId);
            ViewBag.hideEditLinks = true;
            return View("Plans", list);
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Plans(string moduleId, int variantId)
        {
            var list = GetPlans(moduleId, variantId);
            return View(list);
        }

        private List<Plan> GetPlans(string moduleId, int variantId)
        {
            var variant = db.Variants.Find(variantId);
            var module = db.Modules.Find(moduleId);
            ViewBag.Title = "Дисциплины модуля " + module.title;
            ViewBag.BackButtonText = variant.IsBase ? "Редактирование модулей версии ОП" : "Редактирование траектории";
            ViewBag.VariantId = variantId;
            var list =
                db.UniModules().Where(m => m.uuid == moduleId)
                    .SelectMany(m => m.Plans)
                    .Where(p => p.qualification == variant.Program.qualification &&
                                p.familirizationType == variant.Program.familirizationType &&
                                p.familirizationCondition == variant.Program.familirizationCondition &&
                                p.versionNumber == variant.Program.PlanVersionNumber &&
                                p.eduplanNumber == variant.Program.PlanNumber &&
                                p.directionId == variant.Program.directionId &&
                                p.active && !p.remove
                    )
                    .Where(
                        p =>
                            p.faculty == variant.Program.divisionId || p.faculty == variant.Program.departmentId ||
                            p.faculty == variant.Program.chairId)
                    .ToList();


            var direction = db.Directions.First(d => d.Programs.Any(p => p.Variants.Any(v => v.Id == variantId)));

            ViewBag.Direction = direction;

            foreach (var item in list)
            {
                item.Teachers = db.GetTachers(variantId, item);
                if (item.testUnitsByTerm == null)
                    item.testUnitsByTerm = "";
            }
            return list;
        }

        [Authorize(Roles = ItsRoles.VariantsView)]
        public ActionResult BasicContentEditold(int variantId)
        {
            Variant variant = VarinatApiHelper.VariantsQueryForEdit(db).FirstOrDefault(v => v.Id == variantId);

            variant.Program.CheckProgramVariant(db);

            Variant programVariant = VarinatApiHelper.VariantsQueryForEdit(db).FirstOrDefault(v => v.Id == variant.Program.VariantId);

            if (variant.IsBase)
            {
                ViewBag.Title = "Управление модулями версии ОП \"" + variant.Program.Name + "\", направление " +
                                   variant.Program.Direction.okso;
            }
            else
            {
                ViewBag.Title = "Управление модулями траектории \"" + variant.Name + "\" направление " +
                                variant.Program.Direction.okso;
            }


            ViewBag.ModuleTypes = db.VariantModuleTypes.Select(vmt => new SelectListItem() { Text = vmt.Name, Value = vmt.Id.ToString() }).ToList();

            return View(new EditVariantContentViewModel(variant, programVariant));
        }

       
        [Authorize(Roles = ItsRoles.VariantsView)]
        public ActionResult BasicContentEdit(int variantId)
        {
            //db = new ApplicationDbContext("DefaultConnectionTest");
            Debug.WriteLine($"Action enter at {DateTime.Now.ToLongTimeString()}");
            
            
            //Variant variant = VarinatApiHelper.VariantsQueryForEdit(db).FirstOrDefault(v => v.Id == variantId);
            Variant variant = db.Variants
                .FirstOrDefault(v => v.Id == variantId);

            
            variant.Program.CheckProgramVariant(db);

            Variant programVariant = db.Variants
                .FirstOrDefault(v => v.Id == variant.Program.VariantId);

            if (variant.IsBase)
            {
                ViewBag.Title = "Управление модулями версии ОП \"" + variant.Program.Name + "\", направление " +
                                   variant.Program.Direction.okso;
            }
            else
            {
                ViewBag.Title = "Управление модулями траектории \"" + variant.Name + "\" направление " +
                                variant.Program.Direction.okso;
            }
          
         
            ViewBag.ModuleTypes = db.VariantModuleTypes.ToList(); //.Select(vmt => new SelectListItem() { Text = vmt.Name, Value = vmt.Id.ToString() }).ToList();

            programVariant.SelectionGroups = programVariant.SelectionGroups ?? new List<VariantSelectionGroup>();

            ViewBag.ProgramVariantSelectionGroups = programVariant.SelectionGroups.Select(
                g => new { Id = g.Id.ToString(), g.Name }).ToList();
            //ViewBag.ProgramVariantSelectionGroups.Insert(0, new { Id = "", Name = " " });
           
            ViewBag.VariantSelectionGroups = variant.SelectionGroups.Select(
                g => new { Id = g.Id.ToString(), g.Name }).ToList();
          
            ViewBag.VariantSelectionGroups.Insert(0, new { Id = "-1", Name = " " });
            
            return View(new EditVariantVM(variant));
        }
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult VariantSaved(int variantId, bool saved, string message)
        {
            Variant variant = VarinatApiHelper.VariantsQueryForApi(db).FirstOrDefault(v => v.Id == variantId);
            variant.Program.CheckProgramVariant(db);
            ViewBag.Saved = saved;
            ViewBag.Message = message;
            return View(variant);
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        [ValidateAntiForgeryToken]
        public ActionResult BasicContentEdit(/*[Bind(Include = "VariantId,Name,Year,SelectionDeadline,StudentsLimit")]*/ EditVariantContentViewModel model)
        {
            Variant variant = VarinatApiHelper.VariantsQueryForApi(db).First(v => v.Id == model.VariantId);

            if (CanChangeVariant(variant))
            {
                return NotFound("Невозможно изменить утверждённую траекторию");
                //return JsonNet(new { message = "Невозможно изменить утверждённую траекторию" });
            }


            if (ModelState.IsValid)
            {
                variant.State = model.State;
                variant.Name = model.Name;
                variant.StudentsLimit = model.StudentsLimit;
                variant.Program.Year = model.Year;
                variant.SelectionDeadline = model.SelectionDeadline;

                db.SaveChanges();

                variant = VarinatApiHelper.VariantsQueryForApi(db).FirstOrDefault(v => v.Id == model.VariantId);
                variant.OnChanged();

            }
            return JsonNet(new {success = true, status = "OK"});
            //return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult GetGroupsForVariant(Int32 variantId)
        {
            Variant variant = db.Variants.FirstOrDefault(v => v.Id == variantId);

            variant.Program.CheckProgramVariant(db);

            Variant programVariant = db.Variants.FirstOrDefault(v => v.Id == variant.Program.VariantId);

            var rows = VariantHelper.VariantContentRows(variant, programVariant);

            List<VariantGroupModel> groups = 
                variant.Groups.OrderBy(g => g.GroupType).ThenBy(g => g.SubgroupType).ToList().Select(g => new VariantGroupModel(g, rows)).ToList();

            return JsonNet(new
            {
                data = groups,
                total = groups.Count()
            });

        }

        //[Authorize(Roles = ItsRoles.VariantsEdit)]
        [AllowAnonymous]
        public ActionResult GetVariantContentRows(Int32 variantId)
        {
            //db = new ApplicationDbContext("DefaultConnectionTest");
            Variant variant = db.Variants.FirstOrDefault(v => v.Id == variantId);

            variant.Program.CheckProgramVariant(db);

            Variant programVariant = db.Variants.FirstOrDefault(v => v.Id == variant.Program.VariantId);

            var rows = VariantHelper.VariantContentRows(variant, programVariant);//.Select(row =>
            //{
            //    row.SelectionGroups = row.Base
            //        ? programVariant.SelectionGroups.Select(
            //            g =>
            //                new SelectListItem()
            //                {
            //                    Text = g.Name,
            //                    Value = g.Id.ToString()
            //                }).ToList()
            //        : variant.SelectionGroups.Select(
            //            g =>
            //                new SelectListItem()
            //                {
            //                    Text = g.Name,
            //                    Value = g.Id.ToString()
            //                }).ToList();
            //    return row;
            //}).ToList();

            return JsonNet(new
            {
                data = rows,
                total = rows.Count()
            });

        }
        //[NonAction]
        //public static List<EditVariantContentRowViewModel> VariantContentRows(Variant variant, Variant programVariant)
        //{
        //    var res = variant.Program.Direction.Modules
        //        .Where(
        //            m =>
        //                m.UsedInVariantContents.Any(vc => vc.Group.VariantId == variant.Id && vc.Selected) ||
        //                m.Plans.Any(
        //                    p => p.qualification == variant.Program.qualification &&
        //                         p.familirizationType == variant.Program.familirizationType &&
        //                         p.familirizationCondition == variant.Program.familirizationCondition &&
        //                         p.versionNumber == variant.Program.PlanVersionNumber &&
        //                         p.eduplanNumber == variant.Program.PlanNumber &&
        //                         (p.faculty == programVariant.Program.divisionId ||
        //                          p.faculty == programVariant.Program.departmentId ||
        //                          p.faculty == programVariant.Program.chairId) &&
        //                         p.active
        //                )).Select(m => new EditVariantContentRowViewModel(m, variant, programVariant)).OrderBy(m => m.Priority).ThenBy(m => m.GroupType).ThenBy(m => m.ModuleName).ToList();
        //    return res;
        //}

        [HttpPost]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        [ValidateAntiForgeryToken]
        public ActionResult SetVariantGroupTestUnits(Int32 groupId, Int32 testUnits)
        {
            //TODO: Надо доделать
            var group = db.VariantGroups.FirstOrDefault(_ => _.Id == groupId);
            if (group == null) return NotFound("group not found");
            if (CanChangeVariant(group.Variant)) return NotFound("can not edit approved variant");
            group.TestUnits = testUnits;
            db.SaveChanges();
            return JsonNet(new { });
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        [ValidateAntiForgeryToken]
        public ActionResult SetVariantModuleParams(int variantId, VariantGroupType groupType, VariantGroupType? subgroupType,
                string moduleId, bool? selected, bool? selectable, int? moduleTypeId, int? selectionGroupId)
        {
            //Variant variant = VarinatApiHelper.VariantsQueryForApi(db).First(v => v.Id == variantId);
            Variant variant = db.Variants.First(v => v.Id == variantId);
            var module = db.UniModules().FirstOrDefault(m => m.uuid == moduleId);
            if (module == null || CanChangeVariant(variant))
                return JsonNet(new { message = "Невозможно изменить утверждённую траекторию" });

            var group = variant.Groups.FirstOrDefault(g => g.GroupType == groupType && g.SubgroupType == subgroupType);
            if (@group == null)
            {
                group = new VariantGroup();
                variant.Groups.Add(group);
                group.GroupType = groupType;
                group.SubgroupType = subgroupType;
                db.SaveChanges();
            }

            var content = variant.Groups.SelectMany(
                        g => ((IEnumerable<VariantContent>)g.Contents) ?? Enumerable.Empty<VariantContent>()).FirstOrDefault(c => c.moduleId == moduleId);
            if (content == null)
            {
                content = new VariantContent()
                {
                    Group = group,
                    moduleId = moduleId
                };
                db.VariantContents.Add(content);
            }
            content.Selected = selected ?? content.Selected;
            content.Selectable = selectable ?? content.Selectable;
            if (moduleTypeId == null)
            {
                var programContent = variant.Program.Variant.Groups.SelectMany(g => g.Contents).FirstOrDefault(c => c.moduleId == content.moduleId);
                content.ModuleTypeId = programContent?.ModuleTypeId ?? 1;
            }
            else content.ModuleTypeId = moduleTypeId ?? content.ModuleTypeId;
            content.VariantSelectionGroupId = selectionGroupId != null
                ? selectionGroupId > -1 ? selectionGroupId : null : content.VariantSelectionGroupId;

            db.SaveChanges();

            return JsonNet(new {});


        }
        [HttpPost]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        [ValidateAntiForgeryToken]
        public ActionResult BasicContentEditold(EditVariantContentViewModel model)
        {
            Variant variant = VarinatApiHelper.VariantsQueryForApi(db).First(v => v.Id == model.VariantId);
            if (variant.State != model.State && !User.IsInRole(ItsRoles.VariantsChangeState))
                ModelState.AddModelError(string.Empty, "Недостаточно прав для изменения состояния траектории");

            if (CanChangeVariant(variant))
                ModelState.AddModelError(string.Empty, "Невозможно изменить утверждённую траекторию");

            if (model.Rows == null)
                ModelState.AddModelError(String.Empty, "Отсутствует версия УП");

            if (ModelState.IsValid)
            {
                variant.State = model.State;
                variant.Name = model.Name;
                variant.StudentsLimit = model.StudentsLimit;
                variant.Program.Year = model.Year;
                variant.SelectionDeadline = model.SelectionDeadline;

                //var usedGroupTypes = model.Rows.Where(r=>r.Selected).Select(g => g.GroupType).Distinct().ToList();
                var usedGroupTypes = model.Rows.Select(g => g.GroupType).Distinct().ToList();

                Dictionary<VariantGroupType, VariantGroup> groups = new Dictionary<VariantGroupType, VariantGroup>();
                bool groupsCreated = false;
                foreach (var gt in usedGroupTypes)
                {
                    var group = variant.Groups.FirstOrDefault(g => g.GroupType == gt);
                    if (group == null)
                    {
                        group = new VariantGroup();
                        variant.Groups.Add(group);
                        groupsCreated = true;
                        group.GroupType = gt;
                    }
                    else
                    {
                        if (model.Groups != null)
                        {
                            var modelGroup = model.Groups.FirstOrDefault(g => g.GroupType == gt);
                            if (modelGroup != null) @group.TestUnits = modelGroup.TestUnits;
                        }
                    }
                    groups[gt] = group;
                }
                if (groupsCreated)
                    db.SaveChanges();

                var contents =
                    variant.Groups.SelectMany(
                        g => ((IEnumerable<VariantContent>)g.Contents) ?? Enumerable.Empty<VariantContent>()).ToList();
                foreach (var row in model.Rows)
                {
                    var content = contents.FirstOrDefault(c => c.moduleId == row.ModuleId);
                    if (content == null)
                    {
                        content = new VariantContent
                        {
                            Group = groups[row.GroupType],
                            moduleId = row.ModuleId
                        };
                        db.VariantContents.Add(content);
                    }
                    content.Selected = !row.Base && row.Selected;
                    if (row.ModuleTypeId != 0) //AB: Это костыль. Я не понял почему View не передало это поле для выключенных строк.
                        content.ModuleTypeId = row.ModuleTypeId;
                    if (content.ModuleTypeId == 0)
                    {
                        var programContent = variant.Program.Variant.Groups.SelectMany(g => g.Contents).FirstOrDefault(c => c.moduleId == content.moduleId);
                        if (programContent != null)
                            content.ModuleTypeId = programContent.ModuleTypeId;
                        else
                            content.ModuleTypeId = 1;
                    }

                    content.Selectable = row.Selectable;
                    content.VariantSelectionGroupId = row.SelectionGroupId;
                }

                foreach (var g in variant.Groups.ToList())
                {
                    if (g.Contents != null && g.Contents.Count == 0)
                        db.VariantGroups.Remove(g);
                }

                db.SaveChanges();

                variant = VarinatApiHelper.VariantsQueryForApi(db).FirstOrDefault(v => v.Id == model.VariantId);
                variant.OnChanged();

                return RedirectToAction("VariantSaved", new { variantId = variant.Id, saved = true, message = string.Empty });

            }

            return RedirectToAction("VariantSaved", new { variantId = variant.Id, saved = false, message = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(r => r.ErrorMessage)) });
        }

        private static bool CanChangeVariant(Variant variant)
        {
            return variant.State == VariantState.Approved;
        }

        // GET: /Variant/Edit/5
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Variant variant = db.Variants.Find(id);
            if (variant == null)
            {
                return NotFound();
            }
            ViewBag.directionId = new SelectList(db.DirectionsForUser(User), "uid", "OksoAndTitle", variant.Program.directionId);
            return View(variant);
        }

        // POST: /Variant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Edit(/*[Bind(Include = "Id,Name,directionId,State")]*/ Variant variant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(variant).State = EntityState.Modified;
                db.SaveChanges();
                variant.OnChanged();
                return RedirectToAction("Index");
            }
            ViewBag.directionId = new SelectList(db.DirectionsForUser(User), "uid", "OksoAndTitle", variant.Program.directionId);
            return View(variant);
        }

        // GET: /Variant/Delete/5
   

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            Variant variant = db.Variants.Find(id);
            if (variant == null)
            {
                return NotFound();
            }
            var deleteVM = GetVariantDeleteVm(id, variant);

            return View(deleteVM);
            
        }

        private VariantDeleteVM GetVariantDeleteVm(int? id, Variant variant)
        {
            var planTeachers =
                db.PlanTeachers.Include(_ => _.Module)
                    .Where(_ => _.Variant.Id == id)
                    .Select(
                        _ =>
                            new PlanTeacherVM
                            {
                                catalogDisciplineUuid = _.catalogDisciplineUuid,
                                discipline = db.Disciplines.FirstOrDefault(d => d.uid.Contains(_.catalogDisciplineUuid)),
                                Module = _.Module,
                                Teacher = _.Teacher
                            }
                        //new PlanTeacherVM(_.Module, _.Teacher, _.catalogDisciplineUuid,
                        //    db.Disciplines.FirstOrDefault(d => d.uid.Contains(_.catalogDisciplineUuid)))
                    ).ToList();
            var deleteVM = new VariantDeleteVM(variant,
                db.VariantGroups.Where(_ => _.VariantId == id).ToList(),
                db.VariantSelectionGroups.Where(_ => _.VariantId == id).ToList(),
                db.EduProgramLimits.Where(_ => _.VariantId == id).ToList(),
                planTeachers,
                db.VariantAdmissions.Count(_ => _.Variant.Id == id && _.Status== AdmissionStatus.Admitted),
                db.StudentVariantSelections.Count(_ => _.Variant.Id == id),
                db.StudentSelectionPriority.Count(_ => _.variantId == id));
            return deleteVM;
        }

        // POST: /Variant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult DeleteConfirmed(int id)
        {
            Variant variant = db.Variants.Find(id);

            if (variant.State == VariantState.Approved)
                return RedirectToAction("Delete", new { id });
            var deleteVM = GetVariantDeleteVm(id, variant);
            
            if (deleteVM.CanDelete())
            {
                if (db.DropVariant(variant))
                    return RedirectToAction("Index");
            }
            
            return View(deleteVM);
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.VariantsChangeState)]
        public ActionResult ChangeState(int variantId, int state)
        {
            var variant = db.Variants.Find(variantId);
            var newState = (VariantState)state;

            if (newState == VariantState.Approved && variant.Program.State == VariantState.Approved && variant.Program.PlanNumber.HasValue && variant.Program.PlanVersionNumber != 0
                    || newState != VariantState.Approved)
            { 
                variant.State = (VariantState)state;
                db.SaveChanges();
                variant.OnChanged();
                return Json(new { result = true });
            }
            return Json(new { result = false, message = "Не утвержден выбор версии УП на версии ОП" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Download(int variantId)
        {
            Variant variant = VarinatApiHelper.VariantsQueryForApi(db).First(v => v.Id == variantId);
            Variant programVariant = VarinatApiHelper.VariantsQueryForApi(db).FirstOrDefault(v => v.Id == variant.Program.VariantId);
            var stream = new VariantExport().ExportVariant(new EditVariantContentViewModel(variant, programVariant));
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("{0}({1}).xlsx", variant.Name, variant.Program.Direction.OksoAndTitle).CleanFileName().ToDownloadFileName());
        }

        //private static string CleanFileName(string fileName)
        //{
        //    return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        //}

        public ActionResult Copy(int variantId, string name, string qualification, string divisionShortTitle,
            string chairShortTitle, string profileName, string familirizationType, string familirizationCondition, string year, int? page)
        {
            Variant variant = db.Variants.Find(variantId);
            int pageSize = 50;
            int pageNumber = (page ?? 1);


            ViewBag.name = name;
            ViewBag.qualification = qualification;
            ViewBag.divisionShortTitle = divisionShortTitle;
            ViewBag.chairShortTitle = chairShortTitle;
            ViewBag.profileName = profileName;
            ViewBag.familirizationType = familirizationType;
            ViewBag.familirizationCondition = familirizationCondition;
            ViewBag.year = year;
            ViewBag.id = variantId;
            ViewBag.Variant = variant;

            var eduprograms = db.EduProgramsForUser(User)
                .Where(v => v.directionId == variant.Program.directionId)
                .Where(v => v.qualification == variant.Program.qualification)
                .Where(v => v.divisionId == variant.Program.divisionId)
                .Where(v => v.familirizationType == variant.Program.familirizationType)
                .Where(v => v.familirizationCondition == variant.Program.familirizationCondition)
                .Include(e => e.Direction).Include(e => e.Division).Include(e => e.Profile);
            if (!string.IsNullOrWhiteSpace(name)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, DataContext.Profile>)eduprograms.Where(e => e.Name.Contains(name));
            if (!string.IsNullOrWhiteSpace(qualification)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, DataContext.Profile>)eduprograms.Where(e => e.qualification.Contains(qualification));
            if (!string.IsNullOrWhiteSpace(divisionShortTitle)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, DataContext.Profile>)eduprograms.Where(e => e.Division.shortTitle.Contains(divisionShortTitle));
            if (!string.IsNullOrWhiteSpace(chairShortTitle)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, DataContext.Profile>)eduprograms.Where(e => e.Chair.shortTitle.Contains(chairShortTitle));
            if (!string.IsNullOrWhiteSpace(profileName)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, DataContext.Profile>)eduprograms.Where(e => e.Profile.NAME.Contains(profileName));
            if (!string.IsNullOrWhiteSpace(familirizationType)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, DataContext.Profile>)eduprograms.Where(e => e.familirizationType.Contains(familirizationType));
            if (!string.IsNullOrWhiteSpace(familirizationCondition)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, DataContext.Profile>)eduprograms.Where(e => e.familirizationCondition.Contains(familirizationCondition));

            int numYear;
            if (int.TryParse(year, out numYear)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, DataContext.Profile>)eduprograms.Where(e => e.Year == numYear);

            return View(eduprograms.OrderBy(e => e.Year).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CopyExecute(int id, int programid)
        {
            Variant variant = db.Variants.Find(id);
            EduProgram program = db.EduPrograms.Find(programid);

            Variant dst = new Variant();
            db.CopyVariantData(variant, dst);
            dst.Name = "Копия " + dst.Name;
            program.Variants.Add(dst);
            db.SaveChanges();
            return RedirectToAction("BasicContentEdit", new { variantId = dst.Id });
        }


    }
}
