using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Proxy.FogBugz
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

        public T MakeRequest<T>(string method, Dictionary<string, string> arguments)
            where T : class
        {
            arguments.Add("token", Token);
            arguments.Add("cmd", method);

            var argumentString = string.Join("&", arguments.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

            var url = string.Format("{0}?{1}", BaseAddress, argumentString);

            WebRequest request = HttpWebRequest.Create(url);

            var serializer = new XmlSerializer(typeof(T));

            string debugStr;

            using (var stream = request.GetResponse().GetResponseStream())
            {
                debugStr = new StreamReader(stream).ReadToEnd();
                //stream.Position = 0;
            }

            var memStream = new MemoryStream();
            var writer = new StreamWriter(memStream);
            writer.Write(debugStr);
            writer.Flush();

            return serializer.Deserialize(memStream) as T;

        }
    }
}
