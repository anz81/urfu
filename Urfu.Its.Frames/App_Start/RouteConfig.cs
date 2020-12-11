using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Urfu.Its.Frames
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
           /* routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapMvcAttributeRoutes();*/
        }
    }
}
