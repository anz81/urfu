using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using System.Web.SessionState;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
  //  [SessionState(SessionStateBehavior.Disabled)]
    public class ProgramNamesController : BaseController
    {
        public object Get(string okso, string directionId)
        {
            using (var db = new ApplicationDbContext())
            {
                List<EduProgram> programs;
                if (okso != null)
                    programs = db.EduPrograms.Where(v => v.Direction.okso == okso && v.State == VariantState.Approved && v.Variant.State == VariantState.Approved).ToList();
                else if (directionId != null)
                    programs = db.EduPrograms.Where(v => v.directionId == directionId && v.State == VariantState.Approved && v.Variant.State == VariantState.Approved).ToList();
                else
                    programs = db.EduPrograms.ToList();

                return programs.Select(v => new { programName = v.FullName, id = v.Id }).ToList();
            }
        }
    }

    //[SessionState(SessionStateBehavior.Disabled)]
    public class PlanVersionsController : BaseController
    {
        public object Get(int programId, int eduplanNumber)
        {
            using (var ctrlr = new EduProgramsController())
            {
                var enumerable = ctrlr.GetPlansQuery(programId).Where(p => p.eduplanNumber == eduplanNumber).Select(p => p.versionNumber).Distinct();
                
                return enumerable.Select(v => new { id = v }).ToList();
            }
        }
    }
}