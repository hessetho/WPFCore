using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.Charting
{
    /// <summary>
    /// Repräsentiert einen Datenwert, der überschrieben werden kann und dabei Min, Max und Durchschnitt der gespeicherten Werte aufzeichnet.
    /// Geeignet z.B. für CandelSticks.
    /// </summary>
    public class MMAValue
    {
        public double? Min { get; private set; }
        public double? Max { get; private set; }
        public double? Last { get; private set; }

        private int count = 0;
        private double? total;

        public MMAValue() { }

        public MMAValue(double? value)
        {
            this.SetValue(value);
        }

        public void SetValue(double? value)
        {
            if (!value.HasValue || double.IsNaN(value.Value)) return;

            var v = value.Value;
            if(this.count==0)
            {
                this.Min = v;
                this.Max = v;
                this.Last = v;
                this.count = 1;
                this.total = v;
            }
            else
            {
                this.Last = v;
                this.count++;
                this.total += v;
                if (v < this.Min)
                    this.Min = v;
                else if (v > this.Max)
                    this.Max = v;
            }
        }
        public double? Average
        {
            get
            {
                if (count == 0)
                    return null;

                return this.total / this.count;
            }
        }
    }
}
