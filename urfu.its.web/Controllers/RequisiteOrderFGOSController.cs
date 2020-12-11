using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.WorkingProgramManager)]
    public class RequisiteOrderFgosController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

       
        public ActionResult Index(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var sortRules = SortRules.Deserialize(sort);
                var userDirections = _db.DirectionsForUser(User);

                var orders = _db.RequisiteOrderFgoss
                    .Where(o => userDirections.Contains(o.Direction))
                    .Select(o => new
                    {
                        id = o.Id,
                        order = o.Order,
                        directionId = o.DirectionId,
                        directionOkso = o.Direction.okso,
                        directionName = o.Direction.title,
                        date = o.Date
                    }).OrderBy(sortRules.FirstOrDefault(), m => m.id)
                    .Where(FilterRules.Deserialize(filter))
                    .ToList()
                    .Select(o => new
                    {
                        o.id,
                        o.order,
                        o.directionId,
                        o.directionOkso,
                        o.directionName,
                        date = o.date.ToShortDateString()
                    });

                return Json(orders, new JsonSerializerSettings());
            }

            return View();
        }

        public ActionResult Create([ExcludeBind(nameof(RequisiteOrderFGOS.Id))]RequisiteOrderFGOS requisiteOrderFgos)
        {
            if (ModelState.IsValid)
            {
                _db.RequisiteOrderFgoss.Add(requisiteOrderFgos);
                _db.SaveChanges();
                return Json(new { success = true });

            }
            return Json(new { success = false });

        }
        public ActionResult Edit(RequisiteOrderFGOS requisiteOrderFgos)
        {
            if (ModelState.IsValid)
            {
                var orderFgos = _db.RequisiteOrderFgoss.FirstOrDefault(_ => _.Id == requisiteOrderFgos.Id);
                if (orderFgos != null)
                {
                    orderFgos.DirectionId = requisiteOrderFgos.DirectionId;
                    orderFgos.Order = requisiteOrderFgos.Order;
                    orderFgos.Date = requisiteOrderFgos.Date;
                    _db.SaveChanges();
                    return Json(new { success = true });
                }

            }
            return Json(new { success = false });

        }

        public ActionResult Delete(int id)
        {
            var orderFgos = _db.RequisiteOrderFgoss.FirstOrDefault(_ => _.Id == id);
            if (orderFgos != null)
            {
                _db.RequisiteOrderFgoss.Remove(orderFgos);
                _db.SaveChanges();
                return Json(new { success = true });

            }
            return Json(new { success = false });

        }

    }
}