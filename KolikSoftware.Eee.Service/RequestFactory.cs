using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace KolikSoftware.Eee.Service
{
    public class RequestFactory
    {
        static readonly RequestFactory instance = new RequestFactory();

        public static RequestFactory Instance
        {
            get
            {
                return RequestFactory.instance;
            }
        }

        public HttpWebRequest CreateRequest(Uri uri, ProxySettings settings)
        {
            //TODO: Static proxy
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            if (settings.Server.Length > 0)
                request.Proxy = new WebProxy(settings.Server);
            else
                request.Proxy = WebRequest.GetSystemWebProxy();

            //if (this.proxySettings.NoCredentials == false)
            {
                if (settings.User.Length > 0)
                    request.Proxy.Credentials = new NetworkCredential(settings.User, settings.Password, settings.Domain);
                else
                    request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            return request;
        }

        public WebClient CreateClient(ProxySettings settings)
        {
            WebClient client = new WebClient();

            if (settings.Server.Length > 0)
                client.Proxy = new WebProxy(settings.Server);
            else
                client.Proxy = WebRequest.GetSystemWebProxy();

            //if (this.proxySettings.NoCredentials == false)
            {
                if (settings.User.Length > 0)
                    client.Proxy.Credentials = new NetworkCredential(settings.User, settings.Password, settings.Domain);
                else
                    client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            return client;
        }
    }
}
