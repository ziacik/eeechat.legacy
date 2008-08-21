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

namespace KolikSoftware.Jabber.Service
{
    public partial class Send : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Global.SendMessage(Request["myPasswordHash"], Request["login"], Request["to"], Request["message"], Request["nick"]);
        }
    }
}
