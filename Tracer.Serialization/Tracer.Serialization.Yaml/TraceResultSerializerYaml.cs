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

            List<YamlMethodTrace> traceMethods = new();
            foreach (var method in methods)
            {
                traceMethods.Add(new YamlMethodTrace(method.MethodName, method.ClassName,
                    method.Time, ToYamlMethods(method.Methods)));
            }
            return traceMethods;
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
            /*var options = new YamlSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = YamlIgnoreCondition.WhenWritingNull
            };*/

            List<YamlThreadTrace> threadTraces = new();
            foreach (var thread in traceResult.Threads)
            {
                threadTraces.Add(new YamlThreadTrace(thread.Id, thread.Time,
                    YamlMethodTrace.ToYamlMethods(thread.Methods)));
            }

            var serializer = new SerializerBuilder()//.WithNamingConvention(CamelCaseNamingConvention.Instance)
                           .Build();

            var yaml = serializer.Serialize(new YamlTraceResult(threadTraces));
            var sw = new StreamWriter(to);
            sw.Write(yaml);
            sw.Flush();
            //YamlSerializer.Serialize(to, new YamlTraceResult(threadTraces), options);
        }
    }
}
