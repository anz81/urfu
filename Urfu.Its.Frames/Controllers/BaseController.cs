using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Frames.Controllers
{
    public class BaseController : Controller
    {
        protected internal ActionResult JsonNet(object obj)
        {
            return Content(JsonConvert.SerializeObject(obj), "application/json", System.Text.Encoding.UTF8);
        }
    }
}
