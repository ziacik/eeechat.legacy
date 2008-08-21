using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace KolikSoftware.Jabber.Service
{
    public class JabberMessage
    {
        public string To;
        public string Text;
        public string Nick;

        public JabberMessage(string to, string text, string nick)
        {
            this.To = to;
            this.Text = text;
            this.Nick = nick;
        }
    }
}
