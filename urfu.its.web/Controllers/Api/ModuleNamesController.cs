using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using System.Web.SessionState;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
   // [SessionState(SessionStateBehavior.Disabled)]
    public class ModuleNamesController : BaseController
    {
        public object Get(string directionId)
        {
            using (var db = new ApplicationDbContext())
            {
                List<Module> variants;
                if (directionId != null)
                    variants = db.UniModules().Where(v => v.Directions.Any(d=>d.uid == directionId)).ToList();
                else
                    variants = db.UniModules().ToList();

                return variants.Select(v => new { name = v.numberAndTitle, id = v.uuid }).ToList();
            }
        }
    }
}