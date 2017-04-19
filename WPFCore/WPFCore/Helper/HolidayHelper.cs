using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCore.Data;

namespace WPFCore.Helper
{
    /// <summary>
    /// Various helper functions to deteminer weekends and (german) holidays
    /// </summary>
    public static class HolidayHelper
    {
        private static object lockObj = new object();

        private static Dictionary<int, List<Holiday>> holidaysPerYear = new Dictionary<int, List<Holiday>>();

        /// <summary>
        /// Determines whether the specified date is eiter a (german public) holiday or a weekend.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static bool IsHoliday(this DateTime date)
        {
            if (GetHolidaysOfYear(date).Any(f => f.Datum.Date == date.Date))
                return true;

            return (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        /// <summary>
        /// Determines whether the specified date is a weekend.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime date)
        {
            return (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        public static Holiday GetHolidayName(DateTime date)
        {
            return (GetHolidaysOfYear(date).FirstOrDefault(f => f.Datum.Date == date.Date));
        }

        public static DateTime TPlusN(this DateTime refDate, int n)
        {
            if (n < 0)
                throw new ArgumentException("n must be >= 0", "n");

            while (n > 0)
            {
                refDate = refDate.AddDays(1);
                if (!refDate.IsHoliday())
                    n--;
            }

            return refDate;
        }

        public static DateTime TMinusN(this DateTime refDate, int n)
        {
            if (n < 0)
                throw new ArgumentException("n must be >= 0", "n");

            while (n > 0)
            {
                refDate = refDate.AddDays(-1);
                if (!refDate.IsHoliday())
                    n--;
            }

            return refDate;
        }

        public static DateTime TPlusMinusN(this DateTime refDate, int n)
        {
            if (n < 0)
                return refDate.TMinusN(-n);

            return refDate.TPlusN(n);
        }

        public static DateTime TMinus1(this DateTime refDate)
        {
            return refDate.TMinusN(1);
        }

        public static DateTime TPlus1(this DateTime refDate)
        {
            return refDate.TPlusN(1);
        }

        /// <summary>
        /// Gets the holidays of a year.
        /// </summary>
        /// <param name="date">The date representing the required year.</param>
        /// <returns></returns>
        private static List<Holiday> GetHolidaysOfYear(DateTime date)
        {
            return GetHolidaysOfYear(date.Year);
        }

        public static List<Holiday> GetHolidaysOfYear(int year)
        {
            lock (lockObj)
            {
                List<Holiday> h = null;
                if (!holidaysPerYear.TryGetValue(year, out h))
                    h = AddHolidays(year);

                return h;
            }
        }

        /// <summary>
        /// Adjusts to the previous business date if the reference date is either a holiday or a weekend.
        /// </summary>
        /// <param name="refDate">The reference date.</param>
        /// <returns></returns>
        public static DateTime AdjustToPrevBusinessDate(DateTime refDate)
        {
            while (HolidayHelper.IsHoliday(refDate))
                refDate = refDate.TMinus1();

            return refDate;
        }

        /// <summary>
        /// Adjusts to the next business date if the reference date is either a holiday or a weekend.
        /// </summary>
        /// <param name="refDate">The reference date.</param>
        /// <returns></returns>
        public static DateTime AdjustToNextBusinessDate(DateTime refDate)
        {
            while (HolidayHelper.IsHoliday(refDate))
                refDate = refDate.TPlus1();

            return refDate;
        }

        /// <summary>
        /// Creates (and returns) a list of all german public holidays for a given year and adds this to the internal holiday cache.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        private static List<Holiday> AddHolidays(int year)
        {

            var holidays = new List<Holiday>();

            holidays.Add(new Holiday(true, new DateTime(year, 1, 1), "Neujahr"));
            //holidays.Add(new Holiday(true, new DateTime(year, 1, 6), "Heilige Drei Könige"));
            holidays.Add(new Holiday(true, new DateTime(year, 5, 1), "Tag der Arbeit"));
            //holidays.Add(new Holiday(true, new DateTime(year, 8, 15), "Mariä Himmelfahrt"));
            holidays.Add(new Holiday(true, new DateTime(year, 10, 3), "Tag der dt. Einheit"));
            //holidays.Add(new Holiday(true, new DateTime(year, 10, 31), "Reformationstag"));
            //holidays.Add(new Holiday(true, new DateTime(year, 11, 1), "Allerheiligen"));
            holidays.Add(new Holiday(true, new DateTime(year, 12, 25), "1. Weihnachtstag"));
            holidays.Add(new Holiday(true, new DateTime(year, 12, 26), "2. Weihnachtstag"));

            // Silvester is always a bank holiday
            holidays.Add(new Holiday(true, new DateTime(year, 12, 31), "Silvester"));

            DateTime osterSonntag = GetOsterSonntag(year);
            holidays.Add(new Holiday(false, osterSonntag, "Ostersonntag"));
            // TH, 29.3.2016: Gründonnerstag ist KEIN Feiertag!
            //holidays.Add(new Holiday(false, osterSonntag.AddDays(-3), "Gründonnerstag"));
            holidays.Add(new Holiday(false, osterSonntag.AddDays(-2), "Karfreitag"));
            holidays.Add(new Holiday(false, osterSonntag.AddDays(1), "Ostermontag"));
            holidays.Add(new Holiday(false, osterSonntag.AddDays(39), "Christi Himmelfahrt"));
            holidays.Add(new Holiday(false, osterSonntag.AddDays(49), "Pfingstsonntag"));
            holidays.Add(new Holiday(false, osterSonntag.AddDays(50), "Pfingstmontag"));
            holidays.Add(new Holiday(false, osterSonntag.AddDays(60), "Fronleichnam"));

            holidaysPerYear.Add(year, holidays);

            return holidays;
        }

        /// <summary>
        /// Calculates the date of "Ostersonntag"
        /// </summary>
        /// <returns></returns>
        private static DateTime GetOsterSonntag(int year)
        {

            int g, h, c, j, l, i;

            g = year % 19;
            c = year / 100;
            h = ((c - (c / 4)) - (((8 * c) + 13) / 25) + (19 * g) + 15) % 30;
            i = h - (h / 28) * (1 - (29 / (h + 1)) * ((21 - g) / 11));
            j = (year + (year / 4) + i + 2 - c + (c / 4)) % 7;

            l = i - j;
            int month = (int)(3 + ((l + 40) / 44));
            int day = (int)(l + 28 - 31 * (month / 4));

            return new DateTime(year, month, day);
        }
    }
}
