using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.Performance
{
    public class SingleStopWatch : IDisposable
    {
        private DateTime? startTime;

        public event EventHandler<SingleStopWatch> Stopped;

        internal SingleStopWatch(PerformanceItem owningPerformanceItem)
        {
            this.OwningPerformanceItem = owningPerformanceItem;
        }

        public PerformanceItem OwningPerformanceItem { get; private set; }

        public void StartTiming()
        {
            this.startTime = DateTime.Now;
        }

        public void StopTiming()
        {
            if (!this.startTime.HasValue)
                throw new InvalidOperationException(string.Format("Can't stop a timer that hasn't been started. ({0}/{1})", this.OwningPerformanceItem.Category, this.OwningPerformanceItem.ItemName));

            this.StoppedTime = DateTime.Now - this.startTime.Value;
            this.OwningPerformanceItem.Add(this.StoppedTime);

            this.startTime = null;

            this.Stopped?.Invoke(this, this);
        }

        public TimeSpan StoppedTime { get; private set; }

        public TimeSpan GetCurrentTiming()
        {
            return DateTime.Now - this.startTime.Value;
        }

        public void Dispose()
        {
            this.StopTiming();
        }
    }
}
