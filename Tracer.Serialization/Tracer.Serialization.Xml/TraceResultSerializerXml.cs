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
        [XmlAttribute("name")]
        public string MethodName { get; }
        [XmlAttribute("class")]
        public string ClassName { get; }
        [XmlAttribute("time")]
        public string Time { get; }
        [XmlElement("method")]
        public List<XmlMethodTrace> Methods { get; }

        public XmlMethodTrace(string methodName, string className, long time, List<XmlMethodTrace> methods)
        {
            MethodName = methodName;
            ClassName = className;
            Time = String.Format("{0}ms", time);
            Methods = methods;
        }

        public static List<XmlMethodTrace> ToXmlMethods(IReadOnlyList<MethodTrace> methods)
        {
            if (methods.Count == 0)
            {
                return null;
            }

            List<XmlMethodTrace> traceMethods = new();
            foreach (var method in methods)
            {
                traceMethods.Add(new XmlMethodTrace(method.MethodName, method.ClassName,
                    method.Time, ToXmlMethods(method.Methods)));
            }
            return traceMethods;
        }

        public XmlMethodTrace()
        {
            MethodName = String.Empty;
            ClassName = String.Empty;
            Time = String.Empty;
            Methods = new List<XmlMethodTrace>();
        }

    }

    public class XmlThreadTrace
    {
        [XmlAttribute("id")]
        public string Id { get; }
        [XmlAttribute("time")]
        public string Time { get; }
        [XmlElement("method")]
        public List<XmlMethodTrace> Methods { get; }

        public XmlThreadTrace(int id, long time, List<XmlMethodTrace> methods)
        {
            Id = id.ToString();
            Time = String.Format("{0}ms", time);
            Methods = methods;
        }
        public XmlThreadTrace()
        {
            Id = String.Empty;
            Time = String.Empty;
            Methods = new List<XmlMethodTrace>();
        }


    }

    [XmlRoot("root")]
    public class XmlTraceResult
    {
        

        [XmlElement("thread")]
        public List<XmlThreadTrace> Threads { get; }
        public XmlTraceResult(List<XmlThreadTrace> threads)
        {
            Threads = threads;
        }
        public XmlTraceResult()
        {
        }

    }

    public class TraceResultSerializerXml : ITraceResultSerializer
    {
        private string _format = "xml";
        public string Format => _format;

        public void Serialize(TraceResult traceResult, Stream to)
        {
            /*var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };*/

            List<XmlThreadTrace> threadTraces = new();
            foreach (var thread in traceResult.Threads)
            {
                threadTraces.Add(new XmlThreadTrace(thread.Id, thread.Time,
                    XmlMethodTrace.ToXmlMethods(thread.Methods)));
            }
            var res = new XmlTraceResult(threadTraces);
            var xmlWriter = XmlWriter.Create(to, new XmlWriterSettings { Indent = true });
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlTraceResult));
            xmlSerializer.Serialize(xmlWriter, new XmlTraceResult(threadTraces));
        }
    }
}

