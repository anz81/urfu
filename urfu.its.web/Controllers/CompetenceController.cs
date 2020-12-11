using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
using PagedList;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class CompetenceController : BaseController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private readonly List<string> competenceTypes = new List<string>()
        {
            "ПК"//,
            //"ПСК"
        };

        private readonly List<string> competencesWithoutProfile = new List<string>(){ "УК", "ОПК" };

        public ActionResult Index(string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var competences = GetCompenteces(filter);

                return JsonNet(new
                {
                    data = competences
                });
            }

            var directions = _db.DirectionsForUser(User).Where(d => d.standard != "ФГОС ВПО");

            var areaEducations = _db.AreaEducations.ToList().OrderBy(a => a.Code).Select(a => new
            {
                a.Id,
                Name = $"{a.Code} {a.Title}"
            });
            ViewBag.AreaEducations = JsonConvert.SerializeObject(areaEducations);

            var competencesGroups = _db.CompetenceGroups.Select(c => new
            {
                c.Id,
                c.Name
            }).OrderBy(c => c.Name).ToList();
            ViewBag.CompetenceGroups = JsonConvert.SerializeObject(competencesGroups);

            if (User.IsInRole(ItsRoles.AllDirections))
                competenceTypes.AddRange(competencesWithoutProfile);

            var standarts = directions.GroupBy(d => d.standard).ToList().Select(d => new { 
                Standard = d.Key,
                Types = _db.CompetenceTypes.Where(c => (d.Key == "СУОС" || d.Key == "ФГОС ВО 3++") && competenceTypes.Contains(c.Name) || d.Key == "ФГОС ВО"),
                Directions = d.Select(_d => new {
                    Id = _d.uid,
                    Name = _d.OksoAndTitle,
                    _d.AreaEducationId,
                    Profiles = _db.Profiles.Where(p => p.DIRECTION_ID == _d.uid && !p.remove)
                            .Select(p => new
                            {
                                profile = p,
                                divisionTitle = p.Division.title
                            }).Where(p => p.divisionTitle != null).ToList().Select(p => new
                                {
                                    Id = p.profile.ID,
                                    Name = p.profile.OksoAndTitle,
                                    Division = p.divisionTitle.Replace('"', ' ')
                                }).OrderBy(p => p.Name)
                }).OrderBy(_d => _d.Name)
            });
            ViewBag.Standards = JsonConvert.SerializeObject(standarts);

            var types = _db.CompetenceTypes.ToList().Where(c => User.IsInRole(ItsRoles.AllDirections) || !competencesWithoutProfile.Contains(c.Name));
            ViewBag.Types = JsonConvert.SerializeObject(types);

            ViewBag.CompetencesWithoutProfile = JsonConvert.SerializeObject(competencesWithoutProfile);

            ViewBag.Qualifications = JsonConvert.SerializeObject(_db.Qualifications);

            ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit); // при изменении внести изменения и в права на метод Delete

            return View();
        }
        
        private IQueryable<CompetenceViewModel> GetCompenteces(string filter)
        {
            _db.Database.SetCommandTimeout(120000);
            //((IObjectContextAdapter)_db).ObjectContext.CommandTimeout = 120000;

            List<Division> userDivisions;
            if (User.IsInRole(ItsRoles.AllDirections))
            {
                userDivisions = _db.Divisions.ToList();
            }
            else
            {
                var userName = User.Identity.Name;
                userDivisions = _db.Divisions.Where(m => m.Users.Any(u => u.UserName == userName)).ToList();
                userDivisions = userDivisions.SelectMany(d => EnumerateWithChildDivisions(d, _db)).Distinct().ToList();
            }

            var userDivisionIds = userDivisions.Select(d => d.uuid).ToList();

            var userDirections = _db.DirectionsForUser(User).ToList();
            var userDirectionsOkso = userDirections.Select(d => d.okso);
            var userAreaEducations = userDirections.Select(d => d.AreaEducationId).Distinct();

            var allDivisions = _db.Divisions;
            var competences = _db.CompetencesActive()
                .Where(_ => userDirectionsOkso.Contains(_.Direction.okso) && _.Direction != null || userAreaEducations.Contains(_.AreaEducationId) && _.Direction == null)
                .Select(c => new
                {
                    Competence = c,
                    Division = c.ProfileId != null ? allDivisions.FirstOrDefault(d2 => d2.uuid == c.Profile.CHAIR_ID) : null
                })
                .Select(c => new
                {
                    Competence = c.Competence,
                    ParentDivision = c.Division != null ? allDivisions.FirstOrDefault(d => d.uuid == c.Division.parent) : null
                })
                .Where(r => r.ParentDivision == null || userDivisionIds.Contains(r.ParentDivision.uuid)).ToList()
                .Select(c => new CompetenceViewModel(c.Competence))
                .AsQueryable();
            
            competences = competences.Where(FilterRules.Deserialize(filter));
            competences = competences.OrderBy(r => r.Type).ThenBy(r => r.DirectionId).ThenBy(r => r.ProfileId).ThenBy(r => r.Order);

            return competences;
        }
        
        private IEnumerable<Division> EnumerateWithChildDivisions(Division parent, ApplicationDbContext db, bool includeParent = true)
        {
            if (includeParent)
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
        [HttpPost]
        public ActionResult Create([ExcludeBind(nameof(Competence.Id))] Competence competence)
        {
            try
            {
                ModelState.Remove(nameof(competence.Code));
                if (ModelState.IsValid)
                {
                    bool withoutProfile = competencesWithoutProfile.Contains(competence.Type);

                    if(competence.DirectionId == null && withoutProfile && competence.Standard == "ФГОС ВО 3++")
                        return Json(new
                        {
                            success = false,
                            error = "Укажите направление"
                        });

                    if (competence.DirectionId == null && competence.ProfileId == null && !withoutProfile) //&& competence.AreaEducationId == null)
                        return Json(new
                            {
                                success = false,
                                error = "Укажите направление и образовательную программу"
                            });

                    if (competence.AreaEducationId == null && withoutProfile && competence.Standard != "ФГОС ВО 3++")
                    {
                        return Json(new
                        {
                            success = false,
                            error = "Укажите область образования"
                        });
                    }

                    if (competence.QualificationName == null && withoutProfile && competence.Standard != "ФГОС ВО 3++")
                    {
                        return Json(new
                        {
                            success = false,
                            error = "Укажите квалификацию"
                        });
                    }

                    if (!competence.CompetenceGroupId.HasValue && withoutProfile)
                        return Json(new
                        {
                            success = false,
                            error = "Укажите группу компетенции"
                        });


                    var userName = User.Identity.Name;
                    
                    var type = _db.CompetenceTypes.FirstOrDefault(_ => _.Name == competence.Type);
                    var existCompetence = competence.Code == "ПК-М" ? null :
                        _db.CompetencesActive().FirstOrDefault(
                                c => (

                                competence.Standard == "СУОС" && withoutProfile && competence.QualificationName == c.QualificationName && competence.AreaEducationId == c.AreaEducationId

                                || competence.Standard == "СУОС" && !withoutProfile && competence.ProfileId == c.ProfileId

                                || competence.Standard == "ФГОС ВО" && (type.IsStandard || c.ProfileId == competence.ProfileId)

                                || competence.Standard == "ФГОС ВО 3++" && withoutProfile && competence.DirectionId == c.DirectionId
                                || competence.Standard == "ФГОС ВО 3++" && !withoutProfile && competence.ProfileId == c.ProfileId

                                )
                                    && c.Standard == competence.Standard && c.Type == competence.Type && c.Order == competence.Order);

                    if (existCompetence != null)
                        return Json(new
                        {
                            success = false,
                            error = "Компетенция с таким номером уже существует"
                        });

                    competence.Code = $"{competence.Type}-{competence.Order}";
                    competence.Content = competence.Content?.Replace("\n", " ");

                    var direction = _db.Directions.FirstOrDefault(_ => _.uid == competence.DirectionId);

                    competence.Okso = withoutProfile ? null : direction?.okso;
                    
                    competence.AreaEducationId = withoutProfile && competence.Standard == "СУОС" ? competence.AreaEducationId : direction?.AreaEducationId;
                    var profile = _db.Profiles.FirstOrDefault(_ => _.ID == competence.ProfileId);

                    competence.QualificationName = competence.QualificationName ?? profile?.QUALIFICATION ?? direction?.qualifications;

                    competence.ApprovedDate = DateTime.Now;

                    _db.Competences.Add(competence);
                    _db.SaveChanges();

                    var competenceVM = new CompetenceViewModel(_db.Competences.Include(c => c.AreaEducation).Include(c => c.CompetenceGroup).FirstOrDefault(c => c.Id == competence.Id));

                    Logger.Info($"Добавлена компетенция  {competence.Code} Код профиля:{profile?.CODE} {profile?.NAME}. Пользователь {userName}");
                    return Json(new { success = true, data = competenceVM });
                
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
        public ActionResult Edit(Competence competence)
        {
            try
            {
                ModelState.Remove(nameof(competence.Code));
                ModelState.Remove(nameof(competence.DirectionId));
                ModelState.Remove(nameof(competence.ProfileId));
                ModelState.Remove(nameof(competence.Standard));
                ModelState.Remove(nameof(competence.Type));
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.Name;
                    //var user = db.Users.Include(u => u.UserDivisions).Single(u => u.UserName == userName);
                    //var divisions = user.UserDivisions.ToList();
                    // TODO доделать подразделения
                    var currCompentence = _db.Competences.FirstOrDefault(_ => _.Id == competence.Id);

                    bool withoutProfile = competencesWithoutProfile.Contains(currCompentence.Type);

                    if (!competence.CompetenceGroupId.HasValue && withoutProfile)
                        return Json(new
                        {
                            success = false,
                            error = "Укажите группу компетенции"
                        });


                    var existCompetence = currCompentence.Code == "ПК-М" ? null :
                        _db.CompetencesActive().FirstOrDefault(
                                c => (

                                currCompentence.Standard == "СУОС" && withoutProfile && currCompentence.QualificationName == c.QualificationName && currCompentence.AreaEducationId == c.AreaEducationId

                                || currCompentence.Standard == "СУОС" && !withoutProfile && currCompentence.ProfileId == c.ProfileId

                                || currCompentence.Standard == "ФГОС ВО" && (currCompentence.TypeInfo.IsStandard || c.ProfileId == competence.ProfileId)

                                || currCompentence.Standard == "ФГОС ВО 3++" && withoutProfile && currCompentence.DirectionId == c.DirectionId
                                || currCompentence.Standard == "ФГОС ВО 3++" && !withoutProfile && currCompentence.ProfileId == c.ProfileId

                                )
                                    && c.Standard == currCompentence.Standard && c.Type == currCompentence.Type && c.Order == competence.Order && c.Id != competence.Id);

                    if (existCompetence != null)
                        return Json(new
                        {
                            success = false,
                            error = "Компетенция с таким номером уже существует"
                        });

                    
                    currCompentence.Content = competence.Content?.Replace("\n", " ");
                    currCompentence.AreaEducationId = withoutProfile && currCompentence.Standard == "СУОС" ? competence.AreaEducationId : currCompentence.Direction?.AreaEducationId;
                    currCompentence.CompetenceGroupId = competence.CompetenceGroupId;

                    currCompentence.QualificationName = currCompentence.QualificationName ?? currCompentence.Profile?.QUALIFICATION ?? currCompentence.Direction?.qualifications;
                    currCompentence.Order = competence.Order;
                    currCompentence.Code = $"{currCompentence.Type}-{competence.Order}";

                    _db.Entry(currCompentence).State = EntityState.Modified;
                    _db.SaveChanges();

                    var competenceVM = new CompetenceViewModel(_db.Competences.Include(c => c.AreaEducation).Include(c => c.CompetenceGroup).FirstOrDefault(c => c.Id == competence.Id));
                    
                    return Json(new { success = true, data = competenceVM });
                 
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
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult Delete(int id)
        {
            var competence = _db.Competences.FirstOrDefault(_ => _.Id == id);
            if (competence == null)
            {
                return NotFound("competence not found");
            }
            competence.IsDeleted = true;
            _db.SaveChanges();
            Logger.Info($"Удалена компетенция  {competence.Code} Код профиля: {competence.Profile?.CODE}  {competence.Profile?.NAME}");

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
        public ActionResult StandardsList()
        {
            var standarts = _db.Standards.Select(_ => new { Id = _.Name, Name = _.Name });
            return Json(new { data = standarts }, new JsonSerializerSettings());
        }
        public ActionResult CompetenceTypes()
        {
            var standarts = _db.CompetenceTypes;
            return Json(new { data = standarts }, new JsonSerializerSettings());
        }

        [HttpGet]
        public ActionResult Profiles(string directionId)
        {
            using (var db = new ApplicationDbContext())
            {
                var directions = db.Profiles
                    .Where(_ => _.DIRECTION_ID == directionId && !_.remove).Include(_=>_.Division)
                    .Where(_=> _.Division != null)
                    .ToList()
                    .Select(d => new
                    {
                        id = d.ID,
                        name = d.NAME,
                        code = d.CODE,
                        division = d.Division?.title,
                        parent = _db.Divisions.FirstOrDefault(_=>d.Division.parent == _.uuid)?.shortTitle
                    })
                    .ToList();
                return Json(directions, new JsonSerializerSettings());
            }
        }

        [HttpGet]
        public ActionResult UserDirections(string standart)
        {
            using (var db = new ApplicationDbContext())
            {
                var directions = db.Directions
                    .Where(_=>_.standard == standart)
                    .ToList()
                    .Select(d => new
                    {
                        id = d.uid,
                        d.okso,
                        d.OksoAndTitle,
                        d.AreaEducationId
                    })
                    .ToList();
                return Json(directions, new JsonSerializerSettings());
            }
        }

        public ActionResult AllAreaEducations()
        {
            using (var db = new ApplicationDbContext())
            {
                var areaEducations = db.AreaEducations.ToList().Select(a => new {
                    a.Id,
                    Name = $"{a.Code} {a.Title}"
                });
                return Json(areaEducations, new JsonSerializerSettings());
            }
        }

        [Authorize(Roles = ItsRoles.NsiView)]
        public ActionResult EduResults(int id, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var eduResults = _db.EduResults2.Where(r => r.CompetenceId == id).Where(FilterRules.Deserialize(filter)).ToList()
                    .Select(r => new CompetenceEduResultViewModel(r))
                    .OrderBy(r => r.EduResultKindName).ThenBy(r => r.EduResultTypeName).ThenBy(r => r.SerialNumber);

                return JsonNet(new
                {
                    data = eduResults
                });
            }
            
            var types = _db.EduResultTypes.ToList();
            ViewBag.Types = JsonConvert.SerializeObject(types);

            var kinds = _db.EduResultKinds.ToList();
            ViewBag.Kinds = JsonConvert.SerializeObject(kinds);

            ViewBag.CompetenceId = id;

            var competence = _db.Competences.FirstOrDefault(c => c.Id == id);
            ViewBag.Title = $"Результаты обучения компетенции {competence.Code}";
            if (competence.Profile != null)
                ViewBag.Title += $", {competence.Profile?.OksoAndTitle}";
            if (competence.Profile == null && competence.Direction != null)
                ViewBag.Title += $", {competence.Direction?.OksoAndTitle}";
            if (competence.AreaEducation != null)
                ViewBag.Title += $", {competence.AreaEducation?.Code} {competence.AreaEducation?.Title}";

            ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
            return View();
        }

        [HttpPost]
        
        public ActionResult CreateEduResult([ExcludeBind(nameof(EduResult2.Id))]EduResult2 eduResult)
        {
            try
            {
                ModelState.Remove(nameof(eduResult.Code));
                ModelState.Remove(nameof(eduResult.SerialNumber));

                if (ModelState.IsValid)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        var type = db.EduResultTypes.FirstOrDefault(t => t.Id == eduResult.EduResultTypeId);
                        if (type == null)
                            return Json(new
                            {
                                success = false,
                                error = "Не найден указанный тип РО"
                            });

                        var kind = db.EduResultKinds.FirstOrDefault(t => t.Id == eduResult.EduResultKindId);
                        if (kind == null)
                            return Json(new
                            {
                                success = false,
                                error = "Не найден указанный вид РО"
                            });

                        var previousNumber = db.EduResults2.Where(r => r.CompetenceId == eduResult.CompetenceId && r.EduResultTypeId == eduResult.EduResultTypeId && r.EduResultKindId == eduResult.EduResultKindId)
                            .OrderByDescending(r => r.SerialNumber)
                            .FirstOrDefault()?.SerialNumber;

                        eduResult.SerialNumber = previousNumber.HasValue ? previousNumber.Value + 1 : 1;
                        eduResult.Code = $"{kind.ShortName}{type.ShortName}-{eduResult.SerialNumber}";
                        eduResult.Description = eduResult.Description.Replace("\n", " ");
                        db.EduResults2.Add(eduResult);
                        db.SaveChanges();

                        Logger.Info($"Добавлен результат обучения {eduResult.Code} Компетенция:{eduResult.CompetenceId}");

                        return Json(new { success = true, data = new CompetenceEduResultViewModel(eduResult) });
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
        public ActionResult EditEduResult(EduResult2 eduResult)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        eduResult.Description = eduResult.Description.Replace("\n", " ");
                        db.Entry(eduResult).State = EntityState.Modified;
                        db.SaveChanges();
                        Logger.Info($"Изменен результат обучения {eduResult.Code} Компетенция:{eduResult.CompetenceId}");
                        eduResult.EduResultType = db.EduResultTypes.FirstOrDefault(t => t.Id == eduResult.EduResultTypeId);
                        eduResult.EduResultKind = db.EduResultKinds.FirstOrDefault(t => t.Id == eduResult.EduResultKindId);
                        return Json(new { success = true, data = new CompetenceEduResultViewModel(eduResult) });
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
        public void DeleteEduResult(int? id)
        {
            using (var db = new ApplicationDbContext())
            {
                // TODO проверка на возможно удаления
                var eduResult = db.EduResults2.FirstOrDefault(r => r.Id == id);
                if (eduResult != null)
                {
                    db.EduResults2.Remove(eduResult);
                    db.SaveChanges();

                    Logger.Info($"Удален результат обучения {eduResult.Code} Id:{eduResult.Id} CompetenceId: {eduResult.CompetenceId} Type: {eduResult.EduResultTypeId}");

                }
                else
                    throw new InvalidOperationException($"Запись с идентификатором '{id}' не найдена.");
            }
        }
    }
}