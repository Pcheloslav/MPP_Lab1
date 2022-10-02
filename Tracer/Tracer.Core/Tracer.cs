using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Serialization.Abstractions;

namespace Tracer.Core
{

    public class Tracer : ITracer
    {
        private struct MethodInfo
        {
            public string MethodName { get; set; }
            public string ClassName { get; set; }
            public Stopwatch Timer { get; set; }
            public List<MethodInfo> InnerMethods { get; } = new();

            public MethodInfo(string methodName, string className, Stopwatch timer)
            {
                MethodName = methodName;
                ClassName = className;
                Timer = timer;
            }
        }

        private struct ThreadInfo
        {
            public ConcurrentStack<MethodInfo> CallStack { get; } = new();
            public List<MethodInfo> RootMethods { get; } = new();
        }

        private readonly ConcurrentDictionary<int, ThreadInfo> _threads = new();

        public void StartTrace()
        {
            var method = new StackTrace().GetFrame(1)?.GetMethod();
            if (method != null)
            {
                var methodName = method.Name;
                var className = method.DeclaringType == null ?
                    string.Empty : method.DeclaringType.FullName;
                var methodInfo = new MethodInfo(methodName, className, new Stopwatch());

                var threadId = Thread.CurrentThread.ManagedThreadId;
                var threadInfo = _threads.GetOrAdd(threadId, new ThreadInfo());

                if (threadInfo.CallStack.IsEmpty)
                {
                    threadInfo.RootMethods.Add(methodInfo);
                } else
                {
                    threadInfo.CallStack.First().InnerMethods.Add(methodInfo);
                }

                threadInfo.CallStack.Push(methodInfo);
                methodInfo.Timer.Start();
            }
        }

        public void StopTrace()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            MethodInfo methodInfo;
            if (_threads[threadId].CallStack.TryPop(out methodInfo))
            {
                methodInfo.Timer.Stop();
            }
        }

        public TraceResult GetTraceResult()
        {
            List<ThreadTrace> threads = new List<ThreadTrace>();
            foreach (var thread in _threads)
            {
                List<MethodTrace> methods = new List<MethodTrace>();
                foreach (MethodInfo method in thread.Value.RootMethods)
                {
                    methods.Add(new MethodTrace(method.MethodName, method.ClassName,
                        method.Timer.ElapsedMilliseconds, GetInnerMethods(method)));
                }
                threads.Add(new ThreadTrace(thread.Key, methods));
            }

            return new TraceResult(threads);
        }

        private static List<MethodTrace> GetInnerMethods(MethodInfo rootMethod)
        {
            List<MethodTrace> traceMethods = new();
            foreach (var method in rootMethod.InnerMethods)
            {
                traceMethods.Add(new MethodTrace(method.MethodName, method.ClassName,
                    method.Timer.ElapsedMilliseconds, GetInnerMethods(method)));
            }
            return traceMethods;
        }
    }
}
