using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.StudentAdmission)]
    public class LogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Log
        public ActionResult Index()
        {
            return View(db.Logs.OrderByDescending(l => l.Date).Take(300).ToList());
        }

        public ActionResult Filter(DateTime from,DateTime to)
        {
            return View("Index",db.Logs.Where(l=>l.Date>from && l.Date<to).OrderByDescending(l => l.Date).Take(300).ToList());
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
