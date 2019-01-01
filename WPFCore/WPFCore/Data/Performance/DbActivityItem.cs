using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.Performance
{
    public enum DbActivityTypeEnum
    {
        Select, Insert, Update, Delete, Merge, InsertUpdate
    }

    public class DbActivityItem : INotifyPropertyChanged
    {
        public DbActivityItem(string activity, string activityTargetName, DbActivityTypeEnum activityType, int rowsAffected, TimeSpan activityDuration, string additionalInformation = "")
        {
            this.EntryTime = DateTime.Now;
            this.Activity = activity;
            this.ActivityTarget = activityTargetName;
            this.DbActivityType = activityType;
            this.RowsAffected = rowsAffected;
            this.ActivityDuration = activityDuration;
            this.AdditionalInformation = additionalInformation;
        }

        public DateTime EntryTime { get; private set; }

        /// <summary>
        /// Name of the activity (e.g. GetContacts)
        /// </summary>
        public string Activity { get; set; }

        /// <summary>
        /// Name of the (database) target of the activity (e.g. contact)
        /// </summary>
        public string ActivityTarget { get; set; }

        /// <summary>
        /// Type of the database activity.
        /// </summary>
        public DbActivityTypeEnum DbActivityType { get; set; }

        /// <summary>
        /// Number of rows affected by the activity
        /// </summary>
        public int RowsAffected { get; set; }

        /// <summary>
        /// Time elapsed for the database activity
        /// </summary>
        public TimeSpan ActivityDuration { get; set; }

        public string AdditionalInformation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
