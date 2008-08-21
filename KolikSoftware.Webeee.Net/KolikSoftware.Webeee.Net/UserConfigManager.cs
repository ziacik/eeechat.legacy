using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using System.IO;

namespace KolikSoftware.Webeee.Net
{
    public class UserConfigManager
    {
        public static UserConfig Get(string login)
        {
            UserConfig config = (UserConfig)HttpContext.Current.Application[login + "Config"];

            if (config == null)
            {
                config = LoadCreateConfig(login);
                HttpContext.Current.Application[login + "Config"] = config;
            }

            return config;
        }

        static UserConfig LoadCreateConfig(string login)
        {
            string path = HttpContext.Current.Server.MapPath("~/UserConfigs/" + login + ".xml");

            if (File.Exists(path))
            {
                using (FileStream inputStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UserConfig));
                    UserConfig config = (UserConfig)serializer.Deserialize(inputStream);
                    return config;
                }
            }
            else
            {
                UserConfig config = new UserConfig();
                config.Login = login;
                Save(config);
                return config;
            }
        }

        public static void Save(UserConfig config)
        {
            string path = HttpContext.Current.Server.MapPath("~/UserConfigs/" + config.Login + ".xml");

            using (FileStream outputStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UserConfig));
                serializer.Serialize(outputStream, config);
            }
        }
    }
}
