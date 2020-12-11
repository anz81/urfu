using System.Collections.Generic;
using Ext.Utilities;
using Ext.Utilities.Linq;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models.OHOPModels;
using Urfu.Its.Web.Models;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Urfu.Its.Web.Controllers
{

    [Authorize(Roles = ItsRoles.NsiView)]
    public class ProfOrdersController : BaseController
    {
        readonly ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string sort, string filter)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var filterRules = ObjectableFilterRules.Deserialize(filter);
                var sortRules = SortRules.Deserialize(sort);

                var profOrders = db.ProfOrders.Select(p => new ProfOrderViewModel()
                {
                    ProfOrderId = p.Id,
                    ProfStandardCode = p.ProfStandardCode,
                    NumberOfMintrud = p.NumberOfMintrud,
                    DateOfMintrud =p.DateOfMintrud,
                    RegNumberOfMinust = p.RegNumberOfMinust,
                    RegNumberDateOfMinust = p.RegNumberDateOfMinust,
                    Status = p.Status
                }).Where(filterRules).OrderBy(p => p.ProfStandardCode).AsQueryable();

                if (sortRules.Count>0)
                {
                     var sortRule = sortRules[0];
                     profOrders = profOrders.OrderBy(sortRule);
                }

                return Json(
                    new
                    {
                        data = profOrders,
                        total = profOrders.Count()
                    },
                    new JsonSerializerSettings()
                );
            }

            var orderStatuses = db.ProfOrders.Select(p=>new { StatusName = p.Status }).Distinct().ToList();
            ViewBag.OrderStatuses = JsonConvert.SerializeObject(orderStatuses);
            ViewBag.CanEdit = User.IsInRole(ItsRoles.NsiEdit);
            return View();
        }

        [HttpGet]
        public ActionResult ProfStandards()
        {
            var standards = db.ProfStandards.Select(s => new
            {
                s.Code,
                s.Title
            });
            return Json(standards, new JsonSerializerSettings());
        }


        [HttpPost]
        public ActionResult Create([ExcludeBind(nameof(ProfOrder.Id))]ProfOrder profOrder)
        {
            if(ModelState.IsValid)
            {
               var code= db.ProfStandards.FirstOrDefault(s => s.Code == profOrder.ProfStandardCode);
                if (code == null)
                    return Json(new { success = false, message = $"Не найден код профессионального стандарта '{profOrder.ProfStandardCode}'" });//, "text/html", Encoding.Unicode);

                db.ProfOrders.Add(profOrder);
                db.SaveChanges();
                return Json(new { success = true });//,"text/html", Encoding.Unicode);
            }

            return Json(new { success = false });

        }

        [HttpPost]
        public ActionResult Edit(ProfOrder profOrder )
        {
            if(ModelState.IsValid)
            {
                var profOrderE = db.ProfOrders.FirstOrDefault(_ => _.Id == profOrder.Id);
                if(profOrderE== null)
                    return Json(new { success = false, message = $"Не найдена запись'{profOrder.Id}'"});

                var code = db.ProfStandards.FirstOrDefault(s => s.Code == profOrder.ProfStandardCode);
                
                if (profOrderE != null && code!= null)
                {
                    profOrderE.ProfStandardCode = profOrder.ProfStandardCode;
                    profOrderE.NumberOfMintrud = profOrder.NumberOfMintrud;
                    profOrderE.DateOfMintrud = profOrder.DateOfMintrud;
                    profOrderE.RegNumberOfMinust = profOrder.RegNumberOfMinust;
                    profOrderE.RegNumberDateOfMinust = profOrder.RegNumberDateOfMinust;
                    profOrderE.Status = profOrder.Status;

                    db.SaveChanges();

                    return Json(new { success = true });
                }
                
           }
            return Json(new { success = false });

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var profOrder = db.ProfOrders.FirstOrDefault(_=>_.Id ==id);
            
            if(profOrder != null)
            {
                db.ProfOrders.Remove(profOrder);
                db.SaveChanges();
                return Json(new { success = true}, new JsonSerializerSettings());
            }

            return Json(new { success = false, message = $"Не найдена запись'{id}'" });//, "text/html", Encoding.Unicode);
        }


        public ActionResult GetProfOrderChanges(int proforderId)
        {
            var proforder = db.ProfOrders.FirstOrDefault(d => d.Id == proforderId);

            if (proforder == null)

                return Json(new
                {
                    data = new List<object>(),
                },
                    new JsonSerializerSettings()
                    );

            var proforderchanges = proforder.OrderChanges.Select(ch => new { ch.ProfOrderChange, ch.Status })
                .Select(p => new
                {
                    ProfOrderChangeID = p.ProfOrderChange.Id,
                    ProfOrderId = proforder.Id,
                    DateOfMintrud = p.ProfOrderChange.DateOfMintrud?.ToShortDateString(),
                    p.ProfOrderChange.NumberOfMintrud,
                    p.ProfOrderChange.RegNumberOfMinust,
                    RegNumberDateOfMinust = p.ProfOrderChange.RegNumberDateOfMinust?.ToShortDateString(),
                    p.Status
                }).ToList();

            return Json(new
                {
                    data = proforderchanges,
                },
                new JsonSerializerSettings()
            );
        }

        public ActionResult UpdateProforderChange(ProfOrderChangeViewModel model)
        {
            try
            {

                var profOrderChange = db.ProfOrderChanges.FirstOrDefault(p => p.Id == model.ProfOrderChangeID);

                bool isNewprofOrderChange = false;

                if (profOrderChange == null)
                {
                    isNewprofOrderChange = true;
                    profOrderChange = new ProfOrderChange();
                }

                profOrderChange.NumberOfMintrud = model.NumberOfMintrud;
                profOrderChange.DateOfMintrud = model.DateOfMintrud;
                profOrderChange.RegNumberDateOfMinust = model.RegNumberDateOfMinust;
                profOrderChange.RegNumberOfMinust = model.RegNumberOfMinust;

                if (isNewprofOrderChange)
                {
                    db.ProfOrderChanges.Add(profOrderChange);
                    db.SaveChanges();
                    int profOrderChangeId = profOrderChange.Id;
                    db.ProfOrderConnections.Add(new ProfOrderConnection { ProfOrderId = (int)model.ProfOrderId, ProfOrderChangeId = profOrderChangeId, Status = model.Status });
                }
                else
                    db.ProfOrderConnections.First(c => c.ProfOrderId == model.ProfOrderId && c.ProfOrderChangeId == model.ProfOrderChangeID).Status = model.Status;
                db.SaveChanges();

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }


        public ActionResult RemoveProfOrderChange(int ProfOrderChangeId)
        {
            try
            {
                db.ProfOrderChanges.Remove(db.ProfOrderChanges.FirstOrDefault(p => p.Id == ProfOrderChangeId));
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