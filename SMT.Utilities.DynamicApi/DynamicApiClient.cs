using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace SMT.Utilities.DynamicApi
{
    /// <summary>
    /// Used to call a generated webapi endpoint.
    /// </summary>
    /// <typeparam name="ApiType"></typeparam>
    public class DynamicApiClient<ApiType>
    {
        private readonly string Endpoint;
        private readonly string DynamicPrefix;

        /// <summary>
        /// </summary>
        /// <param name="endPoint">base endpoint of the destination api</param>
        /// <param name="dynamicPrefix">the prefix setup in that WebApi's route config for RegisterDynamicRoutes</param>
        public DynamicApiClient(string endPoint, string dynamicPrefix)
        {
            this.Endpoint = endPoint;
            this.DynamicPrefix = dynamicPrefix;
        }

        /// <summary>
        /// Call the remote api
        /// </summary>
        /// <param name="methodExpression">anonymous method call, must conform to (c => c.MethodName(Parameter1, Parameter2...))</param>
        public void Call(Expression<Action<ApiType>> methodExpression)
        {
            //build up method info
            var methodInfo = (methodExpression.Body as MethodCallExpression).Method;

            var methodName = methodInfo.Name;
            var controllerName = (typeof(ApiType).GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute).RouteName;

            //build parameters
            var parameters = new List<object>();
            foreach (var arg in (methodExpression.Body as MethodCallExpression).Arguments)
            {
                parameters.Add(Expression.Lambda<Func<object>>(Expression.Convert(arg, typeof(object)), null).Compile()());
            }

            //result for void return isn't parsed, should be []
            var result = GetResult(controllerName, methodName, parameters.ToArray());
        }

        /// <summary>
        /// Call the remote api
        /// </summary>
        /// <param name="methodExpression">anonymous method call, must conform to (c => c.MethodName(Parameter1, Parameter2...))</param>
        public R Call<R>(Expression<Func<ApiType, R>> methodExpression)
        {
            //build up method info
            var methodInfo = (methodExpression.Body as MethodCallExpression).Method;

            var methodName = methodInfo.Name;
            var controllerName = (typeof(ApiType).GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute).RouteName;

            //build parameters
            var parameters = new List<object>();
            foreach (var arg in (methodExpression.Body as MethodCallExpression).Arguments)
            {
                parameters.Add(Expression.Lambda<Func<object>>(Expression.Convert(arg, typeof(object)), null).Compile()());
            }

            //parse result
            var result = GetResult(controllerName, methodName, parameters.ToArray());
            var resultObj = JsonConvert.DeserializeObject(result, typeof(R));
            return (R)resultObj;
        }

        private string GetResult(string controller, string action, object[] parameters)
        {
            //build up route (TODO-SM centralize routing construction so this cannot be made inconsistent)
            //coupled to DynamicApiBaseController
            var template = Endpoint + "/" + DynamicPrefix + "/" + controller + "/" + action;

            var request = HttpWebRequest.CreateHttp(template);
            request.Method = "POST";
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(JsonConvert.SerializeObject(parameters));
            }

            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
