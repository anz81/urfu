using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Newtonsoft.Json.Linq;
using PagedList.Core;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Ext.Utilities.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.WorkingProgramManager)]
    public class EduResultController : Controller
    {
        public async Task<ActionResult> Index(int? page, int? limit, string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                SortRules sortRules = SortRules.Deserialize(sort);

                using (var db = new ApplicationDbContext())
                {
                    List<Division> userDivisions;
                    if (User.IsInRole(ItsRoles.AllDirections))
                    {
                        userDivisions = await db.Divisions.ToListAsync();
                    }
                    else
                    {
                        var userName = User.Identity.Name;
                        userDivisions = await db.Divisions.Where(m => m.Users.Any(u => u.UserName == userName)).ToListAsync();
                        userDivisions = userDivisions.SelectMany(d => EnumerateWithChildDivisions(d, db)).Distinct().ToList();
                    }

                    var userDivisionIds = userDivisions.Select(d => d.uuid).ToList();

                    var userDirections = db.DirectionsForUser(User);                        
                    var allDivisions = db.Divisions;
                    var canUserRemoveEduresult= User.IsInRole(ItsRoles.Admin);

                    var eduResults = db.EduResults
                        .Include(r => r.Profile)
                        .Include(r => r.Profile.Direction)
                        .Where(r => !r.IsDeleted)
                        .Where(r => userDirections.Contains(r.Profile.Direction))
                        .Select(r => new
                        {
                            EduResult = r,
                            Division = allDivisions.FirstOrDefault(d2 => d2.uuid == r.Profile.CHAIR_ID)
                        })
                        .Select(r => new
                        {
                            r.EduResult,
                            ParentDivision = allDivisions.FirstOrDefault(d => d.uuid == r.Division.parent)
                        })
                        .Where(r=> userDivisionIds.Contains(r.ParentDivision.uuid))
                        .Select(r => new
                        {
                            id = r.EduResult.Id,
                            code = "РО-" + r.EduResult.CodeNumber,
                            description = r.EduResult.Description,
                            directionId = r.EduResult.Profile.DIRECTION_ID,
                            directionOkso = r.EduResult.Profile.Direction.okso,
                            directionName = r.EduResult.Profile.Direction.title,
                            profileId = r.EduResult.ProfileId,
                            profileCode = r.EduResult.Profile.CODE,
                            profileName = r.EduResult.Profile.NAME,
                            divisionId = r.ParentDivision.uuid,
                            divisionName = r.ParentDivision.shortTitle,
                            r.EduResult.Profile.Direction.standard,
                            canremove = canUserRemoveEduresult
                        })
                        .Where(FilterRules.Deserialize(filter))
                        .OrderBy(sortRules.FirstOrDefault(), m => m.id)
                        .ToList().ToPagedList(page ?? 1, limit ?? 25);
                        
                    return Json(eduResults, new JsonSerializerSettings());
                }                
            }

            return View();
        }
        
        [HttpPost]
        public ActionResult Create([ExcludeBind(nameof(EduResult.Id))]EduResult eduResult)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        var profileId = eduResult.ProfileId;
                        var lastCodeNumber = db.EduResults.Where(r => r.ProfileId == profileId).OrderByDescending(r => r.CodeNumber).FirstOrDefault()?.CodeNumber;
                        eduResult.CodeNumber = lastCodeNumber+1 ?? 1;
                        // TODO нужно провалидировать профиль
                        db.EduResults.Add(eduResult);
                        db.SaveChanges();
                        var profile = db.Profiles.FirstOrDefault(_ => _.ID == eduResult.ProfileId);
                        Logger.Info($"Добавлен результат обучения РО-{eduResult.CodeNumber} Код профиля:{profile?.CODE}  {profile?.NAME}");

                        return Json(new {success = true});
                    }
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
        public ActionResult Edit(EduResult eduResult)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        // TODO нужно провалидировать профиль
                        var existingEduResult = db.EduResults.Find(eduResult.Id);
                        if (existingEduResult == null)
                            throw new InvalidOperationException($"Запись с идентификатором '{eduResult.Id}' не найдена");

                        existingEduResult.ProfileId = eduResult.ProfileId;
                        existingEduResult.Description = eduResult.Description;
                        db.SaveChanges();
                        return Json(new {success = true});
                    }
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

        [HttpGet]
        [Authorize(Roles = ItsRoles.Admin)]
        public void Delete(int? id)
        {
            using (var db = new ApplicationDbContext())
            {
                // TODO проверка на возможно удаления
                var eduResult = db.EduResults.FirstOrDefault(r => r.Id == id);
                if (eduResult != null)
                {
                    eduResult.IsDeleted = true;
                    db.SaveChanges();
                   // var profile = db.Profiles.FirstOrDefault(_ => _.ID == eduResult.ProfileId);
                    Logger.Info($"Удаление результата обучения РО-{eduResult.CodeNumber} Код профиля:{eduResult.Profile?.CODE}  {eduResult.Profile?.NAME}");

                }
                else
                    throw new InvalidOperationException($"Запись с идентификатором '{id}' не найдена.");                
            }
        }
        
        [HttpGet]
        public ActionResult UserStandard()
        {
            using (var db = new ApplicationDbContext())
            {               
                var userDirections = db.DirectionsForUser(User).ToList();
                var standarts = userDirections.GroupBy(d => d.standard).Select(d => new
                {
                    standard = d.Key,
                    directions = d.Select(_d => new
                    {
                        id = _d.uid,
                        oksoAndTitle = _d.OksoAndTitle,
                    }).OrderBy(_d => _d.oksoAndTitle)
                }).ToList();
                return Json(standarts, new JsonSerializerSettings());
            }
        }

        [HttpGet]
        public async Task<ActionResult> UserDivisions()
        {
            using (var db = new ApplicationDbContext())
            {
                List<Division> divisions;
                if (User.IsInRole(ItsRoles.AllDirections))
                {
                    divisions = await db.Divisions.ToListAsync();
                }
                else
                {
                    var userName = User.Identity.Name;
                    divisions = await db.Divisions.Where(m => m.Users.Any(u => u.UserName == userName)).ToListAsync();
                    divisions = divisions.SelectMany(d => EnumerateWithChildDivisions(d, db)).Distinct().ToList();
                }

                var eduPrograms = divisions.ToList()
                    .Select(p => new
                    {
                        id = p.uuid,                        
                        nameAndShortName = $"({p.shortTitle}) {p.typeTitle} {p.title}"
                    })
                    .ToList();
                return Json(eduPrograms, new JsonSerializerSettings());
            }
        }

        private IEnumerable<Division> EnumerateWithChildDivisions(Division parent, ApplicationDbContext db, bool includeParent = true)
        {
            if(includeParent)
                yield return parent;
            var parentId = parent.uuid;
            foreach (var childDivision in db.Divisions.Where(d => d.parent == parentId).ToList())
            {
                yield return childDivision;
                foreach (var child2Division in EnumerateWithChildDivisions(childDivision, db, false))
                {
                    yield return child2Division;
                }
            }
        }

        [HttpGet]
        public ActionResult UserProfiles(string directionId, string divisionId)
        {
            using (var db = new ApplicationDbContext())
            {
                var allDivisions = db.Divisions;

                var eduPrograms = db.Profiles
                    .Where(p => p.DIRECTION_ID == directionId)
                    .Select(r => new
                    {
                        Profile = r,
                        Division = allDivisions.FirstOrDefault(d2 => d2.uuid == r.CHAIR_ID)
                    })
                    //.Where(p=>p.Division != null && p.Division.parent == divisionId)
                    .Select(p=>new
                    {
                        id = p.Profile.ID,
                        name = p.Profile.NAME,
                        qualification = p.Profile.QUALIFICATION,
                        code = p.Profile.CODE,
                        divisionName = p.Division.shortTitle
                    })
                    .ToList();
                return Json(eduPrograms, new JsonSerializerSettings());                
            }
        }
    }
}