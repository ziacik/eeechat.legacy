using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using KolikSoftware.Eee.Client.MainFormPlugins;
using KolikSoftware.Eee.Service;

namespace KolikSoftware.Eee.Client.PluginCore
{
    public static class PluginHelper
    {
        public static List<IEeeService> Services = new List<IEeeService>();
        public static List<IMainFormPlugin> MainFormPlugins { get; set; }

        public static void LoadPlugins()
        {
            foreach (string dllFile in Directory.GetFiles(Application.StartupPath, "*.dll"))
            {
                LoadPluginsFrom(dllFile);
            }
        }

        static void LoadPluginsFrom(string dllFile)
        {
            if (!dllFile.EndsWith("KolikSoftware.Controls.Options.dll")
                && !dllFile.EndsWith("KolikSoftware.Eee.Service.dll")
                && !dllFile.EndsWith("Interop.Shell32.dll")
                && !dllFile.EndsWith("Skybound.Gecko.dll"))
            {
                Assembly pluginAssembly = Assembly.LoadFile(dllFile);
                Type serviceType = typeof(IEeeService);
                Type mainFormPluginType = typeof(IMainFormPlugin);
                
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (serviceType.IsAssignableFrom(pluginType))
                    {
                        Services.Add((IEeeService)Activator.CreateInstance(pluginType));
                    }
                    else if (mainFormPluginType.IsAssignableFrom(pluginType))
                    {
                        MainFormPlugins.Add((IMainFormPlugin)Activator.CreateInstance(pluginType));
                    }
                }
            }
        }
    }
}
