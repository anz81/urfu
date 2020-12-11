using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
//using System.Web.UI.WebControls;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Urfu.Its.Web.DataContext;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    public class PracticeWayController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: PracticeWay
        public ActionResult Index(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var sortRules = SortRules.Deserialize(sort);
                var practiceWays = _db.PracticeWays.OrderBy(sortRules.FirstOrDefault(), m => m.Id)
                    .Where(FilterRules.Deserialize(filter)).ToList();

                return Json(practiceWays, new JsonSerializerSettings());
            }

            return View();
        }

        public ActionResult Create([ExcludeBind(nameof(PracticeWay.Id))]PracticeWay practiceWay)
        {
            if (ModelState.IsValid)
            {
                _db.PracticeWays.Add(practiceWay);
                _db.SaveChanges();
                return Json(new { success = true });

            }
            return Json(new { success = false });

        }
        public ActionResult Edit(PracticeWay practiceWay)
        {
            if (ModelState.IsValid)
            {
                var practiceWayE = _db.PracticeWays.FirstOrDefault(_ => _.Id == practiceWay.Id);
                if (practiceWayE != null)
                {
                    practiceWayE.Description = practiceWay.Description;
                    //TODO: проверка на уникальность
                    _db.SaveChanges();
                    return Json(new { success = true });
                }

            }
            return Json(new { success = false });

        }

        public ActionResult Delete(int id)
        {
            var practiceWay = _db.PracticeWays.FirstOrDefault(_ => _.Id == id);
            if (practiceWay != null)
            {
                _db.PracticeWays.Remove(practiceWay);
                _db.SaveChanges();
                return Json(new { success = true });

            }
            return Json(new { success = false });

        }
    }
}