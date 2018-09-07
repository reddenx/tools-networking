using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            //config.MapHttpAttributeRoutes();

            //var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //formatter.SerializerSettings = new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //    TypeNameHandling = TypeNameHandling.Objects,
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};
            //GlobalConfiguration.Configuration.Formatters.Clear();
            //GlobalConfiguration.Configuration.Formatters.Add(formatter);

            //var routes = DynamicApiBaseController.GetRoutes("derp");
            //foreach (var route in routes)
            //{
            //    config.Routes.MapHttpRoute(
            //        name: route.Name,
            //        routeTemplate: route.Template,
            //        defaults: route.Defaults);
            //}
        }
    }
}
