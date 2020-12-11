using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
//using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
//using System.Web.SessionState;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Urfu.Its.Common;
using Urfu.Its.Integration.Models;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;
using Microsoft.EntityFrameworkCore;
using Ext.Utilities;
using Newtonsoft.Json;

namespace Urfu.Its.Web.Controllers
{
    [IdentityBasicAuthentication]
    public class StudentRatingController : BaseController
    {
        public object Get(string studentuuid)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var student = db.Students.Include("Person").FirstOrDefault(s => s.Id == studentuuid);

                var result = new
                {
                    Rating = student?.RatingType == StudentRatingType.Regular ? student?.Rating : null,
                    student?.Person?.DateOfBirth
                };

                return Json(result);
            }
        }
    }
}