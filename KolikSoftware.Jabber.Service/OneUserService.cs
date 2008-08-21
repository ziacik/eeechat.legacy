using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace KolikSoftware.Jabber.Service
{
    public class OneUserService
    {
        object lockingObj = new object();

        DateTime lastRenew;
        DateTime lastMutex;
        Queue<JabberMessage> messages = new Queue<JabberMessage>();

        string myPasswordHash;

        public string MyPasswordHash
        {
            get
            {
                return this.myPasswordHash;
            }
            set
            {
                this.myPasswordHash = value;
            }
        }

        public void Start()
        {
            this.shouldStop = false;
            RenewMutex();
            Renew();
        }

        public void AddMessage(string to, string text, string nick)
        {
            lock (this.lockingObj)
            {
                this.messages.Enqueue(new JabberMessage(to, text, nick));
            }
        }

        public JabberMessage GetNextMessage()
        {
            lock (this.lockingObj)
            {
                if (this.messages.Count > 0)
                    return this.messages.Dequeue();
                else
                    return null;
            }
        }

        public void Stop()
        {
            this.shouldStop = true;
        }

        public void Renew()
        {
            this.lastRenew = DateTime.Now;
        }

        public void RenewMutex()
        {
            this.lastMutex = DateTime.Now;
        }

        public bool ShouldDisconnect
        {
            get
            {
                return (DateTime.Now - this.lastRenew).TotalSeconds > 60;
            }
        }

        bool shouldStop = false;

        public bool ShouldStop
        {
            get
            {
                return this.shouldStop;
            }
        }

        public bool ShouldWait
        {
            get
            {
                return (DateTime.Now - this.lastMutex).TotalSeconds < 60;
            }
        }
    }
}
