using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using System.Net;
using Newtonsoft.Json;
using Ext.Utilities;
using Ext.Utilities.Linq;
using PagedList.Core;
using Urfu.Its.Web.Models;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.NsiView)]
    public class TrainingDurationController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, int? limit, string sort, string filter, string focus)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var filterRules = ObjectableFilterRules.Deserialize(filter);
                var sortRules = SortRules.Deserialize(sort);

                var filterDirection = filterRules?.Find(f => f.Property == "directionUid");
                filterRules?.Remove(filterDirection);

                List<string> filterDirectionValue = new List<string>();
                try
                {
                    JArray array = (JArray)filterDirection?.Value;
                    filterDirectionValue = array.ToObject<List<string>>();
                }
                catch { }

                var durations = db.TrainingDurations.Select(d => new {
                    id = d.Id,
                    divisionUuid = d.Division.uuid,
                    divisionTitle = d.Division.title,
                    qualification = d.Qualification,
                    directionUid = d.DirectionUid,
                    directionCode = d.Direction.okso,
                    directionTitle = d.Direction.title,
                    standard = d.Direction.standard,
                    familirizationType = d.FamilirizationType,
                    duration = d.Duration,
                    durationSPO = d.DurationSPO,
                    durationSPOUnsuitableProfile = d.DurationSPOUnsuitableProfile,
                    durationVPO = d.DurationVPO,
                    durationVPOUnsuitableProfile = d.DurationVPOUnsuitableProfile
                }).Where(d => filterDirectionValue.Contains(d.directionUid) || filterDirectionValue.Count == 0)
                    .Where(filterRules).OrderBy(d => d.divisionTitle).AsQueryable();

                if (sortRules?.Count > 0)
                {
                    var sortRule = sortRules[0];
                    durations = durations.OrderBy(sortRule);
                }
                var paginated = durations.ToPagedList(page ?? 1, limit ?? 25);

                return Json(
                     new
                     {
                         data = paginated,
                         total = durations.Count()
                     },
                     new JsonSerializerSettings()
                 );
            }
            else
            {
                var divisions = db.InstitutesForUser(User, includeDepartments: true).OrderBy(d => d.title).ToList()
                    .Select(d => new
                    {
                        Id = d.uuid,
                        Name = d.typeTitle == "Департамент" 
                                        ? $"Департамент {d.shortTitle} ({d.title})".Replace('"', ' ') 
                                        : $"{d.shortTitle} ({d.title})".Replace('"', ' '),
                        Directions = db.DirectionsForDivision(d.uuid).ToList().GroupBy(dir => dir.OksoAndTitle).Select(dir => new
                                        {
                                            OksoAndTitle = dir.Key,
                                            Ids = dir.Select(dq => dq.uid),
                                            qualifications = dir.SelectMany(dq => dq.qualifications.Split(new string[1] { ", " }, StringSplitOptions.None))
                                                    .Distinct()
                                                    .Select(q => new { qualification = q }),
                                            standards = dir.Select(ds => new { ds.standard, directionUid = ds.uid })
                                        }).ToList()
                    }).ToList();
                ViewBag.Divisions = JsonConvert.SerializeObject(divisions);
                
                var familirizationTypes = db.FamilirizationTypes.Select(f => new
                {
                    Name = f.Name
                });
                ViewBag.FamilirizationTypes = JsonConvert.SerializeObject(familirizationTypes);

                ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);

                return View();
            }
        }

        public ActionResult CreateDuration//([Bind(Exclude = nameof(TrainingDuration.Id))]TrainingDuration duration) 
            (string division, string qualification, string direction, string familirizationType,
                    Decimal duration, decimal? dSPO, decimal? dSPOUP, decimal? dVPO, decimal? dVPOUP)
        {
            try
            {
                if(db.TrainingDurations.Any(d =>
                        d.DivisionUuid == division && d.Qualification == qualification
                        && d.DirectionUid == direction && d.FamilirizationType == familirizationType))
                {
                    return Json(new { success = false, message = "Срок обучения с такими параметрами уже существует" });//, "text/html", Encoding.Unicode);
                }
                
                var trainingDuration = new TrainingDuration();
                    
                trainingDuration.DirectionUid = direction;
                trainingDuration.DivisionUuid = division;
                trainingDuration.FamilirizationType = familirizationType;
                trainingDuration.Qualification = qualification;
                trainingDuration.Duration = duration;
                trainingDuration.DurationSPO = dSPO;
                trainingDuration.DurationSPOUnsuitableProfile = dSPOUP;
                trainingDuration.DurationVPO = dVPO;
                trainingDuration.DurationVPOUnsuitableProfile = dVPOUP;

                db.TrainingDurations.Add(trainingDuration);
                db.SaveChanges();
                return Json(new { success = true, message = "Срок обучения успешно добавлен" });//, "text/html", Encoding.Unicode);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        public ActionResult UpdateDuration(int id, Decimal duration, decimal? dSPO, decimal? dSPOUP, decimal? dVPO, decimal? dVPOUP)
        {
            var trainingDuration = db.TrainingDurations.FirstOrDefault(d => d.Id == id);
            if (trainingDuration == null)
                return Json(new { success = false, message = "Редактируемая запись не найдена" });//, "text/html", Encoding.Unicode);

            trainingDuration.Duration = duration;
            trainingDuration.DurationSPO = dSPO;
            trainingDuration.DurationSPOUnsuitableProfile = dSPOUP;
            trainingDuration.DurationVPO = dVPO;
            trainingDuration.DurationVPOUnsuitableProfile = dVPOUP;

            db.SaveChanges();
            return Json(new { success = true, message = "Срок обучения успешно обновлен" });//, "text/html", Encoding.Unicode);
        }
        
        public ActionResult RemoveDuration(int id)
        {
            try
            {
                db.TrainingDurations.Remove(db.TrainingDurations.FirstOrDefault(d => d.Id == id));
                db.SaveChanges();
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }
    }
}