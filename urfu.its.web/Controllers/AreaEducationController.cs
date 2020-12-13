using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using System.Net;
using Newtonsoft.Json;
using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList;
using Urfu.Its.Web.Models;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class AreaEducationController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var sortRules = SortRules.Deserialize(sort);
                
                var areas = db.AreaEducations.ToList().Select(a => new
                {
                    a.Id,
                    a.Code,
                    a.Title,
                    Orders = a.Orders.Select(o => new
                    {
                        OrderId = o.Id,
                        Date = o.Date.HasValue ? o.Date.Value.ToShortDateString() : "",
                        o.Number,
                        Qualification = o.QualificationName
                    }),
                    OrdersStr = string.Join(", ", a.Orders.Select(o => o.Date.HasValue ? $"{o.Number} от {o.Date.Value.ToShortDateString()} {o.QualificationName}" : $"{o.Number} {o.QualificationName}"))
                }).OrderBy(a => a.Code).AsQueryable();

                if (sortRules?.Count > 0)
                {
                    var sortRule = sortRules[0];
                    areas = areas.OrderBy(sortRule);
                }

                return Json(
                     new
                     {
                         data = areas,
                         total = areas.Count()
                     },
                     new JsonSerializerSettings()
                 );
            }
            else
            {
                var qualifications = db.Qualifications.ToList();
                ViewBag.Qualifications = JsonConvert.SerializeObject(qualifications);

                ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
                return View();
            }
        }

        public ActionResult Update(int id, string code, string title)
        {
            try
            {
                var areaEducation = db.AreaEducations.FirstOrDefault(d => d.Id == id);

                if (areaEducation == null)
                {
                    areaEducation = new AreaEducation();
                    db.AreaEducations.Add(areaEducation);
                }
                areaEducation.Code = code;
                areaEducation.Title = title;

                db.SaveChanges();
                return Json(new { success = true, message = "Область образования успешно добавлена" });//, "text/html", Encoding.Unicode);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }
        
        public ActionResult Remove(int id)
        {
            try
            {
                var areaEducation = db.AreaEducations.FirstOrDefault(d => d.Id == id);
                db.AreaEducations.Remove(areaEducation);
                db.SaveChanges();

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        [HttpPost]
        public ActionResult UpdateOrders(int id, List<AreaEducationOrder> orders)
        {
            try
            {
                orders = orders ?? new List<AreaEducationOrder>();
                foreach (var p in orders)
                    if (!db.AreaEducationOrders.Any(d => d.Id == p.Id))
                    {
                        var d = new AreaEducationOrder();
                        d.Id = p.Id;
                        db.AreaEducationOrders.Add(d);
                    }
               // db.AreaEducationOrders.AddOrUpdate(a => a.Id, orders.ToArray());
               
                db.SaveChanges();

                var ids = orders.Select(o => o.Id);
                db.AreaEducationOrders.RemoveRange(db.AreaEducationOrders.Where(a => a.AreaEducationId == id && !ids.Contains(a.Id)));
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