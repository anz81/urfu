using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Urfu.Its.Web.Models;
using Ext.Utilities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.RatingCoefficientEdit)]
    public class RatingCoefficientsController : BaseController
    {

        readonly ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: RatingCoefficients
        public ActionResult Index(string filter)
        {

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {

                FilterRules rules = FilterRules.Deserialize(filter);

                var yearstr = rules?.FirstOrDefault(r => r.Property == "Year")?.Value;
                int year;
                bool isYear = Int32.TryParse(yearstr, out year);

                var foreignfanguageCoefficients = db.RatingCoefficients.Where(c => c.Year == year && c.ModuleType == (int)ModuleTypeParam.ForeignLanguage);

                var foreignlanguageModules = db.Modules.Where(m => m.type == ModuleTypes.ForeignLanguage).Where(m => m.ForeignLanguage.Periods.Any(p => p.Year == year)).Select(m => new { moduleId = m.uuid, moduleTitle = m.title, ModuleType = (int)ModuleTypeParam.ForeignLanguage, Year = year });


                var modules = foreignlanguageModules
                    .GroupJoin(
                        foreignfanguageCoefficients,
                        module => module.moduleId,
                        coefficient => coefficient.ModuleId,
                        (m,c) => new {module = m, coefficient = c}
                    )
                    .SelectMany(
                        mc => mc.coefficient.DefaultIfEmpty(),
                        (m, c) => new {Module = m.module, Coefficient=c})
                    .Select(s => new RatingCoefficientsViewModel()
                        {
                            ModuleId = s.Module.moduleId,
                            ModuleTitle= s.Module.moduleTitle,
                            Id = s.Coefficient == null ? null : (int?)s.Coefficient.Id,
                            ModuleType = s.Module.ModuleType,
                            Level = s.Coefficient == null ? "" : s.Coefficient.Level,
                            Year =  s.Module.Year,
                            Coefficient = s.Coefficient == null ? null : (decimal?)s.Coefficient.Coefficient
                        }
                    ).ToList();

                var MupCoefficients =
                    db.RatingCoefficients.Where(c => c.ModuleType == (int) ModuleTypeParam.MUP && c.Year == year);

                var MUPs = db.Modules.Where(m => m.type == ModuleTypes.MUP).Where(m => m.MUP.Periods.Any(p => p.Year == year && !p.Removed)).Select(m => new { moduleId = m.uuid, moduleTitle = m.title, ModuleType = (int)ModuleTypeParam.MUP, Year = year, Level = m.Level });

                var MuPModules = MUPs
                    .GroupJoin(
                        MupCoefficients,
                        module => module.moduleId,
                        coefficient => coefficient.ModuleId,
                        (m, c) => new { module = m, coefficient = c}
                    )
                    .SelectMany(
                        mc => mc.coefficient.DefaultIfEmpty(),
                        (m, c) => new { Module = m.module, Coefficient = c })
                    .Select(s => new RatingCoefficientsViewModel()
                        {
                            ModuleId = s.Module.moduleId,
                            ModuleTitle = s.Module.moduleTitle,
                            Id = s.Coefficient == null ? null : (int?)s.Coefficient.Id,
                            ModuleType = s.Module.ModuleType,
                            Level=s.Module.Level,
                            Year = s.Module.Year,
                            Coefficient = s.Coefficient == null ? null : (decimal?)s.Coefficient.Coefficient
                        }
                    ).ToList();


                var projects = db.RatingCoefficients.Where(c => c.ModuleType == (int) ModuleTypeParam.Project &&c.Year==year).Select(
                    c => new RatingCoefficientsViewModel()
                    {
                        Id = c.Id,
                        Coefficient = c.Coefficient,
                        Year = c.Year,
                        ModuleType = c.ModuleType,
                        Level =  c.Level
                    }).ToList();

                modules.AddRange(MuPModules);
                modules.AddRange(projects);

                return Json(
                    new
                    {
                        data = modules.OrderBy(m=>m.ModuleTypeName).ThenBy(m=>m.ModuleTitle).ThenBy(m=>m.Level)

                    },
                    new JsonSerializerSettings()
                );
                           
            }
           return View();
        }

        public ActionResult GetModuleTypes()
        {
            Dictionary<int, string> moduletypes = new Dictionary<int, string>();

            foreach (ModuleTypeParam mtype in Enum.GetValues(typeof(ModuleTypeParam)))
            {
                switch (mtype)
                {
                    case ModuleTypeParam.ForeignLanguage:
                        moduletypes.Add((int)mtype, "Иностранный язык");
                        break;
                    case ModuleTypeParam.Project:
                        moduletypes.Add((int)mtype, "Проектное обучение");
                        break;
                    case ModuleTypeParam.MUP:
                        moduletypes.Add((int)mtype, "МУПы");
                        break;
                }                    
            }
            return Json(moduletypes.Select(t=>new {moduleType=t.Key,moduleName =t.Value}), new JsonSerializerSettings());
        }

        public ActionResult GetForeignLanguageModules(int year)
        {
            var foreignlanguagesList = db.Modules.Where(m => m.type == ModuleTypes.ForeignLanguage).Where(m => m.ForeignLanguage.Periods.Any(p => p.Year == year)).Select(m => new {moduleId = m.uuid, moduleTitle = m.title}).ToList();
            
            return Json( new {data= foreignlanguagesList}, new JsonSerializerSettings());
        }

        public ActionResult GetMUPModules(int year)
        {
            var MUPList = db.Modules.Where(m => m.type == ModuleTypes.MUP).Where(m => m.MUP.Periods.Any(p => p.Year == year)).Select(m => new { moduleId = m.uuid, moduleTitle = m.title});

            return Json(new{data= MUPList}, new JsonSerializerSettings());
        }

        [HttpPost]
        public ActionResult CreateCoefficient([ExcludeBind(nameof(RatingCoefficient.Id))] RatingCoefficient ratingCoefficient)
        {          
            if (ModelState.IsValid)
            {
                if (db.RatingCoefficients.Any(c => c.ModuleType == ratingCoefficient.ModuleType && c.Level == ratingCoefficient.Level && c.Year == ratingCoefficient.Year && c.ModuleId == ratingCoefficient.ModuleId))
                    return Json(new { success = false, message = "Для указанных модуля/уровня/года коэффициент задан" });//, "text/html", Encoding.Unicode);

                if (ratingCoefficient.ModuleType == (int) ModuleTypeParam.MUP)
                {
                    var mupmodule = db.Modules.SingleOrDefault(m => m.uuid == ratingCoefficient.ModuleId);
                    
                    if (mupmodule != null)
                        mupmodule.Level = ratingCoefficient.Level;
                }

                db.RatingCoefficients.Add(ratingCoefficient);
                db.SaveChanges();

                var moduleTitle = db.Modules.FirstOrDefault(m => m.uuid == ratingCoefficient.ModuleId)?.title;
                return Json(new { success = true, newCoefficient = new RatingCoefficientsViewModel(ratingCoefficient, moduleTitle) });//, "text/html", Encoding.Unicode);
                
            }
           
           return new StatusCodeResult(StatusCodes.Status400BadRequest);
         }

        [HttpPost]
        public ActionResult UpdateCoefficient(RatingCoefficient ratingCoefficient)
        {          
            if (ModelState.IsValid) {
                if (db.RatingCoefficients.AsNoTracking().SingleOrDefault(c => c.Id == ratingCoefficient.Id) == null)
                    return Json(new { success = false, message = $"Не найдена запись'{ratingCoefficient.Id}'" });//, "text/html", Encoding.Unicode);

                if (db.RatingCoefficients.Any(c => c.ModuleType == ratingCoefficient.ModuleType && c.Level == ratingCoefficient.Level && c.Year == ratingCoefficient.Year && c.ModuleId == ratingCoefficient.ModuleId && c.Id != ratingCoefficient.Id))
                    return Json(new { success = false, message = "Для указанных модуля/уровня/года коэффициент задан" });//, "text/html", Encoding.Unicode);

                if (ratingCoefficient.ModuleType == (int) ModuleTypeParam.MUP)
                {
                    var mupmodule = db.Modules.SingleOrDefault(m => m.uuid == ratingCoefficient.ModuleId);

                    if (mupmodule != null)
                        mupmodule.Level = ratingCoefficient.Level;
                }

                db.Entry(ratingCoefficient).State = EntityState.Modified;
                db.SaveChanges();

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
          return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            var coefficient = db.RatingCoefficients.SingleOrDefault(_ => _.Id == id);

            if (coefficient != null)
            {
                db.RatingCoefficients.Remove(coefficient);
                db.SaveChanges();
                return new StatusCodeResult(StatusCodes.Status200OK);
            }

            return Json(new { success = false, message = $"Не найдена запись'{id}'" });//, "text/html", Encoding.Unicode);
        }



    }
}