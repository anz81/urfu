using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Urfu.Its.Web.App_Start;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Models;

namespace Urfu.Its.Web
{
    public partial class Startup
    {
        public Startup(HostingEnvironment env)
        {
           // Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; private set; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddOptions();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
            
            services.AddControllersWithViews();
            services.AddMvc();
            
            services.AddAutoMapper(typeof(Startup));
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperConfig>();
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            services.AddTransient<AuthorizeAttribute>();
            services.AddDataProtection();
            services.AddAuthentication()
              .AddCookie(options =>
              {
                  options.AccessDeniedPath = new PathString("/Security/Access");
                  options.LoginPath = new PathString("/Security/Login");
              });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
            /*var mvcOptions = new MvcOptions();
            mvcOptions.EnableEndpointRouting = false;*/
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "frm",
                    pattern: "frm");
            });
            app.Use(async (httpContext, next) =>
            {
                
                CallContext.SetData("CurrentContextKey", httpContext);
                try
                {
                    await next();
                }
                finally
                {
                    //CallContext.CallContext.SetData("CurrentContextKey", httpContext);
                }
            });
        }

        public static ConcurrentDictionary<string, UserSessionInfo> SessionKeys { get; } = new ConcurrentDictionary<string, UserSessionInfo>();

        protected void Session_Start(HttpContext context, EventArgs e)
        {
            SessionKeys.TryAdd(context.Session.Id, new UserSessionInfo());
        }
        protected void Session_End(HttpContext context, EventArgs e)
        {
            UserSessionInfo value;
            SessionKeys.TryRemove(context.Session.Id, out value);
        }

        public static void UpdateSession(string sessionId, string userName)
        {
            UserSessionInfo info;
            SessionKeys.TryGetValue(sessionId, out info);
            if (info == null)
            {
                info = new UserSessionInfo();
                SessionKeys[sessionId] = info;
            }
            if (userName != null)
                info.UserName = userName;
            info.LastSeen = DateTime.Now;
        }

        public static void Touch(HttpContext context)
        {
            string userName = null;
            if (context != null)
            {
                if (context.User != null)
                    if (context.User.Identity != null && context.User.Identity.IsAuthenticated) userName = context.User.Identity.Name;
                UpdateSession(context.Session.Id, userName);
            }
        }
    }

    public class UserSessionInfo
    {
        public string UserName { get; set; }
        public DateTime LastSeen { get; set; }
    }
}