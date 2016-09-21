using System;
using WPFCore.Helper;

namespace WPFCore.Data
{
    public static class DateRanges
    {

        public static DateRange MonthToDay()
        {
            return MonthToDay(DateTime.Today);
        }

        public static DateRange MonthToDay(DateTime refDate)
        {
            return new DateRange(refDate.FirstOfMonth(), refDate);
        }

        public static DateRange CurrentMonth()
        {
            return CurrentMonth(DateTime.Today);
        }

        public static DateRange CurrentMonth(DateTime refDate)
        {
            return new DateRange(refDate.FirstOfMonth(), refDate.LastOfMonth());
        }

        public static DateRange PreviousMonth()
        {
            return PreviousMonth(DateTime.Today);
        }

        public static DateRange PreviousMonth(DateTime refDate)
        {
            refDate = refDate.AddMonths(-1);
            return new DateRange(refDate.FirstOfMonth(), refDate.LastOfMonth());
        }

        public static DateRange YearToDay()
        {
            return YearToDay(DateTime.Today);
        }

        public static DateRange YearToDay(DateTime refDate)
        {
            return new DateRange(new DateTime(refDate.Year, 1, 1), refDate);
        }

        public static DateRange CurrentYear()
        {
            return CurrentYear(DateTime.Today);
        }

        public static DateRange CurrentYear(DateTime refDate)
        {
            return new DateRange(new DateTime(refDate.Year, 1, 1), new DateTime(refDate.Year, 12, 31));
        }

        public static DateRange PreviousYear()
        {
            return PreviousYear(DateTime.Today);
        }

        public static DateRange PreviousYear(DateTime refDate)
        {
            return new DateRange(new DateTime(refDate.Year - 1, 1, 1), new DateTime(refDate.Year - 1, 12, 31));
        }

        public static DateRange Last12Months()
        {
            return Last12Months(DateTime.Today);
        }

        public static DateRange Last12Months(DateTime refDate)
        {
            refDate = refDate.AddYears(-1).AddDays(1);
            return new DateRange(refDate, DateTime.Today);
        }

        public static DateRange LastThreeMonths()
        {
            return LastThreeMonths(DateTime.Today);
        }

        public static DateRange LastThreeMonths(DateTime refDate)
        {
            return new Data.DateRange(refDate.AddMonths(-3).AddDays(1), refDate);
        }
    }
}
