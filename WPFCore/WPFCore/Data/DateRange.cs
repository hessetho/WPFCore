using System;
using WPFCore.ViewModelSupport;

namespace WPFCore.Data
{
    public class DateRange : ValidationViewModelBase
    {
        public event EventHandler DateRangeChanged;

        private DateTime startDate;
        private DateTime endDate;

        public DateRange(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;

            base.Validate();
        }

        public DateRange() : this(DateTime.Today, DateTime.Today)
        {
        }

        /// <summary>
        /// Copy-Konstruktor.
        /// </summary>
        /// <param name="otherRange"></param>
        internal DateRange(DateRange otherRange) : this(otherRange.StartDate, otherRange.EndDate)
        {
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        [ValidateDateRange(ErrorMessage="The start date must be earlier or equal to the end date.")]
        public DateTime StartDate
        {
            get { return this.startDate; }
            set
            {
                this.startDate = value;
                this.OnPropertyChanged("StartDate");

                if (this.IsValidRange)
                    this.FireDateRangeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        [ValidateDateRange(ErrorMessage = "The end date must be later or equal to the start date.")]
        public DateTime EndDate
        {
            get { return this.endDate; }
            set
            {
                this.endDate = value;
                this.OnPropertyChanged("EndDate");

                if (this.IsValidRange)
                    this.FireDateRangeChanged();
            }
        }

        public void SetRange(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;

            this.OnPropertyChanged("EndDate");
            this.OnPropertyChanged("StartDate");

            if (this.IsValidRange)
                this.FireDateRangeChanged();
        }

        public void SetRange(DateRange dateRange)
        {
            this.SetRange(dateRange.StartDate, dateRange.endDate);
        }

        public bool IsValidRange
        {
            get { return this.StartDate <= this.EndDate; }
        }

        public bool IsEqual(DateRange other)
        {
            if (this.IsValidRange && other.IsValidRange)
                return (this.startDate == other.startDate) && (this.endDate == other.endDate);

            return false;
        }

        private void FireDateRangeChanged()
        {
            if (this.DateRangeChanged != null)
                this.DateRangeChanged(this, new EventArgs());
        }

        public override string ToString()
        {
            if (!this.IsValid)
                return "DateRange (invalid)";

            if (this.startDate == this.endDate)
                return string.Format("{0:d} ({0:ddd})", this.startDate);

            return string.Format("{0:d} - {1:d}", this.startDate, this.endDate);
            //return string.Format("DateRange ({0:d} - {1:d})", this.startDate, this.endDate);
        }
    }
}
