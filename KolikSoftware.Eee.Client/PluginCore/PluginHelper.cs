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
        public static List<IMainFormPlugin> MainFormPlugins = new List<IMainFormPlugin>();

        public static void LoadPlugins()
        {
            foreach (string dllFile in Directory.GetFiles(Application.StartupPath, "*.dll"))
            {
                LoadPluginsFrom(dllFile);
            }

            if (Services.Count > 0 && !Services[0].Enabled)
                Services[0].Enabled = true;
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
                        Services.Add(CreateService(pluginType));
                    }
                    else if (mainFormPluginType.IsAssignableFrom(pluginType))
                    {
                        MainFormPlugins.Add((IMainFormPlugin)Activator.CreateInstance(pluginType));
                    }
                }
            }
        }

        static IEeeService CreateService(Type pluginType)
        {
            var service = (IEeeService)Activator.CreateInstance(pluginType);
            var serviceName = service.GetType().Name;
            var property = Properties.Settings.Default.PropertyValues["Service." + serviceName];
            var enabled = property != null && (bool)property.PropertyValue;
            service.Enabled = enabled;
            return service;
        }
    }
}
