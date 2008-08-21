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
using agsXMPP;
using agsXMPP.protocol.client;
using System.Threading;
using agsXMPP.net;
using System.ComponentModel;

namespace KolikSoftware.Jabber.Service
{
    public partial class Connect : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Global.RunService(Request["login"], Request["password"], Request["eeeUser"], Request["eeePasswordHash"]);            
        }
    }
}
