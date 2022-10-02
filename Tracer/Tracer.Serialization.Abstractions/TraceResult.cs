using System.Text.Json.Serialization;

namespace Tracer.Serialization.Abstractions
{
    public class MethodTrace {
        public string MethodName { get; }
        public string ClassName { get; }
        public long Time {  get; }
        public IReadOnlyList<MethodTrace> Methods { get; }

        public MethodTrace(string methodName, string className, long time, IReadOnlyList<MethodTrace> methods)
        {
            MethodName = methodName;
            ClassName = className;
            Time = time;
            Methods = methods;
        }
    }

    public class ThreadTrace {
        public int Id { get; }
        public long Time { get; }
        public IReadOnlyList<MethodTrace> Methods { get; }

        public ThreadTrace(int id, IReadOnlyList<MethodTrace> methods)
        {
            Id = id;
            Methods = methods;
            Time = methods.Sum(m => m.Time);
        }
    }

    public class TraceResult
    {
        public IReadOnlyList<ThreadTrace> Threads { get; }

        public TraceResult(IReadOnlyList<ThreadTrace> threads)
        {
            Threads = threads;
        }
    }
}
