using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using KolikSoftware.Eee.Service;

namespace KolikSoftware.Webeee.Net.Controls
{
    public partial class UsersPanel : System.Web.UI.UserControl
    {
        EeeDataSet.UserDataTable Users
        {
            get
            {
                return (EeeDataSet.UserDataTable)Session["Users"];
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.usersList.DataSource = Users;
            this.usersList.DataBind();
        }
    }
}