using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SMT.Proxy.FogBugz.FogBugzObjects;

namespace SMT.Proxy.FogBugz.Web
{
    internal class FogBugzRequester
    {
        private string BaseAddress;
        private string Token;

        public FogBugzRequester(string baseUrl, string token)
        {
            BaseAddress = baseUrl;
            Token = token;
        }

        public RequestorResponse<T> MakeRequest<T>(string method, Dictionary<string, string> arguments)
            where T : class
        {
            arguments.Add("token", Token);
            arguments.Add("cmd", method);

            var argumentString = string.Join("&", arguments.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
            var url = string.Format("{0}?{1}", BaseAddress, argumentString);

            WebRequest request = HttpWebRequest.Create(url);

            string response;

            using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            using (var parseStream = new MemoryStream())
            {
                var parseWriter = new StreamWriter(parseStream);
                parseWriter.Write(response);
                parseWriter.Flush();
                parseStream.Position = 0;

                var serializer = new XmlSerializer(typeof(T));
                try
                {
                    return RequestorResponse<T>.GetSuccess(serializer.Deserialize(parseStream) as T);
                }
                catch (InvalidOperationException parseException)
                {
                    return RequestorResponse<T>.GetFailure(
                        string.Format("{0}\r\n{1}", parseException.InnerException.Message, response));
                }
            }
        }
    }
}
