using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)] 
    public class ProfActivityAreaController : Controller
    {
        readonly ApplicationDbContext db = new ApplicationDbContext();
 
        public ActionResult Index()
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var profarea = db.ProfActivityAreas.AsEnumerable();

                return Json(
                    new
                    {
                        data = profarea
                    },
                    new JsonSerializerSettings()
                ); 
            }

            ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
            return View();
        }

        public ActionResult Edit(ProfActivityArea ProfArea)
        {
            if (ModelState.IsValid)
            {
                var profarea = db.ProfActivityAreas.FirstOrDefault(a => a.Code.Trim() == ProfArea.Code.Trim());
                if (profarea == null)
                    return Json(new { success = false, message = $"Не найдена запись с кодом'{ProfArea.Code}'" });

                profarea.Title = ProfArea.Title;
                db.SaveChanges();

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult Create(ProfActivityArea ProfArea)
        {
            if (ModelState.IsValid)
            {
               var existprofarea  =db.ProfActivityAreas.FirstOrDefault(a => a.Code.Trim() == ProfArea.Code.Trim());
                if(existprofarea != null)
                    return Json(new { success = false,message = $"Уже существует область профессиональной деятельности с кодом  {ProfArea.Code}" });

                db.ProfActivityAreas.Add(ProfArea);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult Delete(string code)
        {
            var profarea = db.ProfActivityAreas.FirstOrDefault(a => a.Code.Trim() == code.Trim());

            if (profarea != null)
            {
                var existprofstandads = db.ProfStandards.Where(s => s.ProfActivityAreaCode.Equals(profarea.Code)).ToList();

                if (existprofstandads.Count()!=0)
                      return Json(new { success = false, message = $"Удаление невозможно.<br> Существуют профессиональные стандарты,соответствующее области професcиональной деятельности с кодом {code}" }, new JsonSerializerSettings());

                db.ProfActivityAreas.Remove(profarea);
                db.SaveChanges();
                Logger.Info($"Удаление области профессиональной деятельности-{profarea.Code}");
                return Json(new { success = true }, new JsonSerializerSettings());
            }
            return Json(new { success = false, message = $"Не найдена  область профессиональной деятельности с кодом'{code}'" });
        }


    }
}