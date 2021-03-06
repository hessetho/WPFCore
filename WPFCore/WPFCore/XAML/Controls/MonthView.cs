﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFCore.Data;
using WPFCore.Helper;
using WPFCore.ViewModelSupport;

namespace WPFCore.XAML.Controls
{
    public class MonthChangedEventArgs
    {
        public MonthChangedEventArgs(int month, int year)
        {
            this.Month = month;
            this.Year = year;
        }

        public int Month { get; private set; }
        public int Year { get; private set; }
    }

    [TemplatePart(Name = "PART_DayNames", Type = typeof (Grid))]
    [TemplatePart(Name = "PART_DaysGrid", Type = typeof (Grid))]
    [TemplatePart(Name = "PART_PrevMonthButton", Type = typeof (Button))]
    [TemplatePart(Name = "PART_NextMonthButton", Type = typeof (Button))]
    [TemplatePart(Name = "PART_AppointmentList", Type = typeof (ItemsControl))]
    public class MonthView : Control
    {
        public delegate void MonthChangedEventHandler(object sender, MonthChangedEventArgs e);

        private readonly Dictionary<DateTime, CalendarDay> displayedDays = new Dictionary<DateTime, CalendarDay>();
        private ICommand commandClickedADay;
        private ICommand commandSelectedADay;
        private Grid dayNamesGrid;
        private Grid monthViewGrid;
        private Button nextMonthButton;
        private Button prevMonthButton;

        static MonthView()
        {
            // Dem System mitteilen, dass wir einen eigenen Default-Style liefern
            // Dazu werden die Metadaten für das DependencyProperty DefaultStyleKey auf diese Klasse "verbogen"
            DefaultStyleKeyProperty.OverrideMetadata(typeof (MonthView),
                new FrameworkPropertyMetadata(typeof (MonthView)));
        }

        /// <summary>
        ///     Das angezeigte Jahr
        /// </summary>
        public int CurrentYear
        {
            get { return ((int) (GetValue(CurrentYearProperty))); }
            set { SetValue(CurrentYearProperty, value); }
        }

        /// <summary>
        ///     Der angezeigte Monat
        /// </summary>
        public int CurrentMonth
        {
            get { return ((int) (GetValue(CurrentMonthProperty))); }
            set { SetValue(CurrentMonthProperty, value); }
        }

        /// <summary>
        /// Liste der Jahre
        /// </summary>
        public ObservableCollection<int> YearList
        {
            get { return ((ObservableCollection<int>)(GetValue(MonthView.YearListProperty))); }
            set { SetValue(MonthView.YearListProperty, value); }
        }

        /// <summary>
        /// Monatsliste
        /// </summary>
        public ObservableCollection<KeyValuePair<int, string>> MonthList
        {
            get { return ((ObservableCollection<KeyValuePair<int, string>>)(GetValue(MonthView.MonthListProperty))); }
            set { SetValue(MonthView.MonthListProperty, value); }
        }

        /// <summary>
        /// Erstes Jahr in der Jahresliste (Standard: aktuelles Jahr - 5)
        /// </summary>
        public int FirstYear
        {
            get { return ((int)(GetValue(MonthView.FirstYearProperty))); }
            set { SetValue(MonthView.FirstYearProperty, value); }
        }

        /// <summary>
        /// Letztes Jahr in der Jahresliste (Standard: aktuelles Jahr + 1)
        /// </summary>
        public int LastYear
        {
            get { return ((int)(GetValue(MonthView.LastYearProperty))); }
            set { SetValue(MonthView.LastYearProperty, value); }
        }

        /// <summary>
        ///     DataTemplate für einen Tag
        /// </summary>
        public DataTemplate DayBoxTemplate
        {
            get { return ((DataTemplate) (GetValue(DayBoxTemplateProperty))); }
            set { SetValue(DayBoxTemplateProperty, value); }
        }

        public IEnumerable<CalendarAppointment> Appointments
        {
            get { return ((IEnumerable<CalendarAppointment>) (GetValue(AppointmentsProperty))); }
            set { SetValue(AppointmentsProperty, value); }
        }

        /// <summary>
        ///     Bezeichnung des aktuellen Monats
        /// </summary>
        public string MonthName
        {
            get { return ((string) (GetValue(MonthNameProperty))); }
            private set { SetValue(MonthNameProperty, value); }
        }

        public CalendarDay SelectedDay
        {
            get { return ((CalendarDay) (GetValue(SelectedDayProperty))); }
            set { SetValue(SelectedDayProperty, value); }
        }

