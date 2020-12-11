using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Ext.Utilities;
using Ext.Utilities.Linq;
//using Microsoft.Ajax.Utilities;
using PagedList.Core;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
    public class TeachersController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Teachers
        public ActionResult Index(int? page, int? limit, string filter, string sort)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var teachers = db.Teachers.Select(t => new
                {
                    name = t.lastName + " " + t.firstName + " " + t.middleName,
                    t.post,
                    t.workPlace
                });

                SortRules sortRules = SortRules.Deserialize(sort);
                teachers = teachers.OrderBy(sortRules.FirstOrDefault(), m => m.name);

                teachers = teachers.Where(FilterRules.Deserialize(filter));

                var paginated = teachers.ToPagedList(page ?? 1, limit ?? 25);

                return JsonNet(new
                {
                    data = paginated,
                    total = teachers.Count()
                });
            }


            var posts = db.Teachers.Select(t => t.post).Distinct().Select(p => new { post = p, }).ToList();
            ViewBag.post = posts;
            return View();

        }
    }
}