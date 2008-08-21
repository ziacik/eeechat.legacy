using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Threading;
using agsXMPP.protocol.sasl;

namespace KolikSoftware.Jabber.Service
{
    public class Global : System.Web.HttpApplication
    {
        static object LockingObj = new object();

        static Dictionary<string, OneUserService> UserServices = new Dictionary<string, OneUserService>();

        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        internal static void RunService(string login, string pasword, string eeeUser, string eeePasswordHash)
        {
            OneUserService userService = null;

            lock (Global.LockingObj)
            {
                if (UserServices.ContainsKey(login))
                {
                    userService = UserServices[login];
                }
                else
                {
                    userService = new OneUserService();
                    UserServices.Add(login, userService);
                }
            }

            /// If another request is active for current user, ask it to stop and wait until it stops actually or times out.
            if (userService.ShouldWait)
            {
                HttpContext.Current.Response.Write("<EeeResponse>Another service already running for this user.</EeeResponse>");
                return;
            }

            DoService(userService, login, pasword, eeeUser, eeePasswordHash);
        }

        static void DoService(OneUserService userService, string login, string password, string eeeUser, string eeePasswordHash)
        {
            userService.Start();
            HttpContext.Current.Response.Write("Verzia 2<br />");
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Write("<pre>");
            ServiceClient client = new ServiceClient();
            client.Connect(userService, login, password, eeeUser, eeePasswordHash);
            if (client.HasErrors)
                HttpContext.Current.Response.Write("<EeeResponse>" + client.GetErrors() + "</EeeResponse>");
            else
                HttpContext.Current.Response.Write("<EeeResponse>OK</EeeResponse>");
            HttpContext.Current.Response.Write("</pre>");
        }

        internal static void RenewService(string login)
        {
            lock (Global.LockingObj)
            {
                if (UserServices.ContainsKey(login))
                {
                    OneUserService userService = UserServices[login];
                    userService.Renew();
                    HttpContext.Current.Response.Write("<EeeResponse>OK</EeeResponse>");
                }
                else
                {
                    HttpContext.Current.Response.Write("<EeeResponse>User not connected</EeeResponse>");
                }
            }
        }

        internal static void SendMessage(string myPasswordHash, string login, string to, string message, string nick)
        {
            lock (Global.LockingObj)
            {
                if (UserServices.ContainsKey(login))
                {
                    OneUserService userService = UserServices[login];

                    if (userService.MyPasswordHash != myPasswordHash)
                    {
                        HttpContext.Current.Response.Write("<EeeResponse>Not allowed: Invalid password." + myPasswordHash + "!=" + userService.MyPasswordHash + "</EeeResponse>");
                    }
                    else
                    {
                        userService.AddMessage(to, message, nick);
                        HttpContext.Current.Response.Write("<EeeResponse>OK</EeeResponse>");
                    }
                }
                else
                {
                    HttpContext.Current.Response.Write("<EeeResponse>User not connected.</EeeResponse>");
                }
            }
        }

        internal static void StopService(string myPasswordHash, string login)
        {
            lock (Global.LockingObj)
            {
                if (UserServices.ContainsKey(login))
                {
                    OneUserService userService = UserServices[login];

                    if (userService.MyPasswordHash != myPasswordHash)
                    {
                        HttpContext.Current.Response.Write("<EeeResponse>Not allowed: Invalid password." + myPasswordHash + "!=" + userService.MyPasswordHash + "</EeeResponse>");
                    }
                    else
                    {
                        userService.Stop();
                        HttpContext.Current.Response.Write("<EeeResponse>OK</EeeResponse>");
                    }
                }
                else
                {
                    HttpContext.Current.Response.Write("<EeeResponse>User not connected.</EeeResponse>");
                }
            }
        }
    }
}