using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
    //[SessionState(SessionStateBehavior.Disabled)]
    public class DivisionNamesController : ControllerBase
    {
        public object Get(string okso, string directionId)
        {
            using (var db = new ApplicationDbContext())
            {
                List<Division> programs;
                if (okso != null)
                    programs = db.Directions.Where(v => v.okso == okso).SelectMany(d=>d.Modules).SelectMany(m=>m.Plans).Select(p=>p.FacultyDivision).Distinct().ToList();
                else if (directionId != null)
                    programs = db.Plans.Where(v => v.directionId == directionId).Select(p => p.FacultyDivision).Distinct().ToList();
                else
                    programs = db.Divisions.ToList();

                return programs.Where(p=>p!=null).Select(v => new { divisionName = v.shortTitle, id = v.uuid }).ToList();
            }
        }
    }
}