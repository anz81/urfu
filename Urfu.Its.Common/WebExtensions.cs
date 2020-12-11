using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Routing;

namespace Urfu.Its.Common
{
    public static class WebExtensions
    {
        public static RouteValueDictionary QueryStringToRouteValueDictionary(this HttpRequestBase request)
        {
            var queryParameters = new RouteValueDictionary();
            foreach (string key in request.QueryString.Keys)
            {
                queryParameters[key] = request.QueryString[key];
            }
            return queryParameters;
        }
    }
}