using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Plugins
{
    public class PluginsLoader
    {
        public static List<T> Load<T>(string directory)
        {
            DirectoryInfo d = new DirectoryInfo(directory);
            FileInfo[] files = d.GetFiles("*.dll");
            var pluginType = typeof(ITraceResultSerializer);
            List<T> plugins = new();
            files.ToList().ForEach(dll => {
                var asm = Assembly.LoadFile(dll.FullName);
                var asmPlugins = asm.GetTypes().
                    Where(t => pluginType.IsAssignableFrom(t)).
                    Select(t => t.FullName).
                    Where(t => t != null).
                    Select(t => (T?) asm.CreateInstance(t)).
                    Where(inst => inst != null).
                    ToList();

                #pragma warning disable CS8620
                plugins.AddRange(asmPlugins);
                #pragma warning restore CS8620
            });
            return plugins;
        }
    }
}
