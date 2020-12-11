using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using System.Web.SessionState;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Controllers
{
   // [SessionState(SessionStateBehavior.Disabled)]
    public class ProfileNamesController : BaseController
    {
        public object Get(string okso, string directionId)
        {
            using (var db = new ApplicationDbContext())
            {
                List<Profile> programs;
                if (okso != null)
                    programs = db.Profiles.Where(v => v.Direction.okso == okso).ToList();
                else if (directionId != null)
                    programs = db.Profiles.Where(v => v.DIRECTION_ID == directionId).ToList();
                else
                    programs = db.Profiles.ToList();

                return programs.Where(p=>p!=null).Select(v => new { name = v.CODE + " "+v.NAME, id = v.ID}).ToList();
            }
        }
    }
}