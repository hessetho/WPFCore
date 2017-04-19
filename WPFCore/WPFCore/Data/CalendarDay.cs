using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCore.ViewModelSupport;
using WPFCore.XAML.Controls;

namespace WPFCore.Data
{
    public class CalendarDay : ViewModelCore
    {
        private DateTime date;
        private bool belongsToCurrentMonth;
        private readonly MonthView owningMonthView;
        private readonly ObservableCollection<CalendarAppointmentWrapper> localAppointments = new ObservableCollection<CalendarAppointmentWrapper>();
        private bool isSelected;

        internal CalendarDay(MonthView owningMonthView, DateTime date, bool belongsToCurrentMonth)
        {
            this.date = date;
            this.belongsToCurrentMonth = belongsToCurrentMonth;
            this.owningMonthView = owningMonthView;
            
            this.Appointments = new ReadOnlyObservableCollection<CalendarAppointmentWrapper>(this.localAppointments);
        }

        public DateTime Date
        {
            get { return this.date; }
            private set
            {
                this.date = value; 
                OnPropertyChanged("Date");
                OnPropertyChanged("DayOfMonth");
            }
        }

        public int DayOfMonth
        {
            get { return this.date.Day; }
        }

        public bool IsWeekend
        {
            get { return this.date.DayOfWeek == DayOfWeek.Sunday || this.date.DayOfWeek == DayOfWeek.Saturday; }
        }

        public bool IsToday
        {
            get { return this.date == DateTime.Today; }
        }

        public bool BelongsToCurrentMonth
        {
            get { return this.belongsToCurrentMonth; }
            private set
            {
                this.belongsToCurrentMonth = value;
                OnPropertyChanged("BelongsToCurrentMonth");
            }
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                this.isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public ReadOnlyObservableCollection<CalendarAppointmentWrapper> Appointments { get; private set; }

        internal void AddAppointment(CalendarAppointment appointment)
        {
            var wrapper = new CalendarAppointmentWrapper(this.owningMonthView, appointment);
            this.localAppointments.Add(wrapper);
        }

        internal void RemoveAppointment(CalendarAppointment appointment)
        {
            var wrapper = this.Appointments.FirstOrDefault(a => a.OriginalAppointment == appointment);
            this.localAppointments.Remove(wrapper);
        }

        public void ClearAppointments()
        {
            this.localAppointments.Clear();
        }
    }
}
