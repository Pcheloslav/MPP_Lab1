using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Xml
{
    public class XmlMethodTrace
    {
        [XmlAttribute("name")] public string MethodName;
        [XmlAttribute("class")] public string ClassName;
        [XmlAttribute("time")] public string Time;
        [XmlElement("methods")] public List<XmlMethodTrace> Methods;

        public XmlMethodTrace()
        {
            MethodName = string.Empty;
            ClassName = string.Empty;
            Time = string.Empty;
            Methods = new List<XmlMethodTrace>();
        }

        public XmlMethodTrace(string methodName, string className, long time, List<XmlMethodTrace> methods)
        {
            MethodName = methodName;
            ClassName = className;
            Time = string.Format("{0}ms", time);
            Methods = methods;
        }

        public static List<XmlMethodTrace> ToXmlMethods(IReadOnlyList<MethodTrace> methods)
        {
            if (methods.Count == 0)
            {
                return null;
            }

            return methods.Select(method => new XmlMethodTrace(
                method.MethodName, method.ClassName, method.Time, ToXmlMethods(method.Methods))).ToList();
        }
    }

    public class XmlThreadTrace
    {
        [XmlAttribute("id")] public string Id;
        [XmlAttribute("time")] public string Time;
        [XmlElement("method")] public List<XmlMethodTrace> Methods;

        public XmlThreadTrace()
        {
            Id = string.Empty;
            Time = string.Empty;
            Methods = new List<XmlMethodTrace>();
        }

        public XmlThreadTrace(int id, long time, List<XmlMethodTrace> methods)
        {
            Id = id.ToString();
            Time = string.Format("{0}ms", time);
            Methods = methods;
        }
    }

    [XmlRoot("root")]
    public class XmlTraceResult
    {
        [XmlElement("thread")] public List<XmlThreadTrace> Threads { get; }

        public XmlTraceResult()
        {
            Threads = new List<XmlThreadTrace>();
        }

        public XmlTraceResult(List<XmlThreadTrace> threads)
        {
            Threads = threads;
        }
    }

    public class TraceResultSerializerXml : ITraceResultSerializer
    {
        private string _format = "xml";
        public string Format => _format;

        public void Serialize(TraceResult traceResult, Stream to)
        {
            List<XmlThreadTrace> threadTraces = traceResult.Threads.Select(thread => new XmlThreadTrace(
                thread.Id, thread.Time, XmlMethodTrace.ToXmlMethods(thread.Methods))).ToList();

            var res = new XmlTraceResult(threadTraces);
            using (var xmlWriter = XmlWriter.Create(to, new XmlWriterSettings { Indent = true })) {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlTraceResult));
                xmlSerializer.Serialize(xmlWriter, new XmlTraceResult(threadTraces));
            }
        }
    }
}