        public DataTemplate AppointmentBoxTemplate
        {
            get { return ((DataTemplate) (GetValue(AppointmentBoxTemplateProperty))); }
            set { SetValue(AppointmentBoxTemplateProperty, value); }
        }

        public event MonthChangedEventHandler MonthChanged;
        public event EventHandler<CalendarDay> DayDoubleClicked;
        public event EventHandler<CalendarAppointment> AppointmentDoubleClicked;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.DeferUpdates();

            // Monatsliste einrichten
            this.MonthList = new ObservableCollection<KeyValuePair<int, string>>();
            for (int m = 1; m <= 12; m++)
                this.MonthList.Add( new KeyValuePair<int, string>(m, new DateTime(1900, m, 1).ToString("MMMM")));
            // Liste der Jahre einrichten
            this.YearList = new ObservableCollection<int>();
            this.BuildYearList();

            this.CurrentYear = DateTime.Today.Year;
            this.CurrentMonth = DateTime.Today.Month;

            this.monthViewGrid = GetTemplateChild("PART_DaysGrid") as Grid;
            this.dayNamesGrid = GetTemplateChild("PART_DayNames") as Grid;
            this.prevMonthButton = GetTemplateChild("PART_PrevMonthButton") as Button;
            this.nextMonthButton = GetTemplateChild("PART_NextMonthButton") as Button;

            this.DayBoxTemplate = FindResource("DayDefaultTemplate") as DataTemplate;
            this.AppointmentBoxTemplate = FindResource("AppointmentDefaultTemplate") as DataTemplate;

            if (this.prevMonthButton != null)
                this.prevMonthButton.Click += this.PreviousMonthClicked;

            if (this.nextMonthButton != null)
                this.nextMonthButton.Click += this.NextMonthClicked;

            // Wochentags-Leiste einrichten
            var dayNameTemplate = FindResource("DayNameDefaultTemplate") as DataTemplate;
            if (dayNameTemplate != null && this.dayNamesGrid != null)
            {
                for (var dow = 0; dow < 7; dow++)
                {
                    var dayNameBox = (FrameworkElement) dayNameTemplate.LoadContent();
                    dayNameBox.DataContext = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[dow];
                    dayNameBox.SetValue(Grid.ColumnProperty, dow + 1);
                    dayNameBox.SetValue(Grid.RowProperty, 0);

                    // und zuweisen
                    this.dayNamesGrid.Children.Add(dayNameBox);
                }
            }

            //this.appoinmentItemsControl.Loaded += (ds, de) =>
            //{
            //    this.appoinmentItemsControl.ItemContainerGenerator.StatusChanged += (sender, args) =>
            //    {
            //        var ctrl = sender;
            //    };
            //};
            this.AllowUpdates();

            // Initiale Monatsansicht erzeugen
            this.BuildMonthView();
        }

        private static void OnSelectedDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (MonthView) d;

            if (e.OldValue != null)
            {
                var prevDay = (CalendarDay) e.OldValue;
                prevDay.IsSelected = false;
            }

