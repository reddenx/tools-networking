using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SMT.Utilities.DynamicApi.Dto;

namespace SMT.Utilities.DynamicApi.Api
{
	/// <summary>
	/// Acts as a controller for all dynamically generated Apis, It does internal routing to ensure methods are routed to the correct api
	/// </summary>
	public class DynamicApiBaseController//: ApiController
	{
		private static Type[] DynamicApis;

		//server side: given the interface and implementation of the object that is proxied, build out a controller that calls into this object

		/// <summary>
		/// Must be called during webapi configuration setup
		/// </summary>
		/// <param name="routes">the configured routes of the application</param>
		/// <param name="prefix">the prefix for all dynamic endpoints, built to BaseUrl/{prefix}/DestinationObjectApi/Action</param>
		public static RouteDefinition[] GetRoutes(string prefix)
		{
			//get all interfaces with the attribute
			var dynamicApis = Assembly.GetCallingAssembly().GetTypes()
				.Where(type => type.GetInterfaces().Any(i => i.GetCustomAttribute(typeof(DynamicApiAttribute)) != null));

            var routes = new List<RouteDefinition>();
			//gte all implementors of that attribute and register them
			foreach (var api in dynamicApis)
			{
				var routeAttr = api.GetInterfaces().First(i => i.GetCustomAttribute(typeof(DynamicApiAttribute)) != null).GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute;
				var route = routeAttr.RouteName;
				var template = prefix + "/{DynamicObject}/{DestinationMethod}";

				routes.Add(new RouteDefinition()
                {
                    Name = route,
                    Template = template,
                    Defaults = new { controller = "DynamicApiBase", action = "RunDynamic" }
                });
			}

            DynamicApis = dynamicApis.ToArray();
            return routes.ToArray();
		}

		/// <summary>
		/// Not meant to be called directly, this is the entry point for all incoming api calls from the WebApi framework.
		/// </summary>
		/// <returns></returns>
		public static object RunDynamic(IDictionary<string, object> routeData, string requestBody)
		{
			//build up routing information since this is the generic input for everything
            var destObjName = routeData["DynamicObject"] as string;
            var destMethod = routeData["DestinationMethod"] as string;


			if (destObjName == null || destMethod == null)
			{
                throw new System.Net.WebException("Not found");// Exception(HttpStatusCode.NotFound);
			}

			var destObjType = DynamicApis.SingleOrDefault(type => (type.GetInterfaces().First(i => i.GetCustomAttribute(typeof(DynamicApiAttribute)) != null).GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute).RouteName == destObjName);

			if (destObjType == null)
			{
                throw new System.Net.WebException("Not found");// Exception(HttpStatusCode.NotFound);
                //throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			var destObj = Activator.CreateInstance(destObjType);

			var objMethod = destObjType.GetMethod(destMethod);

			var methodParameters = objMethod.GetParameters();
			var parameters = new List<object>();

			//deserialize input parameters
			if (methodParameters.Any())
			{
				var inputList = JArray.Parse(requestBody);

				for (int i = 0; i < methodParameters.Length; ++i)
				{
					parameters.Add(inputList[i].ToObject(methodParameters[i].ParameterType));
				}
			}

			var response = objMethod.Invoke(destObj, parameters.ToArray());
			return response;
		}
	}
}
