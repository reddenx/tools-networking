using SMT.Utilities.Reflection;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace IdeaDump.DynamicApiTake2
{
    public static class ClientFactory
    {
        public static Contract BuildProxy<Contract>(string baseUrl)
        {
            //do some validation
            var contractInterfaceType = typeof(Contract);
            if (!contractInterfaceType.IsInterface)
                throw new ArgumentException("contract type must be a public interface");

            if (contractInterfaceType.IsGenericType)
                throw new ArgumentException("contract type cannot be generic");

            var allMethodsRouted = contractInterfaceType.GetMethods().All(method => method.GetCustomAttributes(typeof(ContractRouteAttribute), false).Any());
            if (!allMethodsRouted)
                throw new ArgumentException("contract type must have all methods routed");

            var contractBaseRoute = contractInterfaceType.GetCustomAttributes(typeof(ContractRouteAttribute), false).FirstOrDefault() as ContractRouteAttribute;
            if (string.IsNullOrWhiteSpace(contractBaseRoute?.Route))
                throw new ArgumentException("contract type must have a base route");

            if (contractInterfaceType.GetProperties().Any() || contractInterfaceType.GetFields().Any())
                throw new ArgumentException("contract must be composed entirely of routed methods");



            //no fields or properties, has all its methods routed and contains a base url
            var routedMethods = contractInterfaceType.GetMethods().Where(method => method.GetCustomAttributes(typeof(ContractRouteAttribute), false).Any());

            var generationResult = TypeInterceptor.BuildInterceptType<Contract>();
            var proxyCaller = new BaseProxy(baseUrl, contractBaseRoute.Route);

            //build methods
            foreach (var method in routedMethods)
            {
                var routeAttribute = method.GetCustomAttributes(typeof(ContractRouteAttribute), false).Single() as ContractRouteAttribute;
                generationResult.Interceptor.SetImplementation(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray(),
                    (inputs) =>
                    {
                        return proxyCaller.MakeCall(routeAttribute.Route, inputs, method.ReturnType);
                    });
            }

            return generationResult.InterceptedInstance;
        }
    }

    internal class BaseProxy
    {
        private readonly string BaseUrl;
        private readonly string TypeUrl;

        public BaseProxy(string baseUrl, string typeUrl)
        {
            this.BaseUrl = baseUrl;
            this.TypeUrl = typeUrl;
        }

        internal object MakeCall(string methodUrl, object[] inputs, Type returnType)
        {
            //build the request
            var webRequest = HttpWebRequest.Create($"{BaseUrl.TrimEnd('/', '\\')}/{TypeUrl.Trim('/','\\')}/{methodUrl.TrimStart('/', '\\')}") as HttpWebRequest;
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(inputs);

            using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                writer.Write(json);

            webRequest.Timeout = 1000;

            try
            {
                //read the response
                using (var response = webRequest.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new GeneratedProxyRequestException(response.StatusCode, response);

                    var responseJson = string.Empty;
                    using (var reader = new StreamReader(response.GetResponseStream()))
                        responseJson = reader.ReadToEnd();

                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson, returnType);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    public class GeneratedProxyRequestException : Exception
    {
        public readonly HttpStatusCode ResponseCode;
        public readonly HttpWebResponse RawResponse;

        public GeneratedProxyRequestException(HttpStatusCode responseCode, HttpWebResponse rawResponse)
        {
            this.ResponseCode = responseCode;
            this.RawResponse = rawResponse;
        }
    }

    public class ContractRouteAttribute : Attribute
    {
        internal string Route;

        public ContractRouteAttribute(string route)
        {
            Route = route;
        }
    }
}
