using System;
using System.Windows.Input;
using WPFCore.ViewModelSupport;
using WPFCore.XAML.Controls;

namespace WPFCore.Data
{
    public class CalendarAppointmentWrapper
    {
        private readonly CalendarAppointment originalAppointment;
        private readonly MonthView owningMonthView;
        private ICommand commandAppointmentDoubleClicked;

        public CalendarAppointmentWrapper(MonthView owningMonthView, CalendarAppointment originalAppointment)
        {
            this.originalAppointment = originalAppointment;
            this.owningMonthView = owningMonthView;
        }

        public DateTime AppointmentDate
        {
            get { return this.originalAppointment.AppointmentDate; }
        }

        public string AppointmentText
        {
            get { return this.originalAppointment.AppointmentText; }
        }

        public object Tag
        {
            get { return this.originalAppointment.Tag; }
        }

        #region Commands

        #region CommandAppointmentDoubleClicked

        public ICommand CommandAppointmentDoubleClicked
        {
            get
            {
                return this.commandAppointmentDoubleClicked ??
                       (this.commandAppointmentDoubleClicked =
                           new RelayCommand(this.AppointmentDoubleClicked, this.CanAppointmentDoubleClicked));
            }
        }

        public CalendarAppointment OriginalAppointment
        {
            get { return this.originalAppointment; }
        }

        private void AppointmentDoubleClicked()
        {
            this.owningMonthView.FireAppointmentDoubleClicked(this.originalAppointment);
        }

        private bool CanAppointmentDoubleClicked()
        {
            return true;
        }

        #endregion CommandAppointmentDoubleClicked

        #endregion Commands    
    }
}
