using System.Reflection;
using Tracer.Core;
using Tracer.Example;
using Tracer.Serialization.Abstractions;


ITracer tracer = new Tracer.Core.Tracer();
var testClass = new TestClass(tracer);

testClass.M1();
testClass.M2();
testClass.M3();
TraceResult result = tracer.GetTraceResult();


DirectoryInfo d = new DirectoryInfo("..\\..\\..\\plugins\\");
FileInfo[] files = d.GetFiles("*.dll");

var serializerType = typeof(ITraceResultSerializer);
List<ITraceResultSerializer> serializers = new();
files.ToList().ForEach(dll => {
    var asm = Assembly.LoadFile(dll.FullName);
    var asmSerializers = asm.GetTypes().
        Where(t => serializerType.IsAssignableFrom(t)).
        Select(t => t.FullName).
        Where(t => t != null).
        Select(t => (ITraceResultSerializer?)asm.CreateInstance(t)).
        Where(inst => inst != null).
        ToList();
    serializers.AddRange(asmSerializers);
});

serializers.ForEach(serializer => {
    using (var ms = new MemoryStream())
    {
        serializer.Serialize(result, ms);
        ms.Seek(0, SeekOrigin.Begin);
        using (var sr = new StreamReader(ms))
        {
            var res = sr.ReadToEnd();
            Console.WriteLine(res);
        }
        Console.WriteLine("==========================================");
    }
});
