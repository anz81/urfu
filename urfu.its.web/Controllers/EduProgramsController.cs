using System;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
//using Microsoft.Ajax.Utilities;
//using OfficeOpenXml.FormulaParsing.Utilities;
using PagedList;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
//using Ext.Utilities;
//using Ext.Utilities.Linq;
using Urfu.Its.Web.Model.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ext.Utilities;
using Ext.Utilities.Linq;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.VariantsView)]
    public class EduProgramsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /EduPrograms/
        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {

                var eduprograms =
                    db.EduProgramsForUser(User)
                        .Include(e => e.Direction)
                        .Include(e => e.Division)
                        .Include(e => e.Profile)
                        .Select(v => new
                        {
                            v.Id,
                            DirectionOkso = v.Direction.okso,
                            DirectionTitle = v.Direction.title +" (" + v.Direction.standard +")",
                            v.Name,
                            v.HeadFullName,
                            v.qualification,
                            DivisionTitle = v.Division.shortTitle,
                            ChairTitle = v.Chair.shortTitle,
                            Profile = v.Profile.NAME,
                            v.familirizationType,
                            v.familirizationCondition,
                            v.Year,
                            State =
                                v.Variant.State == VariantState.Approved
                                    ? "Утверждена"
                                    : v.Variant.State == VariantState.Review ? "На согласовании" : "Формируется"
                        });

                SortRules sortRules = SortRules.Deserialize(sort);
                eduprograms = eduprograms.OrderBy(sortRules.FirstOrDefault(), v => v.DirectionOkso);

                eduprograms = eduprograms.Where(FilterRules.Deserialize(filter));

                var paginated = eduprograms.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = eduprograms.Count()
                });
            }
            else
            {
                ViewBag.Focus = focus;
                return View();
            }
        }

        // GET: /EduPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            EduProgram eduprogram = db.EduPrograms.Find(id);
            if (eduprogram == null)
            {
                return NotFound();
            }
            return View(eduprogram);
        }

        // GET: /EduPrograms/Create
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Create()
        {
            ViewBag.directionId = new SelectList(db.DirectionsForUser(User), "uid", "OksoAndTitle");
            ViewBag.divisionId = new SelectList(Enumerable.Empty<Division>(), "uuid", "shortTitle");
            ViewBag.profileId = new SelectList(Enumerable.Empty<Profile>(), "ID", "CODE");

            var fTypes = db.FamilirizationTypes.Select(q => q.Name);
            ViewBag.familirizationType = new SelectList(fTypes, null, null, "Очная");
            
            var fCond = db.FamilirizationConditions.Select(q => q.Name);
            ViewBag.familirizationCondition = new SelectList(fCond, null, null, fCond.FirstOrDefault());

            var qualifications = db.Qualifications.Select(q => q.Name);
            ViewBag.qualification = new SelectList(qualifications, null, null, qualifications.FirstOrDefault());
            return View();
        }

        // POST: /EduPrograms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Create(/*[Bind(Include = "Id,Name,directionId,qualification,Year,divisionId,profileId,IsNetwork")]*/ EduProgram eduprogram)
        {
            if (ModelState.IsValid)
            {
                db.EduPrograms.Add(eduprogram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var qualifications = db.Qualifications.Select(q => q.Name);
            ViewBag.qualification = new SelectList(qualifications, null, null, qualifications.FirstOrDefault());
            ViewBag.divisionId = new SelectList(Enumerable.Empty<Division>(), "uuid", "shortTitle");
            ViewBag.profileId = new SelectList(Enumerable.Empty<Profile>(), "ID", "CODE");
            ViewBag.directionId = new SelectList(db.DirectionsForUser(User), "uid", "OksoAndTitle", eduprogram.directionId);


            var fTypes = db.FamilirizationTypes.Select(q => q.Name);
            ViewBag.familirizationType = new SelectList(fTypes, null, null, "Очная");
            var fCond = db.FamilirizationConditions.Select(q => q.Name);
            ViewBag.familirizationCondition = new SelectList(fCond, null, null, fCond.FirstOrDefault());

            return View(eduprogram);
        }

        // GET: /EduPrograms/Edit/5
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            EduProgram eduprogram = db.EduProgramsForUser(User).FirstOrDefault(p=>p.Id==id);
            if (eduprogram == null)
            {
                return NotFound();
            }
            ViewBag.PlanVersionNumber = new SelectList(GetPlansQuery(id).Where(p=>p.eduplanNumber== eduprogram.PlanNumber).Select(p=>p.versionNumber).Distinct().OrderBy(x=>x), null, null, eduprogram.PlanVersionNumber);
            ViewBag.PlanNumber = new SelectList(GetPlansQuery(id).Select(p=>p.eduplanNumber).Distinct().OrderBy(x=>x), null, null, eduprogram.PlanNumber);
            var divisions = db.Divisions.Where(d=>d.parent==eduprogram.divisionId)
                .Union(db.Divisions.Where(d => db.Divisions.Where(d1 => d1.parent == eduprogram.divisionId).Any(d1=>d.parent==d1.uuid)))
                .Union(db.Divisions.Where(d => d.uuid == eduprogram.chairId))
                .Union(db.Divisions.Where(d => d.uuid == eduprogram.departmentId))
                .Union(db.Divisions.Where(d => d.uuid == eduprogram.divisionId))
                .Union(db.Divisions.Where(d => d.uuid == eduprogram.divisionId).SelectMany(d=>db.Divisions.Where(dx=>dx.uuid==d.parent)))
                .ToList();
            ViewBag.chairId = new SelectList(divisions, "uuid", "shortTitle", eduprogram.chairId);
            ViewBag.departmentId = new SelectList(divisions, "uuid", "shortTitle", eduprogram.departmentId);
            ViewBag.divisionId = new SelectList(divisions, "uuid", "shortTitle", eduprogram.divisionId);
            return View(eduprogram);
        }

        public IQueryable<Plan> GetPlansQuery(int? id)
        {
            var program = db.EduPrograms.Find(id);
            return db.Plans.Where(p => p.qualification == program.qualification &&
                                p.familirizationType == program.familirizationType &&
                                p.familirizationCondition == program.familirizationCondition &&
                                p.directionId == program.directionId &&
                                p.active &&
                                (p.faculty == program.divisionId || p.faculty == program.departmentId || p.faculty == program.chairId) /*&& p.Module.UsedInVariantContents.Any(vc => vc.Group.Variant.EduProgramId == id && vc.Selected)*/);
        }

        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult EditVariant(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            EduProgram eduprogram = db.EduProgramsForUser(User).FirstOrDefault(p=>p.Id==id);
            if (eduprogram == null)
            {
                return NotFound();
            }
            eduprogram.CheckProgramVariant(db);
            return RedirectToAction("BasicContentEdit", "Variant", new { eduprogram.VariantId });
        }

        // POST: /EduPrograms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Edit(EduProgram eduprogram)
        {
            var dbEntry = db.EduPrograms.Find(eduprogram.Id);
            if ((ModelState["PlanVersionNumber"]).Errors.Count > 0)
            {
                if(dbEntry.chairId != eduprogram.chairId ||
                dbEntry.departmentId != eduprogram.departmentId||
                dbEntry.divisionId != eduprogram.divisionId)
                    (ModelState["PlanVersionNumber"]).Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                dbEntry.State = eduprogram.State;
                dbEntry.Name = eduprogram.Name;
                dbEntry.PlanVersionNumber = eduprogram.PlanVersionNumber;
                dbEntry.PlanNumber = eduprogram.PlanNumber;
                dbEntry.IsNetwork = eduprogram.IsNetwork;
                dbEntry.HeadFullName = eduprogram.HeadFullName;
                if (User.IsInRole(ItsRoles.Admin))
                {
                    dbEntry.chairId = eduprogram.chairId;
                    dbEntry.departmentId = eduprogram.departmentId;
                    dbEntry.divisionId = eduprogram.divisionId;
                }
                db.SaveChanges();
                dbEntry.OnChanged();
                return RedirectToAction("Index", new { focus = dbEntry.Id });
            }


            var divisions = db.Divisions.Where(d => d.parent == eduprogram.divisionId)
                .Union(db.Divisions.Where(d => db.Divisions.Where(d1 => d1.parent == eduprogram.divisionId).Any(d1 => d.parent == d1.uuid)))
                .Union(db.Divisions.Where(d => d.uuid == eduprogram.chairId))
                .Union(db.Divisions.Where(d => d.uuid == eduprogram.departmentId))
                .Union(db.Divisions.Where(d => d.uuid == eduprogram.divisionId))
                .ToList();
            ViewBag.chairId = new SelectList(divisions, "uuid", "shortTitle", eduprogram.chairId);
            ViewBag.departmentId = new SelectList(divisions, "uuid", "shortTitle", eduprogram.departmentId);
            ViewBag.divisionId = new SelectList(divisions, "uuid", "shortTitle", eduprogram.divisionId);

            ViewBag.PlanVersionNumber = new SelectList(GetPlansQuery(eduprogram.Id).Select(p => p.versionNumber).Distinct().OrderBy(x => x), null, null, eduprogram.PlanVersionNumber);
            ViewBag.PlanNumber = new SelectList(GetPlansQuery(eduprogram.Id).Select(p => p.eduplanNumber).Distinct().OrderBy(x => x), null, null, eduprogram.PlanNumber);
            return View(dbEntry);
        }

        // GET: /EduPrograms/Delete/5
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            EduProgram eduprogram = db.EduProgramsForUser(User).FirstOrDefault(p => p.Id == id);
            if (eduprogram == null)
            {
                return NotFound();
            }
            
            return View(eduprogram);
        }

        // POST: /EduPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult DeleteConfirmed(int id)
        {

            EduProgram eduprogram = db.EduPrograms.Find(id);
            var otherVariants = db.Variants.Where(_ => !_.IsBase && _.EduProgramId == id).ToList();
            if (otherVariants.Count == 0)
            {
                if (db.DropVariant(eduprogram.Variant))
                {
                    db.EduPrograms.Remove(eduprogram);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

            }
            ViewBag.msg = "На этой версии ОП существуют траектории";
            ViewBag.otherVariants = otherVariants;
            return Delete(id);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Copy(int id)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            EduProgram eduprogram = db.EduPrograms.Find(id);
            if (eduprogram == null)
            {
                return NotFound();
            }
            return View(eduprogram);
        }

        [HttpPost]
        public ActionResult Copy(int id, int year)
        {
            if (id == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            EduProgram eduprogram = db.EduPrograms.Find(id);

            if (eduprogram == null)
            {
                return NotFound();
            }


            var result = db.CopyProgramAndSave(eduprogram,year);

            return RedirectToAction("EditVariant", new { id = result.Id });
        }


        public ActionResult CopyAdmissions(int programId, string name, string qualification, string divisionShortTitle,
            string chairShortTitle, string profileName, string familirizationType, string familirizationCondition, string year, int? page)
        {
            var program = db.EduPrograms.Find(programId);
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
            ViewBag.id = programId;
            ViewBag.Program = program;

            var eduprograms = db.EduProgramsForUser(User)
                .Where(v => v.directionId == program.directionId)
                .Where(v => v.qualification == program.qualification)
                .Where(v => v.divisionId == program.divisionId)
                .Where(v => v.familirizationType == program.familirizationType)
                .Where(v => v.familirizationCondition == program.familirizationCondition)
                .Include(e => e.Direction).Include(e => e.Division).Include(e => e.Profile);
            if (!String.IsNullOrWhiteSpace(name)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, Profile>)eduprograms.Where(e => e.Name.Contains(name));
            if (!String.IsNullOrWhiteSpace(qualification)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, Profile>)eduprograms.Where(e => e.qualification.Contains(qualification));
            if (!String.IsNullOrWhiteSpace(divisionShortTitle)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, Profile>)eduprograms.Where(e => e.Division.shortTitle.Contains(divisionShortTitle));
            if (!String.IsNullOrWhiteSpace(chairShortTitle)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, Profile>)eduprograms.Where(e => e.Chair.shortTitle.Contains(chairShortTitle));
            if (!String.IsNullOrWhiteSpace(profileName)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, Profile>)eduprograms.Where(e => e.Profile.NAME.Contains(profileName));
            if (!String.IsNullOrWhiteSpace(familirizationType)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, Profile>)eduprograms.Where(e => e.familirizationType.Contains(familirizationType));
            if (!String.IsNullOrWhiteSpace(familirizationCondition)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, Profile>)eduprograms.Where(e => e.familirizationCondition.Contains(familirizationCondition));

            int numYear;
            if (int.TryParse(year, out numYear)) eduprograms = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EduProgram, Profile>)eduprograms.Where(e => e.Year == numYear);

            return View(eduprograms.OrderBy(e => e.Year).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CopyExecute(int srcid, int dstid)
        {
            var pairs = db.Variants
                .Where(v => v.EduProgramId == srcid)
                .Select(v => new {src = v, dst = db.Variants.FirstOrDefault(vx => vx.EduProgramId == dstid && v.Name==vx.Name)})
                .Where(p =>  !p.src.IsBase && !p.dst.IsBase)
                .Select(p=>new {srcId = p.src.Id, dstId = p.dst.Id})
                .ToList();

            foreach (var p in pairs)
            {
                foreach (var sva in db.VariantAdmissions.Where(v=>v.variantId==p.srcId).ToList())
                {
                    db.VariantAdmissions.Add(new VariantAdmission
                    {
                        variantId = p.dstId,
                        Status = sva.Status,
                        studentId = sva.studentId,
                        Published = false,
                    });
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
