using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.Performance
{
    public class DbActivityCollector
    {
        private readonly List<DbActivityItem> activityItems = new List<DbActivityItem>();

        public void AddDbActivity(string activity, string activityTargetName, DbActivityTypeEnum activityType, int rowsAffected, TimeSpan activityDuration, string additionalInformation = "")
        {
            var entry = new DbActivityItem(activity, activityTargetName, activityType, rowsAffected, activityDuration, additionalInformation);

            this.activityItems.Add(entry);
        }

        public List<DbActivityItem> ActivityItems => this.activityItems;
    }
}
