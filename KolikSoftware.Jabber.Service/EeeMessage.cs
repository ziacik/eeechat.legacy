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
    public class EeeMessage
    {
        public string From;
        public string Text;

        public EeeMessage(string from, string text)
        {
            this.From = from;
            this.Text = text;
        }
    }
}
