using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
//using System.Windows.Forms;
using AutoMapper;
using Urfu.Its.Common;
using Urfu.Its.Integration;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace Urfu.Its.Web.Controllers
{
    [Authorize(Roles = ItsRoles.Admin)]
    public class SyncController : Controller
    {
        // GET: Sync
        public ActionResult Index()
        {
            return View(new SyncModel());
        }

        public ActionResult Directions()
        {
            bool alreadyInProgress;
            SyncEngine.DoDirectionsSync(out alreadyInProgress);
            return View("Index",
                new SyncModel
                {
                    Message =
                        alreadyInProgress ? "Синхронизация направлений в процессе" : "Синхронизация направлений запущена"
                });
        }

        public ActionResult Modules()
        {
            bool alreadyInProgress;
            SyncEngine.DoModulesSync(out alreadyInProgress);
            return View("Index", new SyncModel ());
        }

        public ActionResult People()
        {
            bool alreadyInProgress;
            SyncEngine.DoPeopleSync(out alreadyInProgress);
            return View("Index", new SyncModel()
            {
                Message =
                    alreadyInProgress ? "Синхронизация персонала в процессе" : "Синхронизация персонала запущена"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rating(int Year, int Class,int Term, bool WithCoefficients)
        {
            bool alreadyInProgress;
            SyncEngine.DoRatingSync(Year, Class, Term, WithCoefficients, out alreadyInProgress);
            return View("Index", new SyncModel()
            {
                Message =
                    alreadyInProgress ? "Синхронизация рейтинга была запущена ране. Повторите запрос позже." : "Синхронизация рейтинга запущена"
            });
        }
        [HttpPost]
        
        public ActionResult GroupHistories(int year)
        {
            bool alreadyInProgress;
            SyncEngine.DoGroupHistorySync(year, out alreadyInProgress);
            return View("Index", new SyncModel()
            {
                Message =
                    alreadyInProgress ? "Синхронизация исторических групп была запущена ране. Повторите запрос позже." : "Синхронизация исторических групп запущена"
            });
        }

        public ActionResult RatingAvg()
        {
            bool alreadyInProgress;
            SyncEngine.DoRatingAvgSync(out alreadyInProgress);
            return View("Index", new SyncModel()
            {
                Message =
                    alreadyInProgress ? "Синхронизация средного балла была запущена ране. Повторите запрос позже." : "Синхронизация средного балла запущена"
            });
        }
        public ActionResult SyncStudentPlans()
        {
            bool alreadyInProgress;
            SyncEngine.DoStudentAllPlansSync(out alreadyInProgress);
            return View("Index", new SyncModel()
            {
                Message =
                    alreadyInProgress ? "Синхронизация всех планов студентов была запущена ране. Повторите запрос позже." : "Синхронизация планов студентов запущена"
            });
        }
        public ActionResult StudentPlan()
        {
            bool alreadyInProgress;
            SyncEngine.DoStudentPlanSync(out alreadyInProgress);
            return View("Index", new SyncModel()
            {
                Message =
                    alreadyInProgress ? "Синхронизация планов студентов была запущена ране. Повторите запрос позже." : "Синхронизация планов студентов запущена"
            });
        }
        public ActionResult GenerateTestVariants()
        {
            SyncEngine.GenerateTestVariants();
            return View("Index", new SyncModel());
        }

        public ActionResult SyncTeachers()
        {
            SyncEngine.SyncTeachers();
            return View("Index", new SyncModel());
        }
        public ActionResult SyncEntrants()
        {
            SyncEngine.SyncEntrants();
            return View("Index", new SyncModel());
        }

        public ActionResult SyncModuleAgreements()
        {
            SyncEngine.SyncModuleAgreements();
            return View("Index", new SyncModel());
        }

        public ActionResult SyncROPs()
        {
            SyncEngine.SyncROPs();
            return View("Index", new SyncModel());
        }

        public ActionResult SyncTrajectories()
        {
            SyncEngine.SyncTrajectories();
            return View("Index", new SyncModel());
        }

        //public ActionResult Test()
        //{
        //    SyncEngine.SyncGrouoHistory(2012);
        //    //var groups = new GroupHistoryService().GetGroupHistories(2012);
        //    return HttpNotFound();
        //}
        [Authorize(Roles = ItsRoles.NsiEdit)]
        public ActionResult SyncDirection(string directionId)
        {
            var db = new ApplicationDbContext();
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            Logger.Info("Синхронизация по запросу для "+ directionId);
            var divisions = new UniDivisionsService().GetDivisions();
            SyncEngine.SyncModulesForDirection(directionId, db, divisions);
            SyncEngine.SyncDataForDirection(directionId, db, divisions);
            Logger.Info("Синхронизация по запросу закончена для " + directionId);
            return View("Index", new SyncModel());
        }

        public ActionResult Selection()
        {
            bool alreadyInProgress;
            SyncEngine.SyncStudentSelection(out alreadyInProgress);
            return View("Index", new SyncModel()
            {
                Message =
                    alreadyInProgress ? "Синхронизация выбора студентов в процессе" : "Синхронизация выбора студентов запущена"
            });
        }


        public ActionResult CreatePrograms(int year)
        {
            SyncEngine.CreateEduProgramms(year);
            return View("Index", new SyncModel());
        }

        public ActionResult SyncApploads(int year, int term)
        {
            AutoResetEvent e = new AutoResetEvent(false);
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(x =>
            {
                SyncEngine.SyncApploads(year, term);
                e.Set();
            });
            e.WaitOne();
            
            return View("Index", new SyncModel());
        }
        public ActionResult SyncDebtors(string moduleTitle, int? year, string term)
        {
            AutoResetEvent e = new AutoResetEvent(false);
            
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(token =>
            {
                SyncEngine.SyncDebtors(moduleTitle,year, term);
                e.Set();
            });
            e.WaitOne();

            return View("Index", new SyncModel());
        }

        public ActionResult SyncForeignLanguageRating()
        {
            AutoResetEvent e = new AutoResetEvent(false);
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(x =>
            {
                SyncEngine.SyncForeignLanguageRating();
                e.Set();
            });
            e.WaitOne();

            return View("Index", new SyncModel());
        }
        public ActionResult SyncTmers()
        {
            AutoResetEvent e = new AutoResetEvent(false);
            var backtask = new BackgroundTaskScheduler();
            backtask.QueueBackgroundWorkItem(x =>
            {
                SyncEngine.SyncTmers();
                e.Set();
            });
            e.WaitOne();
            
            return View("Index", new SyncModel());
        }


        public ActionResult ResyncStudentSelections()
        {
            SyncEngine.ResyncStudentSelections();

            return View("Index", new SyncModel());
        }

        [HttpPost]
        public ActionResult GenerateTestSelections()
        {
#if DEBUG
            SyncEngine.GenerateTestSelections();
#endif
            return Content("Ok");
        }

        public ActionResult SendAdmissionsToRunp()
        {
            RunpAdmissionsController.QueueAllStudentAdmissionToRunp();
            return View("Index", new SyncModel());
        }

        public ActionResult SendAdmissionsToLKS()
        {
            AdmissionsController.QueueAllStudentAdmissionToLKS();
            return View("Index", new SyncModel());
        }

        public ActionResult SyncDivision(string id)
        {
            DivisionDto ld;
            var divisions = new UniDivisionsService().GetDivisions();
            if (!divisions.TryGetValue(id, out ld))
            {
                return View("Index", new SyncModel() {Message = "Подразделение не найдено"});
            }
            SyncEngine.UpdateDivision(new[] {ld}, divisions);
            return View("Index", new SyncModel());
        }
    }

}