using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using agsXMPP;
using KolikSoftware.Eee.Service;
using agsXMPP.net;
using agsXMPP.protocol.client;
using System.Threading;
using System.IO;
using System.Security;
using System.Collections.Generic;
using System.Text;

namespace KolikSoftware.Jabber.Service
{
    public class ServiceClient
    {
        object lockingObj = new object();

        XmppClientConnection connection;
        IEeeService eeeService;

        string eeeUser;
        List<Exception> errors = new List<Exception>();
        Dictionary<string, string> userNames = new Dictionary<string, string>();
        Dictionary<string, bool> wasOnline = new Dictionary<string, bool>();
        Queue<EeeMessage> eeeMessages = new Queue<EeeMessage>();

        bool loggedIn = false;

        public bool HasErrors
        {
            get
            {
                return this.errors.Count > 0;
            }
        }

        public string GetErrors()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < this.errors.Count; i++)
            {
                builder.Append("Error no. ");
                builder.Append(i + 1);
                builder.Append(System.Environment.NewLine);
                builder.Append(this.errors[i].ToString());
                builder.Append(System.Environment.NewLine);
                builder.Append(System.Environment.NewLine);
            }

            return builder.ToString();
        }

        public void Connect(OneUserService userService, string login, string password, string eeeUser, string eeePasswordHash)
        {
            this.eeeUser = eeeUser;

            Jid jid = new Jid(login);

            this.connection = new XmppClientConnection(SocketConnectionType.Direct);
            this.connection.Server = jid.Server;
            this.connection.Username = jid.User;
            this.connection.Password = password;
            this.connection.Resource = "EeeChatNetwork";
            this.connection.Priority = 10;
            this.connection.Port = 5222;
            this.connection.AutoResolveConnectServer = true;
            //this.connection.UseSSL = false;
            //this.connection.UseStartTLS = false;
            this.connection.UseCompression = false;

            this.connection.OnXmppError += new XmppElementHandler(connection_OnXmppError);
            this.connection.OnXmppConnectionStateChanged += new XmppConnectionStateHandler(connection_OnXmppConnectionStateChanged);
            this.connection.OnLogin += new ObjectHandler(connection_OnLogin);
            this.connection.OnError += new ErrorHandler(connection_OnError);
            this.connection.OnAuthError += new XmppElementHandler(connection_OnAuthError);
            this.connection.OnSocketError += new ErrorHandler(connection_OnSocketError);
            this.connection.OnPresence += new PresenceHandler(connection_OnPresence);
            this.connection.OnRosterEnd += new ObjectHandler(connection_OnRosterEnd);
            this.connection.OnRosterItem += new XmppClientConnection.RosterHandler(connection_OnRosterItem);

            this.eeeService = new EeePhpService("http://www.eeechat.net/Service/Post", new ProxySettings(), false, "Eee Jabber Service", "0.1");

            if (!this.eeeService.CheckUser(eeeUser, eeePasswordHash))
            {
                AddError(new Exception("Eee user bad login."));
                return;
            }

            SecureString adminPassword = new SecureString();
            adminPassword.AppendChar('k');
            adminPassword.AppendChar('u');
            adminPassword.AppendChar('r');
            adminPassword.AppendChar('a');
            adminPassword.AppendChar('t');
            adminPassword.AppendChar('k');
            adminPassword.AppendChar('o');

            if (!this.eeeService.ConnectUser("jabberadmin", adminPassword))
            {
                AddError(new Exception("Could not connect to Eee."));
                return;
            }

            userService.MyPasswordHash = eeePasswordHash;

            this.connection.Open();

            int timeoutCountdown = 60;

            while (timeoutCountdown > 0 && this.HasErrors == false)
            {
                if (this.loggedIn)
                    break;

                Thread.Sleep(1000);
                timeoutCountdown--;
            }

            if (this.HasErrors)
                return;

            if (timeoutCountdown == 0)
            {
                AddError(new Exception("Timeout connecting."));
                return;
            }

            this.connection.OnMessage += new MessageHandler(connection_OnMessage);

            while (userService.ShouldStop == false && this.HasErrors == false && userService.ShouldDisconnect == false)
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("stop.xdi")))
                    break;

                HttpContext.Current.Response.Write('.');
                Thread.Sleep(5000);

                CheckEeeMessages();
                CheckJabberMessages(userService);

                userService.RenewMutex();
            }

            this.connection.Close();
        }

        void connection_OnRosterEnd(object sender)
        {
            Presence presence = new Presence(ShowType.NONE, "Online");
            presence.Type = PresenceType.available;
            this.connection.Send(presence);
            AddEeeMessage("JabberAdmin", "[EXTERNALSTATE ~;Off;]");
        }

        void connection_OnRosterItem(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            if (item.Name != null)
                this.userNames[item.Jid.Bare] = item.Name;
            else
                this.userNames[item.Jid.Bare] = item.Jid.User;
        }

        void AddError(Exception exception)
        {
            this.errors.Add(exception);
        }

        void connection_OnMessage(object sender, Message msg)
        {
            this.eeeService.AddMessage(0, this.eeeUser, this.eeeUser + ": " + msg.Body, msg.From.Bare);
        }

        void connection_OnSocketError(object sender, Exception ex)
        {
            AddError(new Exception("Socket error.", ex));
        }

        void connection_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            AddError(new Exception("Auth error. " + e.InnerXml));
        }

        void connection_OnError(object sender, Exception ex)
        {
            AddError(new Exception("Error.", ex));
        }

        void connection_OnLogin(object sender)
        {
            this.loggedIn = true;
        }

        void connection_OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            if (this.eeeService == null)
            {
                AddError(new Exception("Eee Service not initialized."));
            }
            else
            {
                string name;

                if (this.userNames.ContainsKey(pres.From.Bare))
                    name = this.userNames[pres.From.Bare];
                else
                    name = pres.From.Bare;

                if (name != null)
                {
                    string status;

                    if (pres.Status == null)
                        status = "";
                    else
                        status = pres.Status;

                    if (pres.Type == PresenceType.unavailable || pres.Type == PresenceType.error || pres.Type == PresenceType.invisible)
                    {
                        if (this.wasOnline.ContainsKey(pres.From.Bare))
                            AddEeeMessage(pres.From.Bare, "[EXTERNALSTATE " + name + ";Off;" + status + "]");
                    }
                    else
                    {
                        this.wasOnline[pres.From.Bare] = true;
                        AddEeeMessage(pres.From.Bare, "[EXTERNALSTATE " + name + ";On;" + status + "]");
                    }
                }
            }
        }

        void AddEeeMessage(string from, string text)
        {
            EeeMessage message = new EeeMessage(from, text);

            lock (this.lockingObj)
            {
                this.eeeMessages.Enqueue(message);
            }
        }

        void CheckEeeMessages()
        {
            lock (this.lockingObj)
            {
                while (this.eeeMessages.Count > 0)
                {
                    EeeMessage message = this.eeeMessages.Peek();

                    try
                    {
                        this.eeeService.AddMessage(0, this.eeeUser, message.Text, message.From);
                        this.eeeMessages.Dequeue();
                    }
                    catch (DisconnectedException ex)
                    {
                        AddError(new Exception("Eee service disconnected.", ex));
                        return;
                    }
                    catch (Exception)
                    {
                        /// Try it in next cycle.
                        return;
                    }
                }
            }
        }

        void CheckJabberMessages(OneUserService userService)
        {
            JabberMessage message;

            while ((message = userService.GetNextMessage()) != null)
            {
                Message jabmsg = new Message();
                jabmsg.Type = MessageType.normal;
                jabmsg.To = new Jid(message.To);
                jabmsg.Body = message.Text;
                this.connection.Send(jabmsg);

                AddEeeMessage(this.eeeUser, message.Nick + ": " + message.Text);
            }
        }

        void connection_OnXmppConnectionStateChanged(object sender, XmppConnectionState state)
        {
            if (state == XmppConnectionState.Disconnected)
                AddError(new Exception("Disconnected"));
        }

        void connection_OnXmppError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            AddError(new Exception("Xmpp Error: " + e.InnerXml));
        }
    }
}