            if (e.NewValue != null)
            {
                var currDay = (CalendarDay) e.NewValue;
                currDay.IsSelected = true;
            }
        }

        private static void OnAppointmentBoxTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var monthView = (MonthView) d;
            monthView.BuildMonthView();
        }

        private static void OnCurrentYearMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var monthView = (MonthView)d;
            monthView.BuildMonthView();
        }

        private static void OnRebuildYearList(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var monthView = (MonthView)d;
            monthView.BuildYearList();
        }

        public static DependencyProperty AppointmentBoxTemplateProperty =
            DependencyProperty.Register("AppointmentBoxTemplate", typeof (DataTemplate), typeof (MonthView),
                new PropertyMetadata(OnAppointmentBoxTemplateChanged));

        public static DependencyProperty CurrentMonthProperty =
            DependencyProperty.Register("CurrentMonth", typeof (int), typeof (MonthView), new PropertyMetadata(OnCurrentYearMonthChanged));

        public static DependencyProperty CurrentYearProperty =
            DependencyProperty.Register("CurrentYear", typeof (int), typeof (MonthView), new PropertyMetadata(OnCurrentYearMonthChanged));

        public static DependencyProperty DayBoxTemplateProperty =
            DependencyProperty.Register("DayBoxTemplate", typeof (DataTemplate), typeof (MonthView));

        public static DependencyProperty MonthNameProperty =
            DependencyProperty.Register("MonthName", typeof (string), typeof (MonthView));

        public static DependencyProperty AppointmentsProperty =
            DependencyProperty.Register("Appointments", typeof (IEnumerable<CalendarAppointment>), typeof (MonthView),
                new PropertyMetadata(OnAppointmentsChanged));

        public static DependencyProperty SelectedDayProperty =
            DependencyProperty.Register("SelectedDay", typeof (CalendarDay), typeof (MonthView),
                new PropertyMetadata(OnSelectedDayChanged));

        /// <summary>
        /// Eigenschaft: Liste der Monates
        /// </summary>
        public static DependencyProperty MonthListProperty =
                    DependencyProperty.Register("MonthList", typeof(ObservableCollection<KeyValuePair<int, string>>), typeof(MonthView));

        /// <summary>
        /// Eigenschaft: Liste der Jahre
        /// </summary>
        public static DependencyProperty YearListProperty =
                    DependencyProperty.Register("YearList", typeof(ObservableCollection<int>), typeof(MonthView));

        /// <summary>
        /// Eigenschaft: Erstes Jahr in der Jahresliste
        /// </summary>
        public static DependencyProperty FirstYearProperty =
                    DependencyProperty.Register("FirstYear", typeof(int), typeof(MonthView), new PropertyMetadata(DateTime.Today.Year - 5, OnRebuildYearList));

        /// <summary>
        /// Eigenschaft: Letztes Jahr in der Jahresliste
        /// </summary>
        public static DependencyProperty LastYearProperty =
                    DependencyProperty.Register("LastYear", typeof(int), typeof(MonthView), new PropertyMetadata(DateTime.Today.Year + 1, OnRebuildYearList));

        #region Navigation

        private void PreviousMonthClicked(object sender, RoutedEventArgs e)
        {
            this.DeferUpdates();
            if (this.CurrentMonth == 1)
            {
                this.CurrentMonth = 12;
                this.CurrentYear--;
            }
            else
                this.CurrentMonth--;
            this.AllowUpdates();

            this.BuildMonthView();
        }

        private bool updatingDeferred = false;
        private void DeferUpdates() { this.updatingDeferred = true; }
        private void AllowUpdates() { this.updatingDeferred = false; }


        private void NextMonthClicked(object sender, RoutedEventArgs e)
        {
            this.DeferUpdates();
            if (this.CurrentMonth == 12)
            {
                this.CurrentMonth = 1;
                this.CurrentYear++;
            }
            else
                this.CurrentMonth++;
            this.AllowUpdates();

            this.BuildMonthView();
        }

        #endregion Navigation

        #region Ansicht aufbauen

        private void BuildMonthView()
        {
            if (this.monthViewGrid == null || updatingDeferred) return;
            this.BuildGrid();

            var refDate = new DateTime(this.CurrentYear, this.CurrentMonth, 1);
            var lastDay = refDate.LastOfMonth();
            this.MonthName = refDate.ToString("MMMM yyyy");

            var offset = (int) refDate.DayOfWeek;
            var row = -1;
            var col = -1;

            // Monatstage erzeugen
            for (var idx = 0; idx < lastDay.Day; idx++)
            {
                // Position ermitteln
                row = (idx + offset)/7;
                col = (idx + offset)%7;

                this.AddDayToGrid(new DateTime(this.CurrentYear, this.CurrentMonth, idx + 1), row, col, true);
            }

            // Die "Randtage" ergänzen
            var prevDate = refDate.AddMonths(-1).LastOfMonth();
            for (var idx = 0; idx < offset; idx++)
                this.AddDayToGrid(prevDate.AddDays(-idx), 0, offset - idx - 1, false);

            var nextDate = refDate.AddMonths(1);
            for (var idx = 0; idx < 6 - col; idx++)
                this.AddDayToGrid(nextDate.AddDays(idx), row, col + idx + 1, false);

            // Die Termine ergänzen, diese dazu ggf. erst befüllen lassen
            this.MonthChanged?.Invoke(this, new MonthChangedEventArgs(this.CurrentMonth, this.CurrentYear));

            this.ApplyAppointments();
        }

        private void BuildYearList()
        {
            this.YearList.Clear();
            for (int y = this.FirstYear; y <= this.LastYear; y++)
                this.YearList.Add(y);
        }

        private void AddDayToGrid(DateTime date, int row, int col, bool isCurrentMonth)
        {
            // Das Element für den Tag ermitteln
            var dayBox = (FrameworkElement) this.DayBoxTemplate.LoadContent();
            dayBox.ApplyTemplate();

            var appointmentItemsControl =
                LogicalTreeHelper.FindLogicalNode(dayBox, "PART_AppointmentList") as ItemsControl;
            if (appointmentItemsControl != null)
                appointmentItemsControl.ItemTemplate = this.AppointmentBoxTemplate;


            if(this.monthViewGrid.RowDefinitions.Count < row + 1)
                this.monthViewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Einrichten
            dayBox.DataContext = this.CreateCalendarDay(date, isCurrentMonth);
            dayBox.SetValue(Grid.ColumnProperty, col + 1);
            dayBox.SetValue(Grid.RowProperty, row);

            dayBox.InputBindings.Add(new MouseBinding
            {
                Command = this.CommandClickedADay,
                MouseAction = MouseAction.LeftDoubleClick,
                CommandParameter = dayBox.DataContext
            });

            dayBox.InputBindings.Add(new MouseBinding
            {
                Command = this.CommandSelectedADay,
                MouseAction = MouseAction.LeftClick,
                CommandParameter = dayBox.DataContext
            });

            // und zuweisen
            this.monthViewGrid.Children.Add(dayBox);
        }

        private CalendarDay CreateCalendarDay(DateTime date, bool isCurrentMonth)
        {
            var day = new CalendarDay(this, date, isCurrentMonth);
            this.displayedDays.Add(date, day);

            return day;
        }

        private void BuildGrid()
        {
            if (this.monthViewGrid == null) return;

            this.monthViewGrid.RowDefinitions.Clear();
            // Zeilen der Wochen
            //while (this.monthViewGrid.RowDefinitions.Count < 5)
            //    this.monthViewGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});

            // Spalten der Tage
            this.monthViewGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Auto)});
            while (this.monthViewGrid.ColumnDefinitions.Count < 8)
                this.monthViewGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

            this.monthViewGrid.Children.Clear();
            this.displayedDays.Clear();
        }

        #endregion Ansicht aufbauen

        #region Commands

        #region CommandClickedADay

        public ICommand CommandClickedADay
        {
            get
            {
                return this.commandClickedADay ??
                       (this.commandClickedADay = new RelayCommand<CalendarDay>(this.ClickedADay, this.CanClickedADay));
            }
        }

        private void ClickedADay(CalendarDay day)
        {
            if (this.DayDoubleClicked != null)
                this.DayDoubleClicked(this, day);
        }

        private bool CanClickedADay(CalendarDay day)
        {
            return true;
        }

        #endregion CommandClickedADay

        #region CommandSelectedADay

        public ICommand CommandSelectedADay
        {
            get
            {
                return this.commandSelectedADay ??
                       (this.commandSelectedADay =
                           new RelayCommand<CalendarDay>(this.SelectedADay, this.CanSelectedADay));
            }
        }

        private void SelectedADay(CalendarDay day)
        {
            this.SelectedDay = day;
        }

        private bool CanSelectedADay(CalendarDay day)
        {
            return true;
        }

        #endregion CommandSelectedADay

        #endregion Commands

        #region Verwaltung der Termine

        /// <summary>
        /// Ereignisbehandlung, tritt ein, wenn eine andere Terminliste angehängt wird
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnAppointmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var monthView = (MonthView) d;

            if (e.OldValue is INotifyCollectionChanged)
            {
                var collection = (INotifyCollectionChanged) e.OldValue;
                collection.CollectionChanged -= monthView.OnAppointmentCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged)
            {
                var collection = (INotifyCollectionChanged) e.NewValue;
                collection.CollectionChanged += monthView.OnAppointmentCollectionChanged;
            }

            monthView.ApplyAppointments();
        }

        /// <summary>
        /// Ereignisbehandlung, tritt ein, wenn die Terminliste verändert wurde
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppointmentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var appointment in e.NewItems.Cast<CalendarAppointment>())
                    {
                        CalendarDay day = null;
                        if (this.displayedDays.TryGetValue(appointment.AppointmentDate, out day))
                            day.AddAppointment(appointment);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var appointment in e.OldItems.Cast<CalendarAppointment>())
                    {
                        CalendarDay day = null;
                        if (this.displayedDays.TryGetValue(appointment.AppointmentDate, out day))
                            day.RemoveAppointment(appointment);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var day in this.displayedDays.Values)
                        day.ClearAppointments();
                    break;
            }
        }

        private void ApplyAppointments()
        {
            foreach (var day in this.displayedDays.Values)
                day.ClearAppointments();

            if (this.Appointments != null)
            {
                foreach (var appointment in this.Appointments)
                {
                    CalendarDay day = null;

                    if (this.displayedDays.TryGetValue(appointment.AppointmentDate, out day))
                        day.AddAppointment(appointment);
                }
            }
        }

        internal void FireAppointmentDoubleClicked(CalendarAppointment appointment)
        {
            if (this.AppointmentDoubleClicked != null)
                this.AppointmentDoubleClicked(this, appointment);
        }

        #endregion Verwaltung der Termine
    }
}