using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Web.Controllers
{
    public class BaseController : Controller
    {
        protected internal ActionResult JsonNet(object obj)
        {
            return Content(JsonConvert.SerializeObject(obj), "application/json", System.Text.Encoding.UTF8);
        }
    }
}