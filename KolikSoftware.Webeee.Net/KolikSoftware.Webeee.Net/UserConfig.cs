using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KolikSoftware.Webeee.Net
{
    [XmlRoot]
    public class UserConfig
    {
        public UserConfig()
        {            
        }

        string login;

        public string Login
        {
            get
            {
                return this.login;
            }
            set
            {
                this.login = value;
            }
        }

        int workspaceWidth = 640;

        public int WorkspaceWidth
        {
            get
            {
                return this.workspaceWidth;
            }
            set
            {
                this.workspaceWidth = value;
            }
        }

        int workspaceHeight = 600;

        public int WorkspaceHeight
        {
            get
            {
                return this.workspaceHeight;
            }
            set
            {
                this.workspaceHeight = value;
            }
        }
    }
}
