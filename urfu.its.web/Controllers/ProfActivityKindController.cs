using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;
//using WebGrease.Css.Extensions;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class ProfActivityKindController : Controller
    {

        readonly ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var profactkinds = db.ProfActivityKinds.AsEnumerable();

                return Json(
                    new
                    {
                        data = profactkinds
                    },
                    new JsonSerializerSettings()
                );
            }

            ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
            return View();
        }

        public ActionResult Create(ProfActivityKind profkind)
        {
            if (ModelState.IsValid)
            {
                if (db.ProfActivityKinds.Any(k => k.Code.Trim().Equals(profkind.Code.Trim())))
                    return Json(new { success = false, message = $"Существует вид профессиональной деятельности с кодом {profkind.Code}" });

                if(!db.ProfActivityAreas.Where(a => profkind.Code.StartsWith(a.Code)).AsEnumerable()
                    .Any(a => IsValidCode(profkind.Code, a.Code)))
                    return Json(new { success = false, message = $"<br>Неверный формат кода, либо отсутствует область профессиональной деятельности" });

                db.ProfActivityKinds.Add(profkind);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        private static bool IsValidCode(string code, string area)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;
            try
            {
                string pattern = @"\b" + area + @"\b\.\d{3,}";
                return Regex.IsMatch(code, pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

        }


        public ActionResult Edit(ProfActivityKind profkind)
        {
            if (ModelState.IsValid)
            {
                if (!db.ProfActivityKinds.Any(k => k.Code.Trim().Equals(profkind.Code.Trim())))
                    return Json(new { success = false, message = $"Не найдена запись для редактирования" });

                db.Entry(profkind).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult Delete(string code)
        {
            var profkKind = db.ProfActivityKinds.FirstOrDefault(a => a.Code.Trim() == code.Trim());

            if (profkKind != null)
            {
                if (db.ProfStandards.Any(s => s.ProfActivityKindCode.Trim().Equals(profkKind.Code)))
                    return Json(new { success = false, message = $"Удаление невозможно.<br> Существуют профессиональные стандарты,соответствующее виду професcиональной деятельности с кодом {code}" }, new JsonSerializerSettings());

                db.ProfActivityKinds.Remove(profkKind);
                db.SaveChanges();
                Logger.Info($"Удаление вида профессиональной деятельности-{profkKind.Code}");
                return Json(new { success = true }, new JsonSerializerSettings());
            }
            return Json(new { success = false, message = $"Не найден  вид профессиональной деятельности с кодом'{code}'" }, new JsonSerializerSettings());
        }



    }
}