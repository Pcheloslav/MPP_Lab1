using System.Text.Json.Serialization;

namespace Tracer.Serialization.Abstractions
{
    public class TraceResult
    {
        private string _method;
        private string _className;

        public TraceResult(string method, string className) {
            _className = className;
            _method = method;
        }

        [JsonPropertyName("method")]
        public string Method => _method;

        [JsonPropertyName("class")]
        public string ClassName => _className;
    }
}
