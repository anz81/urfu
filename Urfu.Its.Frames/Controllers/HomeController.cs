using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Owin;
//using Microsoft.Owin.Security.Cookies;
//using Microsoft.Owin.Security.WsFederation;
using System.Linq;
using System.Security.Claims;
using Urfu.Its.Web.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestSharp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Urfu.Its.Frames.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private string magic = "requestoauth2";
        private string magic2 = "quest";

        public ActionResult Index()
        {
            if (IsPracticeRequest())
            {
                return RedirectToAction("LK", "Practice");
            }

            if (IsProjectRequest())
            {
                return RedirectToAction("LK", "Project");
            }

            Response.Cookies.Append(".AspNetCore.Cookies", "");

            ViewBag.User = User;
            ViewBag.UserADName = User.GetADName();

            StudentPageVM vm;
            using (var db = new ApplicationDbContext())
            {
                var studentId = UserSecurity.StudentID(User, db);

                ViewBag.UserId = studentId;
                vm = new StudentPageVM(db, studentId);
            }

            return View("FK", vm);
        }

        private bool CheckRequest()
        {
            try
            {
                var c = Request.Cookies[magic];
                    //Get(magic);
                return c == "1";
            }
            catch
            {
                return false;
            }
        }

        private bool IsPracticeRequest()
        {
            try
            {
                var c = Request.Cookies[magic2];
                return c == "practice";
            }
            catch
            {
                return false;
            }
        }

        private bool IsProjectRequest()
        {
            try
            {
                var c = Request.Cookies[magic2];
                return c == "project";
            }
            catch
            {
                return false;
            }

        }

        [HttpGet]
        public ActionResult Claims()
        {

            var list = new List<HttpCookie>();

            foreach (var cookie in Request.Cookies)
            {
                HttpCookie c = new HttpCookie();
                c.Name = cookie.Key;
                c.Value = cookie.Value;                
                list.Add(c);
            }
            //for (var i = 0; i < Request.Cookies.Count; i++)
            //    list.Add(Request.Cookies[i]);

            var state = new StateVM
            {
                Principal = User as ClaimsPrincipal,
                Cookies = list
            };



            return View("ListClaims", state);
        }

        public ActionResult ListClaims()
        {
            return View(User);
        }


        public ActionResult FK()
        {
            //HttpContext.User.Identities.Any(i => i.IsAuthenticated);
            if (HttpContext.User.Identities.Any(i => i.IsAuthenticated))
            {
                SignOut();
                //SignIn();
            }
            else
            {
                SignIn();
            }

            //Request.Cookies.Clear();
            //Response.Cookies.Clear();
            return new EmptyResult();
        }

        public void SignIn()
        {
            if (!HttpContext.User.Identities.Any(i => i.IsAuthenticated))
            {
                HttpContext.ChallengeAsync(WsFederationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            }
        }

        public void SignOut()
        {            
            string callbackUrl = Url.Action("SignOutCallback", "Home"); 
            HttpContext.SignInAsync(ClaimsPrincipal.Current, new AuthenticationProperties { RedirectUri = callbackUrl });                          
        }

        public ActionResult SignOutCallback()
        {
            if (HttpContext.User.Identities.Any(i => i.IsAuthenticated))
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

    }

    public class StateVM
    {
        public ClaimsPrincipal Principal { get; set; }
        public List<HttpCookie> Cookies { get; set; }
    }

}