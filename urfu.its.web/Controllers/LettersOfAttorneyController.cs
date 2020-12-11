using Ext.Utilities;
//using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TemplateEngine;
using Urfu.Its.Integration;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.Practice;
using Urfu.Its.Web.Models;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.PracticeView)]
    public class LettersOfAttorneyController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LettersOfAttorney
        public ActionResult Index()
        {
            ViewBag.CanEdit = User.IsInRole(ItsRoles.PracticeManager);
            return View();
        }

        public ActionResult GetLetterOfAttorney()
        {

            var r1 = db.LettersOfAttorney.Select(d => new {
                d.Id,
                d.Number,
                d.StartDate,
                d.EndDate
            }).ToList();
            var r2 = r1.Select(r => new {
                r.Id,
                r.Number,
                StartDate = r.StartDate.ToShortDateString(),
                EndDate = r.EndDate.ToShortDateString()
            }).ToList();
            return Json(r2, new JsonSerializerSettings());
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult EditLettersOfAttorney(int id, string number, DateTime Startdate, DateTime Enddate)
        {
            if (Enddate < Startdate)
                return Json(new { succsess = false, message = "Дата окончания доверенности не может быть раньше даты начала действия доверенности" });//, "text/html", Encoding.Unicode);

            var existLetterOfAttorney = db.LettersOfAttorney.Where(l=>l.Id!=id).Where(d => (d.StartDate <= Startdate && d.EndDate >= Startdate) || (d.StartDate <= Enddate && d.EndDate >= Enddate)|| (d.StartDate>= Startdate && d.StartDate<= Enddate)).FirstOrDefault();
            if (existLetterOfAttorney != null)
                return Json(new { succsess = false, message = "Доверенность на этот период создана" });//,"text/html", Encoding.Unicode);
  
            if (id == 0)
            {
                db.LettersOfAttorney.Add(new LettersOfAttorney
                {
                    Number = number,
                    StartDate = Startdate,
                    EndDate = Enddate
                });
                db.SaveChanges();
                return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
            }

            var data = db.LettersOfAttorney.FirstOrDefault(n => n.Id == id);
            if (data == null)
            {
                return Json(new { success = false, message = "Редактируемыая доверенность не найдена" });//, "text/html", Encoding.Unicode);
            }
              
            data.Number = number;
            data.StartDate = Startdate;
            data.EndDate = Enddate;

            db.SaveChanges();

            return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
        }

        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult RemoveLettersOfAttorney(int id)
        {
            var data = db.LettersOfAttorney.FirstOrDefault(n => n.Id == id);
            if (data == null)
            {
                return Json(new { success = false, message = "Удаляемая доверенность не найдена" });//, "text/html", Encoding.Unicode);
            }
            db.LettersOfAttorney.Remove(data);
            db.SaveChanges();
            return Json(new { success = true, message = "" });//, "text/html", Encoding.Unicode);
        }


    }
}