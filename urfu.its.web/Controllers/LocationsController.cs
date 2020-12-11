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
    [Authorize(Roles = ItsRoles.PracticeView)]
    public class LocationsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var countries = db.CompanyLocations.Where(l => l.Level == 1)
                    .Where(FilterRules.Deserialize(filter))
                    .Select(l => new { l.Id, l.Name })
                    .OrderBy(l => l.Name);

                var paginated = countries.ToPagedList(page ?? 1, limit ?? 25);

                return Json(
                    new
                    {
                        data = paginated,
                        total = countries.Count()
                    },
                    new JsonSerializerSettings() 
                );
            }
            else
            {
                ViewBag.Focus = focus;
                ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
                return View();
            }
        }

        public ActionResult Regions(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                SortRules sortRules = SortRules.Deserialize(sort);
                var regions = db.CompanyLocations.Where(l => l.Level == 2)
                    .Select(l => new { l.Id, l.Name, Country = l.Parent.Name, CountryId = l.ParentId })
                    .Where(FilterRules.Deserialize(filter))
                    .OrderByThenBy(sortRules.FirstOrDefault(), l => l.Country, l => l.Name);

                var paginated = regions.ToPagedList(page ?? 1, limit ?? 25);

                return Json(
                    new
                    {
                        data = paginated,
                        total = regions.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                ViewBag.Focus = focus;
                ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
                return View();
            }
        }

        public ActionResult Cities(int? page, int? limit, string sort, string filter, string focus)
        {
            if (Request.IsAjaxRequest())
            {
                SortRules sortRules = SortRules.Deserialize(sort);
                var cities = db.CompanyLocations.Where(l => l.Level == 3)
                    .Select(l => new { l.Id, l.Name, Region = l.Parent.Name, RegionId = l.ParentId, Country = l.Parent.Parent.Name, CountryId = l.Parent.Parent.Id })
                    .Where(FilterRules.Deserialize(filter))
                    .OrderByThenBy(sortRules.FirstOrDefault(), l => l.Country, l => l.Region, l => l.Name);

                var paginated = cities.ToPagedList(page ?? 1, limit ?? 25);

                return Json(
                    new
                    {
                        data = paginated,
                        total = cities.Count()
                    },
                    new JsonSerializerSettings()
                );
            }
            else
            {
                ViewBag.Focus = focus;
                ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
                return View();
            }
        }

        public ActionResult CountriesList()
        {
            var countries = db.CompanyLocations.Where(l => l.Level == 1)
                .Select(l => new {
                    CountryId = l.Id,
                    Country = l.Name
                }).OrderBy(l => l.Country);
            return Json(
                new
                {
                    data = countries,
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult RegionsList(int id)
        {
            var regions = db.CompanyLocations.Where(l => l.Level == 2 && l.ParentId == id)
                .Select(l => new {
                    RegionId = l.Id,
                    Region = l.Name })
                .OrderBy(l => l.Region);
            return Json(
                new
                {
                    data = regions,
                },
                new JsonSerializerSettings()
            );
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult EditCountry(int id, string name)
        {
            var existCountry = db.CompanyLocations.FirstOrDefault(l => l.Name == name && l.Level == 1 && l.Id != id);
            if (existCountry == null)
            {
                if (id != 0)
                {
                    // уже существующая страна
                    var country = db.CompanyLocations.FirstOrDefault(l => l.Id == id && l.Level == 1);
                    if (country != null)
                    {
                        country.Name = name;
                        db.SaveChanges();
                        return Json(new { success = true, message = "" }); //, "text/html", Encoding.Unicode);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Редактируемая страна не найдена" });//, "text/html", Encoding.Unicode);
                    }
                }
                else
                {
                    // новая страна
                    db.CompanyLocations.Add(new CompanyLocation()
                    {
                        Level = 1,
                        Name = name
                    });
                    db.SaveChanges();
                    return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                }
            }
            else
            {
                return Json(new { success = false, message = "Страна с таким названием уже существует" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult EditRegion(int id, int parentId, string name)
        {
            var existRegion = db.CompanyLocations.FirstOrDefault(l => l.Name == name && l.Level == 2 && l.ParentId == parentId && l.Id != id);
            if (existRegion == null)
            {
                if (id != 0)
                {
                    // уже существующий регион
                    var region = db.CompanyLocations.FirstOrDefault(l => l.Id == id && l.Level == 2 && l.ParentId == parentId);
                    if (region != null)
                    {
                        region.Name = name;
                        db.SaveChanges();
                        return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Редактируемый регион не найден" });//, "text/html", Encoding.Unicode);
                    }
                }
                else
                {
                    // новый регион
                    db.CompanyLocations.Add(new CompanyLocation()
                    {
                        ParentId = parentId,
                        Level = 2,
                        Name = name
                    });
                    db.SaveChanges();
                    return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                }
            }
            else
            {
                return Json(new { success = false, message = "Регион с таким названием уже существует" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult EditCity(int id, int parentId, string name)
        {
            var existCity = db.CompanyLocations.FirstOrDefault(l => l.Name == name && l.Level == 3 && l.ParentId == parentId && l.Id != id);
            if (existCity == null)
            {
                if (id != 0)
                {
                    // уже существующий город
                    var city = db.CompanyLocations.FirstOrDefault(l => l.Id == id && l.Level == 3 && l.ParentId == parentId);
                    if (city != null)
                    {
                        city.Name = name;
                        db.SaveChanges();
                        return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Редактируемый город не найден" });//, "text/html", Encoding.Unicode);
                    }
                }
                else
                {
                    // новый город
                    db.CompanyLocations.Add(new CompanyLocation()
                    {
                        ParentId = parentId,
                        Level = 3,
                        Name = name
                    });
                    db.SaveChanges();
                    return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                }
            }
            else
            {
                return Json(new { success = false, message = "Город с таким названием уже существует" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveCountry(int id)
        {
            var country = db.CompanyLocations.FirstOrDefault(l => l.Id == id && l.Level == 1);
            if (country != null)
            {
                List<CompanyLocation> locations = new List<CompanyLocation>();
                var regions = db.CompanyLocations.Where(l => l.ParentId == id && l.Level == 2);
                var cities = db.CompanyLocations.Where(l => regions.Select(r => r.Id).Contains((int)l.ParentId) && l.Level == 3);
                locations.Add(country);
                locations.AddRange(regions);
                locations.AddRange(cities);
                var _locations = locations.Select(l => l.Id);

                var company = db.Companies.Where(c => c.CompanyLocationId != null).FirstOrDefault(c => _locations.Contains((int)c.CompanyLocationId));

                if (company == null)
                {
                    db.CompanyLocations.RemoveRange(cities);
                    db.CompanyLocations.RemoveRange(regions);
                    db.CompanyLocations.Remove(country);
                    db.SaveChanges();
                    return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                }
                else
                {
                    return Json(new { success = false, message = "Страна не может быть удалена, так как на ее территории есть предприятия" });//, "text/html", Encoding.Unicode);
                }
            }
            else
            {
                return Json(new { success = false, message = "Удаляемая страна не найдена" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveRegion(int id)
        {
            var region = db.CompanyLocations.FirstOrDefault(l => l.Id == id && l.Level == 2);
            if (region != null)
            {
                List<CompanyLocation> locations = new List<CompanyLocation>();
                var cities = region.Childs;
                locations.Add(region);
                locations.AddRange(cities);
                var _locations = locations.Select(l => l.Id);

                var company = db.Companies.Where(c => c.CompanyLocationId != null).FirstOrDefault(c => _locations.Contains((int)c.CompanyLocationId));

                if (company == null)
                {
                    db.CompanyLocations.RemoveRange(cities);
                    db.CompanyLocations.Remove(region);
                    db.SaveChanges();
                    return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                }
                else
                {
                    return Json(new { success = false, message = "Регион не может быть удален, так как на его территории есть предприятия" });//, "text/html", Encoding.Unicode);
                }
            }
            else
            {
                return Json(new { success = false, message = "Удаляемый регион не найден" });//, "text/html", Encoding.Unicode);
            }
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveCity(int id)
        {
            var city = db.CompanyLocations.FirstOrDefault(l => l.Id == id && l.Level == 3);
            if (city != null)
            {
                var company = db.Companies.Where(c => c.CompanyLocationId != null).FirstOrDefault(c => c.CompanyLocationId == id);

                if (company == null)
                {
                    db.CompanyLocations.Remove(city);
                    db.SaveChanges();
                    return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
                }
                else
                {
                    return Json(new { success = false, message = "Город не может быть удален, так как на его территории есть предприятия" });//, "text/html", Encoding.Unicode);
                }
            }
            else
            {
                return Json(new { success = false, message = "Удаляемый город не найден" });//, "text/html", Encoding.Unicode);
            }
        }
    }
}