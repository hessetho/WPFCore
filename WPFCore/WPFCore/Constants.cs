using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore
{
    public sealed class Constants
    {
        public static readonly TraceSource CoreTraceSource = new TraceSource("WPFCore");
        public static readonly TraceSource SqlTraceSource = new TraceSource("SqlTrace");
    }
}
