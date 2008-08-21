using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using KolikSoftware.Eee.Service;
using System.Security;
using System.Web.Services;

namespace KolikSoftware.Webeee.Net
{
    public partial class _Default : System.Web.UI.Page
    {
        static EeePhpService Service
        {
            get
            {
                return (EeePhpService)HttpContext.Current.Session["Service"];
            }
        }

        static EeeDataSet.UserDataTable Users
        {
            get
            {
                return (EeeDataSet.UserDataTable)HttpContext.Current.Session["Users"];
            }
            set
            {
                HttpContext.Current.Session["Users"] = value;
                UsersChanged = true;
            }
        }        

        static bool UsersChanged
        {
            get
            {
                object usersChanged = HttpContext.Current.Session["UsersChanged"];

                if (usersChanged != null)
                    return (bool)usersChanged;
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["UsersChanged"] = value;
            }
        }

        protected void connectButton_Click(object sender, EventArgs e)
        {
            Connect();
        }

        protected void disconnectButton_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        void Connect()
        {
            EeePhpService service = new EeePhpService("http://service.eeechat.net", new ProxySettings(), false, "Webeee.Net 2008", "alpha");

            SecureString passwordSecure = new SecureString();

            foreach (char passwordChar in this.passwordText.Text)
            {
                passwordSecure.AppendChar(passwordChar);
            }

            if (service.ConnectUser(this.loginText.Text, passwordSecure))
            {
                this.loginPanel.Visible = false;
                this.getMessagesTriggerPanel.Visible = true;
                this.toolbarPanel.Visible = true;
                this.scriptPanel.Visible = true;

                HttpContext.Current.Session["Service"] = service;
                HttpContext.Current.Session["Form"] = this;

                UserConfig config = UserConfigManager.Get(this.loginText.Text);

                this.workspacePanel.Visible = true;
                this.workspacePanel.Width = config.WorkspaceWidth;
                this.workspacePanel.Height = config.WorkspaceHeight;
            }
        }

        void Disconnect()
        {
            Service.DisconnectUser();
            HttpContext.Current.Session.Remove("Service");
            HttpContext.Current.Session.Remove("Users");
            HttpContext.Current.Session.Remove("UsersChanged");

            this.loginPanel.Visible = true;
            this.getMessagesTriggerPanel.Visible = false;
            this.toolbarPanel.Visible = false;
            this.scriptPanel.Visible = false;
            this.workspacePanel.Visible = false;
        }

        [WebMethod]
        public static string GetMessages()
        {
            IEeeService service = Service;

            if (service == null)
                return "";

            if (MessageHelper.Instance.Rooms == null)
                MessageHelper.Instance.Rooms = service.GetRooms();

            EeeDataSet.MessageDataTable messages = service.GetMessagesTran();

            bool hasUserChanges;

            if (Users == null)
                Users = service.GetUsers();

            string html = MessageHelper.Instance.ConvertMessages(messages, Users, service.CurrentUser.UserID, out hasUserChanges);

            if (hasUserChanges)
                UsersChanged = true;

            return html;
        }

        [WebMethod]
        public static string GetMessagesCommit()
        {
            IEeeService service = Service;

            if (service == null)
                return "";
            
            service.GetMessagesCommit();
            return "OK";
        }

        [WebMethod]
        public static string GetUsers(int selectedUserId)
        {
            IEeeService service = Service;

            if (service == null)
                return "";

            if (Users == null)
                Users = service.GetUsers();

            if (UsersChanged)
                return MessageHelper.Instance.ConvertUsers(Users, selectedUserId);
            else
                return "";
        }

        [WebMethod]
        public static string GetRooms()
        {
            if (MessageHelper.Instance.Rooms == null)
                MessageHelper.Instance.Rooms = Service.GetRooms();

            return MessageHelper.Instance.ConvertRooms(MessageHelper.Instance.Rooms);
        }

        [WebMethod]
        public static string GetUsersCommit()
        {
            UsersChanged = false;
            return "OK";
        }

        [WebMethod]
        public static void Send(string text, int recipientId, int roomId)
        {
            text = text.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                string recipient = "";

                if (text.StartsWith("/"))
                {
                    int index = text.IndexOfAny(new char[] { ' ', ':' });
                    recipient = text.Substring(1, index - 1);
                    text = text.Substring(index + 1).Trim();
                }
                else if (recipientId != 0)
                {
                    recipient = Users.FindByUserID(recipientId).Login;
                }

                Service.AddMessage(roomId, recipient, text, "");
            }
        }

        [WebMethod]
        public static void OnResize(string widthStr, string heightStr)
        {
            if (widthStr.EndsWith("px"))
                widthStr = widthStr.Substring(0, widthStr.Length - 2);

            if (heightStr.EndsWith("px"))
                heightStr = heightStr.Substring(0, heightStr.Length - 2);

            int width = int.Parse(widthStr);
            int height = int.Parse(heightStr);

            UserConfig config = UserConfigManager.Get(Service.CurrentUser.Login);
            config.WorkspaceWidth = width;
            config.WorkspaceHeight = height;
            UserConfigManager.Save(config);
        }
    }
}
