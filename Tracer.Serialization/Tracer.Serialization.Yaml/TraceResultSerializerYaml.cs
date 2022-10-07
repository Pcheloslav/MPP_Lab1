using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Serialization.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace Tracer.Serialization.Yaml
{
    internal class YamlMethodTrace
    {
         public string MethodName { get; }
         public string ClassName { get; }
         public string Time { get; }
         public List<YamlMethodTrace> Methods { get; }

        internal YamlMethodTrace(string methodName, string className, long time, List<YamlMethodTrace> methods)
        {
            MethodName = methodName;
            ClassName = className;
            Time = String.Format("{0}ms", time);
            Methods = methods;
        }

        internal static List<YamlMethodTrace> ToYamlMethods(IReadOnlyList<MethodTrace> methods)
        {
            if (methods.Count == 0)
            {
                return null;
            }

            return methods.Select(method => new YamlMethodTrace(
                method.MethodName, method.ClassName, method.Time, ToYamlMethods(method.Methods))).ToList();
        }
    }

    internal class YamlThreadTrace
    {
         public string Id { get; }
         public string Time { get; }
         public List<YamlMethodTrace> Methods { get; }

        internal YamlThreadTrace(int id, long time, List<YamlMethodTrace> methods)
        {
            Id = id.ToString();
            Time = String.Format("{0}ms", time);
            Methods = methods;
        }
    }

    internal class YamlTraceResult
    {
         public List<YamlThreadTrace> Threads { get; }

        public YamlTraceResult(List<YamlThreadTrace> threads)
        {
            Threads = threads;
        }
    }

    public class TraceResultSerializerYaml : ITraceResultSerializer
    {
        private string _format = "Yaml";
        public string Format => _format;

        public void Serialize(TraceResult traceResult, Stream to)
        {
            List<YamlThreadTrace> threadTraces = traceResult.Threads.Select(thread => new YamlThreadTrace(
                thread.Id, thread.Time, YamlMethodTrace.ToYamlMethods(thread.Methods))).ToList();

            var serializer = new SerializerBuilder().
                WithNamingConvention(new CamelCaseNamingConvention()).Build();

            var yaml = serializer.Serialize(new YamlTraceResult(threadTraces));
            var sw = new StreamWriter(to);
            serializer.Serialize(sw, new YamlTraceResult(threadTraces));
            sw.Flush();
        }
    }
}
