using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Libs;

namespace Common.Server
{
    public interface IPlugin
    {
        void Init(IServiceProvider services, Assembly[] assemblies);
    }

    public static class PluginLoader
    {
        public static void Init(IServiceProvider services, Assembly[] assemblies)
        {
            IEnumerable<Type> types = ReflectionHelper.GetInterfaceSchieves(assemblies, typeof(IPlugin)).Distinct();
            IPlugin[] plugins = types.Select(c => (IPlugin)Activator.CreateInstance(c)).ToArray();

            foreach (var item in plugins)
            {
                item.Init(services, assemblies);
            }
        }
    }
}