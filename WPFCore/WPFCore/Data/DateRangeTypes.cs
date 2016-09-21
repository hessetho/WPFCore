using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WPFCore.Data
{
    /// <summary>
    /// Represents a list of <see cref="DateRangeType"/>s
    /// </summary>
    public class DateRangeTypes : KeyedCollection<string, DateRangeType>
    {
        /// <summary>
        /// Returns the key for the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        protected override string GetKeyForItem(DateRangeType item)
        {
            return item.RangeType;
        }

        /// <summary>
        /// Finds the specified range type.
        /// </summary>
        /// <param name="rangeType">Type of the range.</param>
        /// <returns></returns>
        public DateRangeType Find(string rangeType)
        {
            return this.FirstOrDefault(rt => rt.RangeType == rangeType);
        }

        /// <summary>
        /// Finds a type matching the <paramref name="startDate"/> and <paramref name="endDate"/>, returns <c>null</c> if no match found.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public DateRangeType FindMatchingType(DateTime startDate, DateTime endDate)
        {
            foreach(var rt in this)
                if(rt.Matches(startDate, endDate))
                    return rt;

            return null;
        }

        /// <summary>
        /// Finds a type matching the <paramref name="range"/>, returns <c>null</c> if no match found.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public DateRangeType FindMatchingType(DateRange range)
        {
            return this.FindMatchingType(range.StartDate, range.EndDate);
        }

        /// <summary>
        /// Adds the specified range type.
        /// </summary>
        /// <param name="rangeType">Type of the range.</param>
        /// <param name="description">The description.</param>
        /// <param name="getDateRangeFunction">The get date range function.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Add(string rangeType, string description, Func<DateRange> getDateRangeFunction)
        {
            if (this.Find(rangeType) != null)
                throw new InvalidOperationException(string.Format("The DateRangeType is already contained in this list.", rangeType));

            base.Add(new DateRangeType(rangeType, description, getDateRangeFunction));
        }

        /// <summary>
        /// Adds the specified range type.
        /// </summary>
        /// <param name="rangeType">Type of the range.</param>
        /// <param name="description">The description.</param>
        /// <param name="getDateRangeFunction">The get date range function.</param>
        /// <param name="matchFunction">The match function.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Add(string rangeType, string description, Func<DateRange> getDateRangeFunction, Func<DateTime, DateTime, bool> matchFunction)
        {
            if (this.Find(rangeType) != null)
                throw new InvalidOperationException(string.Format("The DateRangeType is already contained in this list.", rangeType));

            base.Add(new DateRangeType(rangeType, description, getDateRangeFunction, matchFunction));
        }
    }
}
