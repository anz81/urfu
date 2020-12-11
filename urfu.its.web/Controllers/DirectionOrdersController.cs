using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using System.Net;
using Newtonsoft.Json;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Urfu.Its.Web.Models;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class DirectionOrdersController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var filterRules = FilterRules.Deserialize(filter);

                var orders = db.DirectionOrders.ToList().Select(o => new
                {
                    o.Id,
                    o.Number,
                    Date = o.Date.HasValue ? o.Date.Value.ToShortDateString() : "",
                    o.DirectionId,
                    Direction = o.Direction.OksoAndTitle
                }).AsQueryable()
                .Where(filterRules).OrderBy(a => a.Direction);
                
                return Json(
                     new
                     {
                         data = orders
                     },
                     new JsonSerializerSettings()
                 );
            }

            var directions = db.DirectionsForUser(User).Where(d => d.standard == "ФГОС ВО 3++").ToList()
                .Select(d => new
                {
                    Id = d.uid,
                    Name = d.OksoAndTitle
                })
                .OrderBy(d => d.Name);
            ViewBag.Directions = JsonConvert.SerializeObject(directions);

            ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Create([ExcludeBind(nameof(DirectionOrder.Id))] DirectionOrder order)
        {
            if (ModelState.IsValid)
            {
                if (db.DirectionOrders.Any(d => d.DirectionId == order.DirectionId))
                    return Json(new { success = false, message = $"Приказ на указанное направление уже существует" });//, "text/html", Encoding.Unicode);

                if (string.IsNullOrWhiteSpace(order.DirectionId) || string.IsNullOrWhiteSpace(order.Number)
                    || !order.Date.HasValue)
                    return Json(new { success = false, message = $"Все поля должны быть заполнены" });//, "text/html", Encoding.Unicode);

                db.DirectionOrders.Add(order);
                db.SaveChanges();
                return Json(new { success = true });//, "text/html", Encoding.Unicode);
            }

            return Json(new { success = false, message = "" });
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Edit(DirectionOrder order)
        {
            if (ModelState.IsValid)
            {
                if (!db.DirectionOrders.Any(d => d.Id == order.Id))
                    return Json(new { success = false, message = $"Не найдена запись для редактирования" });

                if (string.IsNullOrWhiteSpace(order.DirectionId) || string.IsNullOrWhiteSpace(order.Number)
                    || !order.Date.HasValue)
                    return Json(new { success = false, message = $"Все поля должны быть заполнены" });//, "text/html", Encoding.Unicode);

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true });
            }
            return Json(new { success = false, message = "" });
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Remove(int id)
        {
            try
            {
                var order = db.DirectionOrders.FirstOrDefault(d => d.Id == id);
                db.DirectionOrders.Remove(order);
                db.SaveChanges();

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }
    }
}