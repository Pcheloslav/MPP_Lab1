using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Json
{
    public class TraceResultSerializerJson : ITraceResultSerializer
    {
        private string _format = "json";
        public string Format => _format;

        public void Serialize(TraceResult traceResult, Stream to)
        {
            var options = new JsonSerializerOptions();
            var json = JsonSerializer.Serialize(traceResult, options);

            var uniEncoding = new UnicodeEncoding();
            var jsonBytes = uniEncoding.GetBytes(json);
            to.Write(jsonBytes, 0, jsonBytes.Length);
        }
    }
}
