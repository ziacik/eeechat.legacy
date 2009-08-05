using System;
using System.Collections.Generic;
using System.Text;
using KolikSoftware.Eee.Service;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace KolikSoftware.Eee.Client
{
    internal sealed class Global
    {
        static readonly Global instance = new Global();

        private Global()
        {
        }

        public static Global Instance
        {
            get
            {
                return Global.instance;
            }
        }

        public IEeeService CreateService()
        {
            ProxySettings proxySettings = new ProxySettings();
            proxySettings.Server = Properties.Settings.Default.ProxyServer;
            proxySettings.Domain = Properties.Settings.Default.ProxyDomain;
            proxySettings.User = Properties.Settings.Default.ProxyUser;
            proxySettings.Password = Properties.Settings.Default.ProxyPassword;
            proxySettings.NoCredentials = Properties.Settings.Default.ProxyUser.Trim().Length == 0;

            return new EeeBindService(Properties.Settings.Default.ServiceUrl, proxySettings, this.Installed, this.ApplicationAndVersion, Application.ProductVersion);

            //TODO: 
            if (Properties.Settings.Default.ServiceUrl.ToLower().Contains("enc"))
            {
                return new EeeEncService(Properties.Settings.Default.ServiceUrl, proxySettings, this.Installed, this.ApplicationAndVersion, Application.ProductVersion);
            }
            else
            {
                return new EeePhpService(Properties.Settings.Default.ServiceUrl, proxySettings, this.Installed, this.ApplicationAndVersion, Application.ProductVersion);
            }
        }

        public string ApplicationName
        {
            get
            {
                return Application.ProductName;
            }
        }

        public string ApplicationAndVersion
        {
            get
            {
                return this.ApplicationName + " (" + this.Version + ")";
            }
        }

        public string Version
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return version.Major.ToString() + "." + version.Minor.ToString() + " (Bind)"; //TODO:
            }
        }

        bool installed;
        bool installedSet = false;

        public bool Installed
        {
            get
            {
                if (!this.installedSet)
                    SetInstalled();
                return this.installed;
            }
        }

        void SetInstalled()
        {
            RegistryKey appKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");

            this.installed = false;

            foreach (string subKeyName in appKey.GetSubKeyNames())
            {
                if (subKeyName[0] == '{')
                {
                    RegistryKey subKey = appKey.OpenSubKey(subKeyName);
                    string displayName = (string)subKey.GetValue("DisplayName", "");
                    if (displayName.StartsWith("Eee Client"))
                    {
                        this.installed = true;
                        break;
                    }
                }
            }

            this.installedSet = true;
        }
    }
}
