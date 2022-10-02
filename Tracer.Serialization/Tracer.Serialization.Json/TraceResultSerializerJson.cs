using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Json
{
    internal class JsonMethodTrace
    {
        [JsonPropertyName("name")] public string MethodName { get; }
        [JsonPropertyName("class")] public string ClassName { get; }
        [JsonPropertyName("time")] public string Time { get; }
        [JsonPropertyName("methods")] public List<JsonMethodTrace> Methods { get; }

        internal JsonMethodTrace(string methodName, string className, long time, List<JsonMethodTrace> methods)
        {
            MethodName = methodName;
            ClassName = className;
            Time = String.Format("{0}ms", time);
            Methods = methods;
        }

        internal static List<JsonMethodTrace> ToJsonMethods(IReadOnlyList<MethodTrace> methods)
        {
            if (methods.Count == 0)
            {
                return null;
            }

            List<JsonMethodTrace> traceMethods = new();
            foreach (var method in methods)
            {
                traceMethods.Add(new JsonMethodTrace(method.MethodName, method.ClassName,
                    method.Time, ToJsonMethods(method.Methods)));
            }
            return traceMethods;
        }
    }

    internal class JsonThreadTrace
    {
        [JsonPropertyName("id")] public string Id { get; }
        [JsonPropertyName("time")] public string Time { get; }
        [JsonPropertyName("methods")] public List<JsonMethodTrace> Methods { get; }

        internal JsonThreadTrace(int id, long time, List<JsonMethodTrace> methods)
        {
            Id = id.ToString();
            Time = String.Format("{0}ms", time);
            Methods = methods;
        }
    }

    internal class JsonTraceResult
    {
        [JsonPropertyName("threads")] public List<JsonThreadTrace> Threads { get; }

        public JsonTraceResult(List<JsonThreadTrace> threads)
        {
            Threads = threads;
        }
    }

    public class TraceResultSerializerJson : ITraceResultSerializer
    {
        private string _format = "json";
        public string Format => _format;

        public void Serialize(TraceResult traceResult, Stream to)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            List<JsonThreadTrace> threadTraces = new();
            foreach (var thread in traceResult.Threads)
            {
                threadTraces.Add(new JsonThreadTrace(thread.Id, thread.Time,
                    JsonMethodTrace.ToJsonMethods(thread.Methods)));
            }

            JsonSerializer.Serialize(to, new JsonTraceResult(threadTraces), options);
        }
    }
}
