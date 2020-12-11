﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
//using System.Web.Http.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Urfu.Its.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public virtual string Roles { get; set; }
        public AuthorizeAttribute()
        { 
            
        }

        public AuthorizeAttribute(string role)
        {
            Roles = role; 
            
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isValid = context.HttpContext.User.IsInRole(Roles);
            if (!isValid)
            {
                var unauthResult = new UnauthorizedResult();

                context.Result = unauthResult;
            }

            base.OnActionExecuting(context);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ExcludeBindAttribute : Attribute, IModelNameProvider, IPropertyFilterProvider
    {
        private static readonly Func<ModelMetadata, bool> _default = (m) => true;

        private Func<ModelMetadata, bool> _propertyFilter;

        public string[] Exclude { get; }

        public string Prefix { get; set; }

        public ExcludeBindAttribute(params string[] exclude)
        {
            var items = new List<string>();
            foreach (var item in exclude)
            {
                items.AddRange(SplitString(item));
            }

            Exclude = items.ToArray();
        }

        public string Name
        {
            get { return Prefix; }
        }

        public Func<ModelMetadata, bool> PropertyFilter
        {
            get
            {
                if (Exclude != null && Exclude.Length > 0)
                {
                    if (_propertyFilter == null)
                    {
                        _propertyFilter = (m) => !Exclude.Contains(m.PropertyName, StringComparer.Ordinal);
                    }

                    return _propertyFilter;
                }
                else
                {
                    return _default;
                }
            }
        }

        private static IEnumerable<string> SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return Array.Empty<string>();
            }

            var split = original.Split(',').Select(piece => piece.Trim()).Where(piece => !string.IsNullOrEmpty(piece));

            return split;
        }
    }
}