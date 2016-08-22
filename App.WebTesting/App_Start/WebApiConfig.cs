using SMT.Utilities.DynamicApi.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace App.WebTesting
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var routes = DynamicApiBaseController.GetRoutes("derp");
            foreach (var route in routes)
            {
                config.Routes.MapHttpRoute(
                    name: route.Name,
                    routeTemplate: route.Template,
                    defaults: route.Defaults);
            }
        }
    }
}
