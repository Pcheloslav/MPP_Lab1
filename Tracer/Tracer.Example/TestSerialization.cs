using System.Reflection;
using Tracer.Core;
using Tracer.Example;
using Tracer.Serialization.Abstractions;
using Tracer.Serialization.Plugins;

ITracer tracer = new Tracer.Core.Tracer();
var testClass1 = new TestClass1(tracer);
var testClass2 = new TestClass2(tracer);

var t1 = new Thread(() =>
{
    testClass1.M1();
    testClass1.M2();
    testClass1.M3();
});
t1.Start();

var t2 = new Thread(() =>
{
    testClass2.M1();
});
t2.Start();

testClass1.M1();
t1.Join();
t2.Join();
TraceResult result = tracer.GetTraceResult();

var serializersLocation = "..\\..\\..\\plugins\\";
var serializers = PluginsLoader.Load<ITraceResultSerializer>(serializersLocation);

var outputPath = serializersLocation + "..\\output\\";
Directory.CreateDirectory(outputPath);

foreach (var serializer in serializers)
{
    using (var fileStream = new FileStream(
        outputPath + $"test.{serializer.Format}", FileMode.Create, FileAccess.Write))
    {
        serializer.Serialize(result, fileStream);
    }
}
