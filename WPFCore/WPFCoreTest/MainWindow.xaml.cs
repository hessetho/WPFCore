using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFCore.Data;
using WPFCore.XAML.Controls;

namespace WPFCoreTest
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static DependencyProperty AppointmentsProperty =
                        DependencyProperty.Register("Appointments", typeof(ObservableCollection<CalendarAppointment>), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();

            this.Appointments = new ObservableCollection<CalendarAppointment>
            {
                new CalendarAppointment(new DateTime(2017, 01, 6), "Hl. Drei Könige"),
                new CalendarAppointment(new DateTime(2017, 01, 6), "Geb. M. Blicker")
            };
        }

        public ObservableCollection<CalendarAppointment> Appointments
        {
            get { return ((ObservableCollection<CalendarAppointment>)(GetValue(MainWindow.AppointmentsProperty))); }
            set { SetValue(MainWindow.AppointmentsProperty, value); }
        }

        private void MonthView_OnDayDoubleClicked(object sender, CalendarDay e)
        {
            var calendar = (MonthView) sender;

            Debug.WriteLine("double clicked: {0:d}", (object)e.Date);
            var eventText = InputTextBox.Show("Bezeichnung:");
            if (!string.IsNullOrEmpty(eventText))
            {
                var appointment = new CalendarAppointment(e.Date, eventText);
                this.Appointments.Add(appointment);
            }
        }

        private void MonthView_OnAppointmentDoubleClicked(object sender, CalendarAppointment e)
        {
            Debug.WriteLine("double clicked: {0:d} - {1}", (object)e.AppointmentDate, e.AppointmentText);
        }
    }
}
