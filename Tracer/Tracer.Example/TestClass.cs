using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Core;

namespace Tracer.Example
{
    internal class TestClass
    {
        private ITracer _tracer;
        internal TestClass(ITracer tracer)
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
}
