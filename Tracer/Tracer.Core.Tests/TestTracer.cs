using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Example;
using NUnit.Framework;
using Tracer.Core;
using Tracer.Serialization.Abstractions;
using System.Threading;

namespace Test
{
    [TestFixture]
    public class TestTracer
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void SingleThreadWithInnerMethods()
        {
            ITracer tracer = new Tracer.Core.Tracer();
            TestClass1 testclass1 = new TestClass1(tracer);
            testclass1.M1();

            TraceResult traceResult = tracer.GetTraceResult();
            Assert.That(traceResult.Threads.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[0].Methods.Count, Is.EqualTo(1));

                Assert.That(traceResult.Threads[0].Methods[0].MethodName, Is.EqualTo("M1"));
                Assert.That(traceResult.Threads[0].Methods[0].ClassName,
                    Is.EqualTo(typeof(TestClass1).FullName));
                Assert.That(traceResult.Threads[0].Methods[0].Time, Is.InRange(600, 700));
                Assert.That(traceResult.Threads[0].Methods[0].Methods.Count, Is.EqualTo(1));

                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].MethodName, Is.EqualTo("M2"));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].ClassName,
                    Is.EqualTo(typeof(TestClass1).FullName));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Time, Is.InRange(500, 600));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods.Count, Is.EqualTo(1));

                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods[0].MethodName, 
                    Is.EqualTo("M3"));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods[0].ClassName,
                    Is.EqualTo(typeof(TestClass1).FullName));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods[0].Time, 
                    Is.InRange(300, 400));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods[0].Methods.Count, 
                    Is.EqualTo(0));
            });
            Assert.That(traceResult.Threads[0].Time, Is.InRange(600, 700));
        }

        [Test]
        public void MultiThreadWithInnerMethods()
        {
            
            ITracer tracer = new Tracer.Core.Tracer();
            TestClass1 testclass1 = new TestClass1(tracer);
            TestClass2 testclass2 = new TestClass2(tracer);

            
            var t1 = new Thread(() =>
            {
                testclass1.M1();
            });

            var t2 = new Thread(() =>
            {
                testclass2.M1();
            });

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            TraceResult traceResult = tracer.GetTraceResult();

            Assert.That(traceResult.Threads.Count, Is.EqualTo(2));
            Assert.That(traceResult.Threads[0].Methods.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[0].Methods[0].MethodName, 
                    Is.EqualTo("M1"));
                Assert.That(traceResult.Threads[0].Methods[0].ClassName, 
                    Is.EqualTo(typeof(TestClass1).FullName));
                Assert.That(traceResult.Threads[0].Methods[0].Time, 
                    Is.InRange(600, 700));
                Assert.That(traceResult.Threads[0].Methods[0].Methods.Count, 
                    Is.EqualTo(1));
            });

            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].MethodName, 
                    Is.EqualTo("M2"));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].ClassName, 
                    Is.EqualTo(typeof(TestClass1).FullName));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Time, 
                    Is.InRange(500, 600));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods.Count, 
                    Is.EqualTo(1));
            });

            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods[0].MethodName, 
                    Is.EqualTo("M3"));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods[0].ClassName,
                    Is.EqualTo(typeof(TestClass1).FullName));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods[0].Time, 
                    Is.InRange(300, 400));
                Assert.That(traceResult.Threads[0].Methods[0].Methods[0].Methods[0].Methods.Count, 
                    Is.EqualTo(0));
            });

            Assert.That(traceResult.Threads[0].Time, Is.InRange(600, 700));

            Assert.That(traceResult.Threads[1].Methods.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[1].Methods[0].MethodName, Is.EqualTo("M1"));
                Assert.That(traceResult.Threads[1].Methods[0].ClassName, Is.EqualTo(typeof(TestClass2).FullName));
                Assert.That(traceResult.Threads[1].Methods[0].Time, Is.InRange(500, 600));
                Assert.That(traceResult.Threads[1].Methods[0].Methods.Count, Is.EqualTo(2));
            });

            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[1].Methods[0].Methods[0].MethodName, 
                    Is.EqualTo("M2"));
                Assert.That(traceResult.Threads[1].Methods[0].Methods[0].ClassName, 
                    Is.EqualTo(typeof(TestClass2).FullName));
                Assert.That(traceResult.Threads[1].Methods[0].Methods[0].Time, 
                    Is.InRange(200, 400));
                Assert.That(traceResult.Threads[1].Methods[0].Methods[0].Methods.Count, 
                    Is.EqualTo(0));
            });

            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[1].Methods[0].Methods[1].MethodName, Is.EqualTo("M3"));
                Assert.That(traceResult.Threads[1].Methods[0].Methods[1].ClassName, 
                    Is.EqualTo(typeof(TestClass2).FullName));
                Assert.That(traceResult.Threads[1].Methods[0].Methods[1].Time, Is.InRange(200, 400));
                Assert.That(traceResult.Threads[1].Methods[0].Methods[1].Methods.Count, Is.EqualTo(0));
            });

            Assert.That(traceResult.Threads[1].Time, Is.InRange(500, 600));
        }

        [Test]
        public void MultiThreadWithNoInnerMethods()
        {

            ITracer tracer = new Tracer.Core.Tracer();
            TestClass1 testclass1 = new TestClass1(tracer);
            TestClass2 testclass2 = new TestClass2(tracer);


            var t1 = new Thread(() =>
            {
                testclass1.M3();
            });

            var t2 = new Thread(() =>
            {
                testclass2.M3();
            });

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            TraceResult traceResult = tracer.GetTraceResult();

            Assert.That(traceResult.Threads.Count, Is.EqualTo(2));
            Assert.That(traceResult.Threads[0].Methods.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[0].Methods[0].MethodName,
                    Is.EqualTo("M3"));
                Assert.That(traceResult.Threads[0].Methods[0].ClassName,
                    Is.EqualTo(typeof(TestClass1).FullName));
                Assert.That(traceResult.Threads[0].Methods[0].Time,
                    Is.InRange(300, 400));
                Assert.That(traceResult.Threads[0].Methods[0].Methods.Count,
                    Is.EqualTo(0));
            });
            Assert.That(traceResult.Threads[0].Time, Is.InRange(300, 400));

            Assert.That(traceResult.Threads[1].Methods.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(traceResult.Threads[1].Methods[0].MethodName, Is.EqualTo("M3"));
                Assert.That(traceResult.Threads[1].Methods[0].ClassName, 
                    Is.EqualTo(typeof(TestClass2).FullName));
                Assert.That(traceResult.Threads[1].Methods[0].Time, Is.InRange(200, 300));
                Assert.That(traceResult.Threads[1].Methods[0].Methods.Count, Is.EqualTo(0));
            });
            Assert.That(traceResult.Threads[1].Time, Is.InRange(200, 300));
        }
    }
}



