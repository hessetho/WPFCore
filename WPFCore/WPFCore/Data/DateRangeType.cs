using System;
using System.Windows.Navigation;

namespace WPFCore.Data
{
    /// <summary>
    /// Represents a date range type (such as YearToDay, CurrentMonth etc.)
    /// </summary>
    public class DateRangeType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateRangeType"/> class.
        /// </summary>
        /// <param name="rangeType">Type of the range.</param>
        /// <param name="description">The description.</param>
        /// <param name="getDateRangeFunction">The get date range function.</param>
        public DateRangeType(string rangeType, string description, Func<DateRange> getDateRangeFunction) : this(rangeType, description, getDateRangeFunction, null)
        {
            this.Matches = this.DefaultMatchFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRangeType"/> class.
        /// </summary>
        /// <param name="rangeType">Type of the date range.</param>
        /// <param name="description">The description.</param>
        /// <param name="getDateRangeFunction">The get date range function.</param>
        /// <param name="matchFunction">The match function.</param>
        public DateRangeType(string rangeType, string description, Func<DateRange> getDateRangeFunction, Func<DateTime, DateTime, bool> matchFunction)
        {
            this.RangeType = rangeType;
            this.Description = description;
            this.GetDateRange = getDateRangeFunction;
            this.Matches = matchFunction;
            this.IsUserDefined = false;
        }

        /// <summary>
        /// Creates a user definable date range type.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public static DateRangeType CreateUserDefinedType(DateTime startDate, DateTime endDate)
        {
            return CreateUserDefinedType("User defined", startDate, endDate);
        }

        /// <summary>
        /// Creates a user definable date range type.
        /// </summary>
        /// <param name="description">Descriptive text for this type.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public static DateRangeType CreateUserDefinedType(string description, DateTime startDate, DateTime endDate)
        {
            return new DateRangeType("UserDefined", description, () => new DateRange(startDate, endDate)) { IsUserDefined = true };
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance is user defined.
        /// </summary>
        public bool IsUserDefined
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the get date range.
        /// </summary>
        public Func<DateRange> GetDateRange { get; private set; }

        /// <summary>
        /// Gets the matches.
        /// </summary>
        public Func<DateTime, DateTime, bool> Matches { get; private set; }

        /// <summary>
        /// Gets the type of the date range.
        /// </summary>
        public string RangeType { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        private bool DefaultMatchFunction(DateTime startDate, DateTime endDate)
        {
            var dr = new DateRange { StartDate = startDate, EndDate = endDate };
            return this.GetDateRange().IsEqual(dr);
        }
    }
}
