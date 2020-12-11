using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
//using Urfu.Its.Web.Migrations;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class CompetenceGroupsController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var groups = db.CompetenceGroups.Select(c => new
                {
                    c.Id,
                    c.Name
                });
                
                return JsonNet(groups);
            }

            ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);

            return View();
        }
        
        [HttpPost]
        public ActionResult Create([ExcludeBind(nameof(CompetenceGroup.Id))] CompetenceGroup cg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.CompetenceGroups.Any(c => c.Name == cg.Name))
                    {
                        return Json(new
                        {
                            success = false,
                            error = "Группа компетенций с таким названием уже существует"
                        });
                    }
                    
                    db.CompetenceGroups.Add(cg);
                    db.SaveChanges();

                    Logger.Info($"Добавлена группа компетенций  \"{cg.Name}\"");// +
                       // $"Уровень подготовки: \"{areaEducation.Qualifications}\"");
                    return Json(new { success = true });
                }

                dynamic result = new JObject();
                var propertyStatesWithErrors = ModelState.Where(s => s.Value?.Errors.Any() == true).ToList();
                if (propertyStatesWithErrors.Any())
                {
                    var errorsObject = new JObject();
                    result.errors = errorsObject;
                    result.success = false;
                    foreach (var s in propertyStatesWithErrors)
                        errorsObject[s.Key.ToLowerFirstLetter()] = string.Join("; ", s.Value.Errors.Select(e => e.ErrorMessage));
                }

                return Content(result.ToString(), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public ActionResult Edit(CompetenceGroup cg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.CompetenceGroups.Any(c => c.Name == cg.Name && c.Id != cg.Id))
                    {
                        return Json(new
                        {
                            success = false,
                            error = "Группа компетенций с таким названием уже существует"
                        });
                    }
                    
                    if (db.CompetenceGroups.Any(c => c.Id == cg.Id))
                    {
                        db.Entry(cg).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true });
                    }

                    return Json(new
                    {
                        success = false,
                        error = "Редактируемая группа компетенций не найдена"
                    });
                }

                dynamic result = new JObject();
                var propertyStatesWithErrors = ModelState.Where(s => s.Value?.Errors.Any() == true).ToList();
                if (propertyStatesWithErrors.Any())
                {
                    var errorsObject = new JObject();
                    result.errors = errorsObject;
                    result.success = false;
                    foreach (var s in propertyStatesWithErrors)
                        errorsObject[s.Key.ToLowerFirstLetter()] = string.Join("; ", s.Value.Errors.Select(e => e.ErrorMessage));
                }

                return Content(result.ToString(), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
        
        public ActionResult Delete(int id)
        {
            var competenceGroup = db.CompetenceGroups.FirstOrDefault(c => c.Id == id);
            if (competenceGroup == null)
            {
                return NotFound("competence not found");
            }
            db.Entry(competenceGroup).State = EntityState.Deleted;
            db.SaveChanges();
            Logger.Info($"Удалена группа компетенций ");

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}