using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace WPFCore.Data.Performance
{
    public class PerformanceItem : IDisposable, INotifyPropertyChanged
    {
        private string category;
        private string itemName;
        private DateTime? startTime;
        private int itemCount;
        private TimeSpan totalDuration;
        private TimeSpan minimumDuration;
        private TimeSpan maximumDuration;
        private TimeSpan latestDuration;

        public event EventHandler<PerformanceItem> Stopped;
        public event PropertyChangedEventHandler PropertyChanged;

        public PerformanceItem()
        {
        }

        internal PerformanceItem(string category, string itemName)
        {
            this.Category = category;
            this.ItemName = itemName;
        }

        public string Category
        {
            get { return this.category; }
            set
            {
                if (string.IsNullOrEmpty(this.category))
                {
                    this.category = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Category"));
                }
                else
                    throw new InvalidOperationException("Category is readonly.");

            }
        }

        public string ItemName
        {
            get { return this.itemName; }
            set
            {
                if (string.IsNullOrEmpty(this.itemName))
                {
                    this.itemName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemName"));
                }
                else
                    throw new InvalidOperationException("ItemName is readonly.");
            }
        }

        public int ItemCount
        {
            get => itemCount; set
            {
                itemCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemCount"));
            }
        }

        [XmlIgnore]
        public TimeSpan TotalDuration
        {
            get => totalDuration;
            set
            {
                totalDuration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalDuration"));
            }
        }


        [XmlIgnore]
        public TimeSpan MinimumDuration
        {
            get => minimumDuration; set
            {
                minimumDuration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinimumDuration"));
            }
        }


        [XmlIgnore]
        public TimeSpan MaximumDuration
        {
            get => maximumDuration; set
            {
                maximumDuration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaximumDuration"));
            }
        }

        [XmlIgnore]
        public TimeSpan LatestDuration
        {
            get => latestDuration; set
            {
                latestDuration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LatestDuration"));
            }
        }

        [XmlIgnore]
        public TimeSpan AverageDuration
        {
            get
            {
                if (this.ItemCount == 0)
                    return new TimeSpan(0);

                return new TimeSpan(this.TotalDuration.Ticks / this.ItemCount);
            }
        }

        [XmlElement("TotalDuration")]
        public long TotalDurationTicks
        {
            get { return this.TotalDuration.Ticks; }
            set { this.TotalDuration = new TimeSpan(value); }
        }

        [XmlElement("MinimumDuration")]
        public long MinimumDurationTicks
        {
            get { return this.MinimumDuration.Ticks; }
            set { this.MinimumDuration = new TimeSpan(value); }
        }

        [XmlElement("MaximumDuration")]
        public long MaximumDurationTicks
        {
            get { return this.MaximumDuration.Ticks; }
            set { this.MaximumDuration = new TimeSpan(value); }
        }

        [XmlElement("LatestDuration")]
        public long LatestDurationTicks
        {
            get { return this.LatestDuration.Ticks; }
            set { this.LatestDuration = new TimeSpan(value); }
        }

        public void StartTiming()
        {
            this.startTime = DateTime.Now;
        }

        public void StopTiming()
        {
            if (!this.startTime.HasValue)
                throw new InvalidOperationException(string.Format("Can't stop a timer that hasn't been started. ({0})", this.ItemName));

            this.Add(DateTime.Now - this.startTime.Value);

            this.startTime = null;

            this.Stopped?.Invoke(this, this);
        }

        public TimeSpan GetCurrentTiming()
        {
            return DateTime.Now - this.startTime.Value;
        }

        public void Add(TimeSpan duration)
        {
            this.LatestDuration = duration;

            if (this.ItemCount == 0)
            {
                this.MinimumDuration = duration;
                this.MaximumDuration = duration;
                this.TotalDuration = duration;
            }
            else
            {
                if (this.MinimumDuration > duration) this.MinimumDuration = duration;
                if (this.MaximumDuration < duration) this.MaximumDuration = duration;
                this.TotalDuration += duration;
            }

            this.ItemCount++;
        }

        public void Dispose()
        {
            this.StopTiming();
        }
    }
}