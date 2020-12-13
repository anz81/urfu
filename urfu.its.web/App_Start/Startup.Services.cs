using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Autofac;
using Autofac.Integration.AspNetCore;
//using Owin;
using Urfu.Its.VersionedDocs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using System;

namespace Urfu.Its.Web
{
  /*  public partial class Startup
    {
        private IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterType<AuthorRepository>().
            As<IAuthorRepository>();
            var container = containerBuilder.Build();
            return container.Resolve<IServiceProvider>();


            
            builder.RegisterControllers(executingAssembly);
            builder.RegisterModelBinders(executingAssembly);
            builder.RegisterModelBinderProvider();

            // Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // Enable action parameter injection
            builder.RegisterType<ExtensibleActionInvoker>().As<IActionInvoker>();

            builder.RegisterModule(new WorkingProgramsModule(()=>HttpContext.Current.User, ()=>
            {
                var queryString = HttpContext.Current.Request.QueryString;
                return queryString.AllKeys.ToDictionary(key => key, key => queryString[key]);
            }));

            var container = builder.Build();            

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));            
        }
    }*/
}