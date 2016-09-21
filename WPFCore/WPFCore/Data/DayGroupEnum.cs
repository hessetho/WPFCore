using System.ComponentModel;
using WPFCore.Helper;

namespace WPFCore.Data
{
    /// <summary>
    /// Auflistung verschiedener Datumsgruppen, in welche ein Datum einsortiert werden kann.
    /// </summary>
    /// <remarks>
    /// Die Extension-Methoden <see cref="DateTimeExtensions.GetDayGroup"/> für den 
    /// Typ DateTime erlauben eine Klassifizierung eines Datums in eine der Gruppen.
    /// </remarks>
    public enum DayGroupEnum
    {
        /// <summary>
        /// Group: Today
        /// </summary>
        Today,

        /// <summary>
        /// Group: Yesterday
        /// </summary>
        Yesterday,

        /// <summary>
        /// Group: last sunday (will never occur, is either identical with "Today" or "LastWeek")
        /// </summary>
        [Description("Last sunday")]
        LastSunday,

        /// <summary>
        /// Group: last saturday (will never occur, is either identical with "Yesterday" or "LastWeek")
        /// </summary>
        [Description("Last saturday")]
        LastSaturday,

        /// <summary>
        /// Group: last friday
        /// </summary>
        [Description("Last friday")]
        LastFriday,

        /// <summary>
        /// Group: last thursday
        /// </summary>
        [Description("Last thursday")]
        LastThursday,

        /// <summary>
        /// Group: last wednesday
        /// </summary>
        [Description("Last wednesday")]
        LastWednesday,

        /// <summary>
        /// Group: last tuesday
        /// </summary>
        [Description("Last tuesday")]
        LastTuesday,

        /// <summary>
        /// Group: last monday
        /// </summary>
        [Description("Last monday")]
        LastMonday,

        /// <summary>
        /// Group: last week
        /// </summary>
        [Description("Last week")]
        LastWeek,

        /// <summary>
        /// Group: this month
        /// </summary>
        [Description("Earlier this month")]
        EarlierThisMonth,

        /// <summary>
        /// Group: last month
        /// </summary>
        [Description("Last month")]
        LastMonth,

        /// <summary>
        /// Group: earlier
        /// </summary>
        [Description("Earlier")]
        Earlier,

        /// <summary>
        /// Group: tomorrow
        /// </summary>
        Tomorrow,


        /// <summary>
        /// Group: Next wednesday
        /// </summary>
        [Description("Wednesday")]
        NextWednesday,

        /// <summary>
        /// Group: Next thursday
        /// </summary>
        [Description("Thursday")]
        NextThursday,

        /// <summary>
        /// Group: Next friday
        /// </summary>
        [Description("Friday")]
        NextFriday,

        /// <summary>
        /// Group: Next saturday (will never occur, is either identical with "Yesterday" or "NextWeek")
        /// </summary>
        [Description("Saturday")]
        NextSaturday,

        /// <summary>
        /// Group: Next sunday (will never occur, is either identical with "Today" or "NextWeek")
        /// </summary>
        [Description("Sunday")]
        NextSunday,

        /// <summary>
        /// Group: Next monday
        /// </summary>
        [Description("Next monday")]
        NextMonday,

        /// <summary>
        /// Group: Next tuesday
        /// </summary>
        [Description("Next tuesday")]
        NextTuesday,

        /// <summary>
        /// Group: next week
        /// </summary>
        [Description("Next week")]
        NextWeek,

        /// <summary>
        /// Group: next week
        /// </summary>
        [Description("Later this month")]
        LaterThisMonth,

        /// <summary>
        /// Group: next month
        /// </summary>
        [Description("Next month")]
        NextMonth,

        /// <summary>
        /// Group: later
        /// </summary>
        [Description("Later")]
        Later,

        /// <summary>
        /// Group: never
        /// </summary>
        Never
    }
}