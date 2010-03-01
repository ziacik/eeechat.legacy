using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using KolikSoftware.Eee.Service.Core;
using KolikSoftware.Eee.Service.Domain;
using System.Security;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KolikSoftware.Eee.Service
{
    public class EeeServiceBase
    {
        public IServiceConfiguration Configuration { get; protected set; }
        public ProxySettings ProxySettings { get; set; }
        public User CurrentUser { get; protected set; }
        protected string PasswordHash { get; private set; }

        protected void CreateHash(AuthenticationData authenticationData, SecureString password)
        {
            this.PasswordHash = SecurityHelper.CreatePasswordHash(password, authenticationData.Salt);
        }

        protected string GetQueryString(params Expression<Func<object>>[] arguments)
        {
            StringBuilder builder = new StringBuilder();

            foreach (Expression<Func<object>> argument in arguments)
            {
                MemberExpression expression;

                if (argument.Body is UnaryExpression)
                    expression = (MemberExpression)((UnaryExpression)argument.Body).Operand;
                else 
                    expression = (MemberExpression)argument.Body;

                if (builder.Length > 0)
                    builder.Append('&');

                builder.Append(expression.Member.Name);
                builder.Append('=');

                Func<object> valueGetter = argument.Compile();
                object value = valueGetter();

                if (value == null)
                    value = "";

                string valueStr = HttpUtility.UrlEncode(value.ToString());
                valueStr = valueStr.Replace("+", "%20");

                builder.Append(valueStr);
            }

            string query = builder.ToString();
            return query;
        }

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

        protected T Query<T>(string method, params Expression<Func<object>>[] arguments)
        {
            string response = CallRequest(CallType.GET, method, arguments);
            return Deserialize<T>(response);
        }

        protected T Query<T>(string method, params object[] paramsAndValues)
        {
            string response = CallRequest(CallType.GET, method, paramsAndValues);
            return Deserialize<T>(response);
        }

        protected IList<T> QueryList<T>(string method, params Expression<Func<object>>[] arguments)
        {
            string response = CallRequest(CallType.GET, method, arguments);
            return Deserialize<List<T>>(response);
        }

        protected T Action<T>(string method, params Expression<Func<object>>[] arguments)
        {
            string response = CallRequest(CallType.POST, method, arguments);
            return Deserialize<T>(response);
        }

        protected ActionResult Action(string method, params Expression<Func<object>>[] arguments)
        {
            string response = CallRequest(CallType.POST, method, arguments);
            return Deserialize<ActionResult>(response);
        }

        protected T Action<T>(string method, params object[] paramsAndValues)
        {
            string response = CallRequest(CallType.POST, method, paramsAndValues);
            return Deserialize<T>(response);
        }

        protected ActionResult Action(string method, params object[] paramsAndValues)
        {
            return Action<ActionResult>(method, paramsAndValues);
        }

        enum CallType
        {
            GET,
            POST
        }

        string CallRequest(CallType callType, string method, params Expression<Func<object>>[] arguments)
        {
            string query = GetQueryString(arguments);

            string url = this.Configuration.ServiceUrl;

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
                data = Encoding.ASCII.GetBytes(query);
                request.ContentLength = data.Length;
            }

            if (callType == CallType.GET)
            {
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
            }
            else
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    if (data != null)
                        requestStream.Write(data, 0, data.Length);

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
                }
            }
        }

        string CallRequest(CallType callType, string method, params object[] paramsAndValues)
        {
            string query = GetQueryString(paramsAndValues);

            string url = this.Configuration.ServiceUrl;

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

            if (callType == CallType.GET)
            {
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
            }
            else
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    if (data != null)
                        requestStream.Write(data, 0, data.Length);

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
                }
            }
        }

        T Deserialize<T>(string json)
        {
            if (typeof(T) != typeof(ActionResult) && json.StartsWith("{\"Result\":"))
            {
                ActionResult result = Deserialize<ActionResult>(json);
                throw new Exception(result.Result);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    T obj = (T)serializer.ReadObject(ms);
                    return obj;
                }
            }
        }
    }
}
