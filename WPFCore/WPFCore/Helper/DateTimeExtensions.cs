using System;
using System.Globalization;
using WPFCore.Data;

namespace WPFCore.Helper
{
    /// <summary>
    /// Erweiterungs-Methoden für den Datentyp <see cref="DateTime"/>
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Klassifiziert ein Datum in eine Gruppe der <see cref="DayGroupEnum"/>-Auflistung
        /// </summary>
        /// <remarks>
        /// Die Klassifikation erfolgt in Bezug auf das aktuelle Datum, d.h. entspricht das verwendete Datum dem aktuellen
        /// Datum, wird die Gruppe "Today" geliefert.
        /// </remarks>
        /// <param name="date">Das zu gruppierende Datum.</param>
        /// <returns>Die Gruppe</returns>
        public static DayGroupEnum GetDayGroup(this DateTime? date)
        {
            return GetDayGroup(date, DateTime.Today);
        }

        public static DayGroupEnum GetDayGroup(this DateTime date)
        {
            return GetDayGroup((DateTime?)date, DateTime.Today);
        }

        /// <summary>
        /// Klassifiziert ein Datum in eine Gruppe der <see cref="DayGroupEnum"/>-Auflistung
        /// </summary>
        /// <remarks>
        /// Die Klassifikation erfolgt in Bezug auf ein Referenzdatum, d.h. sind das verwendete Datum und das Referenzdatum
        /// identisch, wird die Gruppe "Today" geliefert.
        /// Die Gruppen "Today" und "Yesterday" werden direkt ermittelt und zurück gegeben.
        /// Liegt das Datum innerhalb derselben Woche (mit Montag als erstem Wochentag), wird der Wochentag zurück gegeben.
        /// Liegt das Datum innerhalb der Vorwoche (wird anhand der Kalenderwoche ermittelt), wird "LastWeek" zurück gegeben.
        /// Liegt das Datum innerhalb desselben Monats desselben Jahres, wird "ThisMonth" zurück gegeben.
        /// Liegt das Datum im Vormonat, wird "LastMonth" zurück gegeben.
        /// Andernfalls wird "Earlier" zurück gegeben.
        /// </remarks>
        /// <param name="date">Das zu gruppierende Datum.</param>
        /// <param name="refDate">Das Referenzdatum.</param>
        /// <returns>Die Gruppe</returns>
        public static DayGroupEnum GetDayGroup(this DateTime? date, DateTime refDate)
        {
            if (date.HasValue == false)
                return DayGroupEnum.Never;

            var dt = date.Value;

            int week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int currentweek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(refDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            int month = dt.Year * 12 + dt.Month;
            int currentmonth = refDate.Year * 12 + refDate.Month;

            if (dt == refDate)
            {
                return DayGroupEnum.Today;
            }

            if (dt == refDate.AddDays(-1))
            {
                return DayGroupEnum.Yesterday;
            }

            if (dt == refDate.AddDays(1))
            {
                return DayGroupEnum.Tomorrow;
            }

            if (week == currentweek && dt.Year == refDate.Year)
            {
                string weekday = dt.DayOfWeek.ToString();
                if (dt < refDate)
                    weekday = "Last" + weekday;
                else
                    weekday = "Next" + weekday;

                return (DayGroupEnum)Enum.Parse(typeof(DayGroupEnum), weekday);
            }

            if (week == currentweek - 1 && dt.Year == refDate.Year)
            {
                return DayGroupEnum.LastWeek;
            }

            if (week < currentweek - 1 && dt.Year == refDate.Year && dt.Month == refDate.Month)
            {
                return DayGroupEnum.EarlierThisMonth;
            }
            

            if (week == currentweek + 1 && dt.Year == refDate.Year)
            {
                return DayGroupEnum.NextWeek;
            }

            if (week > currentweek + 1 && dt.Year == refDate.Year && dt.Month == refDate.Month)
            {
                return DayGroupEnum.LaterThisMonth;
            }

            if (month == currentmonth - 1)
            {
                return DayGroupEnum.LastMonth;
            }

            if (month == currentmonth + 1)
            {
                return DayGroupEnum.NextMonth;
            }

            if (dt > refDate)
                return DayGroupEnum.Later;

            return DayGroupEnum.Earlier;
        }

        public static DateTime FirstOfMonth(this DateTime refDate)
        {
            return new DateTime(refDate.Year, refDate.Month, 1);
        }

        public static DateTime LastOfMonth(this DateTime refDate)
        {
            return FirstOfMonth(refDate).AddMonths(1).AddDays(-1);
        }

        public static int GetIso8601WeekOfYear(this DateTime refDate)
        {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(refDate);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                refDate = refDate.AddDays(3);

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(refDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}

