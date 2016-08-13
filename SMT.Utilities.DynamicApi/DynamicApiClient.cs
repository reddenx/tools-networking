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
    public class DynamicApiClient<ApiType>
    {
        private readonly string Endpoint;
        private readonly string DynamicPrefix;

        public DynamicApiClient(string endPoint, string dynamicPrefix)
        {
            this.Endpoint = endPoint;
            this.DynamicPrefix = dynamicPrefix;
        }

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

            var result = GetResult(controllerName, methodName, parameters.ToArray());
        }

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

            var result = GetResult(controllerName, methodName, parameters.ToArray());
            var resultObj = JsonConvert.DeserializeObject(result, typeof(R));
            return (R)resultObj;
        }

        public string GetResult(string controller, string action, object[] parameters)
        {
            //buid up serialized web info
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

        private class VoidResult { }
    }
}
