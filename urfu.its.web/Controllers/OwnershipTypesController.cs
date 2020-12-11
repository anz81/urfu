using Ext.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using PagedList.Core;
using Ext.Utilities.Linq;
using Urfu.Its.Web.Model.Models.Practice;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.PracticeManager)]
    public class OwnershipTypesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                SortRules sortRules = SortRules.Deserialize(sort);
                var types = db.OwnershipTypes.Where(FilterRules.Deserialize(filter)).OrderByThenBy(sortRules.FirstOrDefault(), v => v.FullName);

                var paginated = types.ToPagedList(page ?? 1, limit ?? 25);

                return Json(
                    new
                    {
                        data = paginated,
                        total = types.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                ViewBag.Focus = focus;
                return View();
            }
        }

        public ActionResult EditOwnershipType(int id, string name, string shortName)
        {
            var existType = db.OwnershipTypes.FirstOrDefault(l => l.FullName == name && l.ShortName == shortName && l.Id != id);
            if (existType == null)
            {
                if (id != 0)
                {
                    // уже существующая форма собственности
                    var type = db.OwnershipTypes.FirstOrDefault(l => l.Id == id);
                    if (type != null)
                    {
                        type.FullName = name;
                        type.ShortName = shortName;
                        db.SaveChanges();
                        return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Редактируемая форма собственности не найдена" });//, "text/html", Encoding.Unicode);
                    }
                }
                else
                {
                    // новая форма собственности
                    db.OwnershipTypes.Add(new OwnershipTypes()
                    {
                        FullName = name,
                        ShortName = shortName
                    });
                    db.SaveChanges();
                    return Json(new { success = true, message = "" }); //, "text/html", Encoding.Unicode);
                }
            }
            else
            {
                return Json(new { success = false, message = "Такая форма собственности уже существует" });//, "text/html", Encoding.Unicode);
            }
        }

        public ActionResult RemoveOwnershipType(int id)
        {
            var type = db.OwnershipTypes.FirstOrDefault(t => t.Id == id);
            if (type != null)
            {
                var company = db.PracticeCompanies().Where(c => c.OwnershipType != null).FirstOrDefault(c => c.OwnershipTypeId == id);

                if (company == null)
                {
                    db.OwnershipTypes.Remove(type);
                    db.SaveChanges();
                    return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                }
                else
                {
                    return Json(new { success = false,
                        message = "Форма собственности не может быть удалена, так как есть предприятие с этой формой собственности" });//, "text/html", Encoding.Unicode);
                }
            }
            else
            {
                return Json(new { success = false, message = "Удаляемая форма собственности не найдена" });//, "text/html", Encoding.Unicode);
            }
        }
    }
}