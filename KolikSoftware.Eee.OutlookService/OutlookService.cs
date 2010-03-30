using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using KolikSoftware.Eee.Service.Domain;
using Microsoft.Office.Interop.Outlook;

namespace KolikSoftware.Eee.Service
{
    public class OutlookService : IEeeService
    {
        protected Application OutlookApplication { get; private set; }

        public string ApplicationVersion { get; set; }

        public OutlookService()
        {
            this.PendingPosts = new List<Post>();
        }

        public IServiceConfiguration Configuration
        {
            get 
            {
                return null;
            }
        }

        public ProxySettings ProxySettings
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public AuthenticationData GetAuthenticationData(string login)
        {
            throw new NotImplementedException();
        }

        public void Connect(string login, SecureString password)
        {
            this.CurrentUser = new User { Login = login };
            this.OutlookApplication = new Application();
            this.OutlookApplication.NewMailEx += new ApplicationEvents_11_NewMailExEventHandler(OutlookApplication_NewMailEx);
        }

        public List<Post> PendingPosts { get; private set; }

        void OutlookApplication_NewMailEx(string entryIdCollection)
        {
            NameSpace nameSpace = this.OutlookApplication.GetNamespace("MAPI");
            MAPIFolder folder = this.OutlookApplication.Session.GetDefaultFolder(OlDefaultFolders.olFolderInbox);

            MailItem mail = (MailItem)nameSpace.GetItemFromID(entryIdCollection, folder.StoreID);

            if (mail.SenderEmailAddress.Contains("@"))
            {
                string text = mail.Body;

                int quoteIndex = text.IndexOf("From:");
                if (quoteIndex > 0)
                    text = text.Substring(0, quoteIndex);

                Post post = new Post
                {
                    From = new User { Login = mail.SenderEmailAddress, Client = "Mail" },
                    To = this.CurrentUser,
                    GlobalId = entryIdCollection,
                    Id = 0,
                    Room = new Room { Name = "Pokec" },
                    Text = text.Trim(),
                    Sent = DateTime.Now
                };

                mail.UnRead = false;

                this.PendingPosts.Add(post);
            }

            Marshal.ReleaseComObject(mail);
            Marshal.ReleaseComObject(folder);
            Marshal.ReleaseComObject(nameSpace);
        }

        public void Disconnect()
        {
            if (this.OutlookApplication != null)
            {
                Marshal.ReleaseComObject(this.OutlookApplication);
                this.OutlookApplication = null;
            }
        }

        public IList<User> GetUsers()
        {
            return null;
        }

        public IList<Room> GetRooms()
        {
            return null;
        }

        public IList<Post> GetMessages()
        {
            while (this.PendingPosts.Count == 0)
            {
                Thread.Sleep(1000);
            }

            Thread.Sleep(3000);

            List<Post> posts = this.PendingPosts;
            this.PendingPosts = new List<Post>();

            return posts;
        }

        public IList<Post> GetMessagesSafe()
        {
            if (this.PendingPosts.Count > 0)
            {
                Thread.Sleep(3000);

                List<Post> posts = this.PendingPosts;
                this.PendingPosts = new List<Post>();

                return posts;
            }
            else
            {
                return new List<Post>();
            }
        }

        public void CommitMessage(Post message)
        {
        }

        public User CurrentUser { get; set; }        

        public void SendMessage(Room room, User recipient, string message)
        {
            if (recipient != null && recipient.Login.IndexOf('@') < 0)
                return;

            if (message.StartsWith(recipient.Login + ":"))
                message = message.Substring(recipient.Login.Length + 1).Trim();

            MailItem mail = (MailItem)this.OutlookApplication.CreateItem(OlItemType.olMailItem);
            mail.Recipients.Add(recipient.Login);
            mail.Subject = "[Eee] " + room.Name;
            mail.Body = message;
            mail.Send();
            Marshal.ReleaseComObject(mail);
        }
    }
}
