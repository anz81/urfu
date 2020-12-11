using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.Models;
using Urfu.Its.Web.DataContext;
using Ext.Utilities;
using Ext.Utilities.Linq;
using System.Text.RegularExpressions;
using Urfu.Its.Common;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class ProfStandardsController : Controller
    {
        readonly ApplicationDbContext db = new ApplicationDbContext();
       

        public ActionResult Index(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var filterRules = FilterRules.Deserialize(filter);
                var sortRules = SortRules.Deserialize(sort);

                var profstandards = db.ProfStandards.Select(s => new
                {
                     s.Code,
                     s.Title,
                     ProfAreaCode= s.ProfActivityArea.Code,
                     ProfArea = s.ProfActivityArea.Code + " - " + s.ProfActivityArea.Title,
                     ProfKind=s.ProfActivityKind.Code + " - " + s.ProfActivityKind.Title,
                    s.ProfActivityKindCode
                }).Where(filterRules).OrderBy(s=>s.Code).AsQueryable();

                if(sortRules.Count >0)
                {
                    var sortRule = sortRules[0];
                    profstandards = profstandards.OrderBy(sortRule);
                }

                return Json(
                    new
                    {
                        data = profstandards
                    },
                    new JsonSerializerSettings()
                );                     
            }
            ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
            return View();
        }

         public ActionResult GetProfActivityArea()
        {
            
                var profarea = db.ProfActivityAreas.Select(a => new {ProfArea = a.Code, ProfAreaTitle = a.Code + " - " + a.Title}).ToList();
                return Json(profarea, new JsonSerializerSettings());
        }

         [HttpGet]
        public ActionResult GetProfStandards(string areaCode)
        {

            var profStandards = db.ProfStandards.Where(p => p.ProfActivityAreaCode == areaCode)
                .Select(p=>new{ Code =p.Code, Title =p.Code+ " - " + p.Title})
                .ToList();
            return Json(
                new
                {
                    data = profStandards
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult GetProfAktivityKinds(string areaCode, bool newprofactivitykinds)
        {
            var area = db.ProfActivityAreas.FirstOrDefault(a => a.Code.Trim() == areaCode.Trim());
            if (area == null)
                 return new StatusCodeResult(StatusCodes.Status404NotFound);

            var profactivkind = db.ProfActivityKinds.Where(k => k.Code.StartsWith(areaCode)).AsEnumerable().Where(k => IsValidCode(k.Code, areaCode)).ToList();

            if (newprofactivitykinds)
            {
                profactivkind = profactivkind.Except(db.ProfStandards.Where(s => s.ProfActivityKindCode.StartsWith(areaCode)).AsEnumerable()
                    .Where(s => IsValidCode(s.Code, areaCode)).Select(s => s.ProfActivityKind)).ToList();
            }

            var result = profactivkind.Select(k => new {ProfActivityKindCode = k.Code, Title = k.Code + " - " + k.Title});

            return Json(
                new
                {
                    data = result
                },
                new JsonSerializerSettings()
            );
        }



        [HttpPost]
        public ActionResult Create(ProfStandard standard)
        {
            standard.Code = standard.ProfActivityKindCode;
                var profstandard = db.ProfStandards.FirstOrDefault(s => s.Code.Trim() == standard.Code.Trim());

                if (profstandard != null)
                    return Json(new { success = false, message = $"Уже существует стандарт с кодом  {standard.Code}" });
                if (!IsValidCode(standard.Code, standard.ProfActivityAreaCode))
                    return Json(new { success = false, message = "Код стандарта не соответствует области образования"});

                db.ProfStandards.Add(standard);
                db.SaveChanges();
                return Json(new { success = true });
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
            catch(ArgumentException)
            {
                return false;
            }

        }


        [HttpPost]
        public ActionResult Edit(ProfStandard standard)
        {
               standard.Code = standard.ProfActivityKindCode;
               var prstandard = db.ProfStandards.FirstOrDefault(s => s.Code == standard.Code);
                if (prstandard == null)
                    return Json(new { success = false, message = $"Не найдена запись'{standard.Code}'" });
                if (!IsValidCode(standard.Code, standard.ProfActivityAreaCode) || !standard.Code.Equals(standard.ProfActivityKindCode))
                    return Json(new { success = false, message = "Код стандарта не соответствует области профессиональной деятельности" });
                prstandard.Title = standard.Title;
                db.SaveChanges();
                  Logger.Info($"Редактирование профессионального стандарта:{prstandard.Code}");
                 return Json(new { success = true });
            
        }

        [HttpGet]
        public ActionResult Delete(string code)
        {
            var standard = db.ProfStandards.FirstOrDefault(s => s.Code.Trim() == code.Trim());
            if (standard != null)
            {
                var existproforder = db.ProfOrders.Where(p=>p.ProfStandardCode.Equals(standard.Code));
                 if (existproforder.Count() !=0)
                     return Json(new { success = false, message = $"Удаление невозможно! <br>Существуют приказы,связанные с кодом проф.стандарта " }, new JsonSerializerSettings());

                 db.ProfStandards.Remove(standard);
                db.SaveChanges();
                Logger.Info($"Удаление профессионального стандарта-{standard.Code}");
                return Json(new {success = true}, new JsonSerializerSettings());
            }
            return Json(new { success = false, message = $"Не найден  профессиональный стандарт с кодом'{code}'" });
        }

    }
}