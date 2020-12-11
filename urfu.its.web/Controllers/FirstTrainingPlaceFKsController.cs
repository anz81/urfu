using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.SectionFKManager)]
    public class FirstTrainingPlaceFKsController : BaseController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: FirstTrainingPlaceFKs
        public ActionResult Index(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var eduprograms =
                    _db.FirstTrainingPlaceFKs.Select(p => new {p.Id, p.Address, p.Description});

                var sortRules = SortRules.Deserialize(sort);
                eduprograms = eduprograms.OrderBy(sortRules.FirstOrDefault(), v => v.Address);

                eduprograms = eduprograms.Where(FilterRules.Deserialize(filter));

                var paginated = eduprograms.ToPagedList(page ?? 1, limit ?? 25);
                return JsonNet(new
                {
                    data = paginated,
                    total = eduprograms.Count()
                });
            }
            return View();
        }

        // GET: FirstTrainingPlaceFKs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            var firstTrainingPlaceFK = _db.FirstTrainingPlaceFKs.Find(id);
            if (firstTrainingPlaceFK == null)
                return NotFound();
            return View(firstTrainingPlaceFK);
        }

        // GET: FirstTrainingPlaceFKs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FirstTrainingPlaceFKs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "Id,Address,Description")]*/ FirstTrainingPlaceFK firstTrainingPlaceFK)
        {
            if (ModelState.IsValid)
            {
                _db.FirstTrainingPlaceFKs.Add(firstTrainingPlaceFK);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(firstTrainingPlaceFK);
        }

        // GET: FirstTrainingPlaceFKs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            var firstTrainingPlaceFK = _db.FirstTrainingPlaceFKs.Find(id);
            if (firstTrainingPlaceFK == null)
                return NotFound();
            return View(firstTrainingPlaceFK);
        }

        // POST: FirstTrainingPlaceFKs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include = "Id,Address,Description")] */FirstTrainingPlaceFK firstTrainingPlaceFK)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(firstTrainingPlaceFK).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(firstTrainingPlaceFK);
        }

        // GET: FirstTrainingPlaceFKs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            var firstTrainingPlaceFK = _db.FirstTrainingPlaceFKs.Find(id);
            if (firstTrainingPlaceFK == null)
                return NotFound();
            return View(firstTrainingPlaceFK);
        }

        // POST: FirstTrainingPlaceFKs/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var firstTrainingPlaceFK = _db.FirstTrainingPlaceFKs.Find(id);
            _db.FirstTrainingPlaceFKs.Remove(firstTrainingPlaceFK);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }
}