using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using WPFCore.ViewModelSupport;

namespace WPFCore.Data
{
    public class CalendarAppointment
    {
        private readonly DateTime appointmentDate;
        private readonly string appointmentText;

        public CalendarAppointment(DateTime appointmentDate, string appointmentText)
        {
            this.appointmentDate = appointmentDate;
            this.appointmentText = appointmentText;

            this.Background = Brushes.LightBlue;
            this.BorderBrush = Brushes.LightSkyBlue;
        }

        public object Tag { get; set; }

        public DateTime AppointmentDate
        {
            get { return this.appointmentDate; }
        }

        public string AppointmentText
        {
            get { return this.appointmentText; }
        }

        public Brush BorderBrush { get; set; }

        public Brush Background { get; set; }
    }
}