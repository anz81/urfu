using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.VariantsView)]
    public class EduProgramLimitsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: EduProgramLimits
        public ActionResult Index(int variantId)
        {
            var variant = db.Variants.Find(variantId);
            ViewBag.Variant = variant;

            var wrongModules = new List<string>();
            bool error;
            var limitViewModels = db.GetLimitViewModels(User, variantId, out error, out wrongModules);

            ViewBag.Error = error;
            ViewBag.WrongModules = string.Join(", ", wrongModules);

            return View(limitViewModels);
        }

        [HttpPost]
        [Authorize(Roles = ItsRoles.VariantsEdit)]
        public ActionResult SetStudentsCount(int variantId, string moduleId, string value)
        {
            try
            {
                var limit = db.EduProgramLimits.FirstOrDefault(l => l.VariantId == variantId && l.ModuleId == moduleId);

                if (value.Trim().Length == 0 && limit != null)
                {
                    db.Entry(limit).State = EntityState.Deleted;
                }
                else if (value.Trim().Length != 0 && limit != null)
                {
                    int result;
                    if (!int.TryParse(value, out result))
                    {
                        return Json(new { status = false, message = "Не удалось распознать значение. Укажите целое число." });
                    }
                    limit.StudentsCount = result;
                }
                else if (value.Trim().Length != 0 && limit == null)
                {
                    limit = new EduProgramLimit
                    {
                        VariantId = variantId,
                        ModuleId = moduleId,
                        StudentsCount = int.Parse(value)
                    };
                    db.Entry(limit).State = EntityState.Added;
                }
                db.SaveChanges();

                var program = db.Variants.Find(variantId).Program;
                var message = db.GetStudentsCountLimit(program.Id, moduleId);

                return Json(new {status = true, message});
            }
            catch (Exception ex)
            {
                return Json(new {status = false, message = "Ошибка выполнения."});
            }
        }


    }
}