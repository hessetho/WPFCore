using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Helper
{
    public class StopWatch : IDisposable
    {
        private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public static StopWatch Start()
        {
            var sw = new StopWatch();
            sw.StartInternal();

            return sw;
        }

        private void StartInternal()
        {
            sw.Start();
        }

        public void Stop()
        {
            sw.Stop();
        }

        public void Restart()
        {
            sw.Restart();
        }

        public TimeSpan Elapsed
        {
            get { return sw.Elapsed; }
        }

        public void Dispose()
        {
            sw.Stop();
            Console.WriteLine("Duration: {0}", sw.Elapsed);
        }
    }
}
