﻿using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace KolikSoftware.Webeee.Net
{
    public class ServiceManager
    {
        static readonly ServiceManager instance = new ServiceManager();

        public static ServiceManager Instance
        {
            get
            {
                return ServiceManager.instance;
            }
        }
    }
}
