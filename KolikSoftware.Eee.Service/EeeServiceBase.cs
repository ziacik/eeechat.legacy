using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using KolikSoftware.Eee.Service.Core;

namespace KolikSoftware.Eee.Service
{
    public class EeeServiceBase
    {
        public string ServiceUrl { get; set; }
        public ProxySettings ProxySettings { get; set; }

        protected string GetQueryString(params object[] paramsAndValues)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < paramsAndValues.Length; i++)
            {
                if (i != 0)
                    builder.Append("&");

                string param = paramsAndValues[i].ToString();
                i++;

                object objVal = paramsAndValues[i];
                if (objVal == null) objVal = "";

                string value = HttpUtility.UrlEncode(objVal.ToString());
                value = value.Replace("+", "%20");

                builder.Append(param);
                builder.Append("=");
                builder.Append(value);
            }

            string query = builder.ToString();
            return query;
        }

        protected T Query<T>(string method, params object[] paramsAndValues)
        {
            string response = CallRequest(CallType.GET, method, paramsAndValues);
            return Deserialise<T>(response);
        }

        enum CallType
        {
            GET,
            POST
        }

        string CallRequest(CallType callType, string method, params object[] paramsAndValues)
        {
            string query = GetQueryString(paramsAndValues);

            string url = this.ServiceUrl;

            if (url.EndsWith("/") == false)
                url += "/";

            Uri uri;

            method += ".php";

            if (callType == CallType.GET)
                uri = new Uri(url + method + "?" + query);
            else
                uri = new Uri(url + method);

            HttpWebRequest request = (HttpWebRequest)RequestFactory.Instance.CreateRequest(uri, this.ProxySettings);
            request.KeepAlive = false;
            //request.Timeout = 15000;
            request.Timeout = Timeout.Infinite;

            if (callType == CallType.POST)
                request.ContentType = "application/x-www-form-urlencoded";

            request.Method = callType.ToString();
            request.AllowAutoRedirect = false;
            request.ProtocolVersion = HttpVersion.Version11;

            byte[] data = null;

            if (callType == CallType.POST)
            {
                data = Encoding.ASCII.GetBytes(GetQueryString(paramsAndValues));
                request.ContentLength = data.Length;
            }

            //using (Stream requestStream = request.GetRequestStream())
            //{
            //    if (data != null)
            //        requestStream.Write(data, 0, data.Length);

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string text = reader.ReadToEnd();
                            return text;
                        }
                    }
                }
            //}
        }

        T Deserialise<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();

            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        }
    }
}
