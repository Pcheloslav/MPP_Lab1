using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Core;

namespace Tracer.Example
{
    internal class TestClass1
    {
        private ITracer _tracer;
        internal TestClass1(ITracer tracer)
        {
            _tracer = tracer;
        }

        internal void M1()
        {
            _tracer.StartTrace();
            M2();
            Thread.Sleep(100);
            _tracer.StopTrace();
        }

        internal void M2()
        {
            _tracer.StartTrace();
            Thread.Sleep(200);
            M3();
            _tracer.StopTrace();
        }

        internal void M3()
        {
            _tracer.StartTrace();
            Thread.Sleep(300);
            _tracer.StopTrace();
        }
    }

    public class TestClass2
    {
        private ITracer _tracer;

        public TestClass2(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void M1()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            M2();
            M3();
            _tracer.StopTrace();
        }

        public void M2()
        {
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }

        public void M3()
        {
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }
    }
}
