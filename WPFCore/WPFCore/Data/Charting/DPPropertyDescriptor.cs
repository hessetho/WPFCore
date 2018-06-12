using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.Charting
{
    public class DPPropertyDescriptor : PropertyDescriptor
    {
        private string valueName;

        public DPPropertyDescriptor(string name) : base(name, null)
        {
            this.valueName = name;
        }

        public override Type ComponentType => typeof(ChartDataPoint);

        public override bool IsReadOnly => false;

        public override Type PropertyType => typeof(double?);

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            var dp = (ChartDataPoint)component;
            return dp[this.valueName];
        }

        public override void ResetValue(object component)
        {
            var dp = (ChartDataPoint)component;
            dp[this.valueName] = null;
        }

        public override void SetValue(object component, object value)
        {
            var dp = (ChartDataPoint)component;
            dp[this.valueName] = (double?)value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }
}
