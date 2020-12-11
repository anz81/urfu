using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Web.Controllers
{
    public class AliveController : Controller
    {
        // GET: Alive
         [HttpGet]
        public ActionResult Index()
        {
            return Content("alive");
        }
    }
}