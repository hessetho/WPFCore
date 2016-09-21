// ==========================================================================
// <summary>
// WpfHelper, Sammlung von Hilfsfunktionen und Klassen zur Verwendung mit WPF
// </summary>
//
// $Rev, 94 $
// $Id, DateRangeSelector.cs 94 2010-12-13 14,19,17Z  $
// 
// <copyright file="DateRangeSelector.cs" company="ICEP GmbH">
//      2009-2012 ICEP GmbH, T. Hesse
// </copyright>
// ==========================================================================

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WPFCore.Data;

namespace WPFCore.XAML.Controls
{
    /// <summary>
    ///     Stellt ein Steuerelement dar, welches dem Anwender die Auswahl eines Zeitraumes ermöglicht
    /// </summary>
    [TemplatePart(Name = "PART_Selector", Type = typeof (Selector))]
    public class DateRangeSelector : Control
    {
        /// <summary>
        ///     Das Control vom Typ <see cref="Selector" />, welches die verfügbaren Zeitraumtypen darstellt.
        /// </summary>
        private Selector selectorControl;

        /// <summary>
        ///     Initialisiert die <see cref="DateRangeSelector" />-Klasse.
        /// </summary>
        static DateRangeSelector()
        {
            // Dem System mitteilen, dass wir einen eigenen Default-Style liefern
            // Dazu werden die Metadaten für das DependencyProperty DefaultStyleKey auf diese Klasse "verbogen"
            DefaultStyleKeyProperty.OverrideMetadata(typeof (DateRangeSelector),
                new FrameworkPropertyMetadata(typeof (DateRangeSelector)));
        }

        /// <summary>
        ///     Setzt/Liefert den ausgewählten Zeitraum.
        /// </summary>
        public DateRange DateRange
        {
            get { return (DateRange) this.GetValue(DateRangeProperty); }
            set { this.SetValue(DateRangeProperty, value); }
        }

        /// <summary>
        ///     Setzt/Liefert die Liste der auswählbaren Zeiträume
        /// </summary>
        public DateRangeTypes DateRangeList
        {
            get { return ((DateRangeTypes) (this.GetValue(DateRangeListProperty))); }
            set { this.SetValue(DateRangeListProperty, value); }
        }

        /// <summary>
        ///     Wird per ApplyTemplate ein neues Template zugewiesen, so stellen wir hier
        ///     sicher, dass unser Button_Click noch korrekt funktioniert.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Wichtig: PART_Selector MUSS zwingend als TemplatePartAttribute der Klasse
            //          deklariert worden sein!
            this.selectorControl = this.GetTemplateChild("PART_Selector") as Selector;
            if (this.selectorControl != null)
            {
                if (this.DateRangeList == null)
                    this.DateRangeList = new DateRangeTypes
                    {
                        new DateRangeType("MonthToDay", "Laufender Monat", DateRanges.MonthToDay),
                        new DateRangeType("CurrentMonth", "Aktueller Monat", DateRanges.CurrentMonth),
                        new DateRangeType("PreviousMonth", "Vorheriger Monat", DateRanges.PreviousMonth),
                        new DateRangeType("YearToDay", "Laufendes Jahr", DateRanges.YearToDay),
                        new DateRangeType("CurrentYear", "Aktuelles Jahr", DateRanges.CurrentYear),
                        new DateRangeType("PreviousYear", "Vorheriges Jahr", DateRanges.PreviousYear),
                        new DateRangeType("Last12Months", "Letzte 12 Monate", DateRanges.Last12Months),
                        DateRangeType.CreateUserDefinedType("Benutzerdefiniert", DateTime.Today, DateTime.Today)
                    };

                // und dem Selector-Control als Datenquelle anhängen
                this.selectorControl.ItemsSource = this.DateRangeList;
                this.selectorControl.DisplayMemberPath = "Description";
                this.selectorControl.SelectedValuePath = "RangeType";

                this.selectorControl.SelectionChanged += this.SelectionChanged;

                if (this.DateRange != null)
                {
                    var rangeType =this.DateRangeList.FindMatchingType(this.DateRange);
                    if (rangeType != null)
                        this.selectorControl.SelectedValue = rangeType.RangeType;
                    else this.selectorControl.SelectedValue = "UserDefined";
                }
            }
        }

        /// <summary>
        ///     Tritt auf, wenn <see cref="DateRange" /> geändert worden ist.
        /// </summary>
        /// <param name="d">Eine <see cref="DateRangeSelector" />-Instanz.</param>
        /// <param name="e">Ereignisdaten mit dem neuen Wert.</param>
        private static void OnDateRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = d as DateRangeSelector;
            var range = e.NewValue as DateRange;
            Debug.Assert(selector != null && range != null, string.Format("Ooops! {0} / {1}", selector, e.NewValue));

            // Den DateRangeSelector einrichten.
            selector.SetDateRange(range);
        }

        /// <summary>
        ///     Richtet den <c>DateRangeSelector</c> für einen neuen Zeitraum ein.
        /// </summary>
        /// <param name="newRange">Der neue Zeitraum</param>
        private void SetDateRange(DateRange newRange)
        {
            // Ereignis-Handler einrichten, um Änderungen am Zeitraumstyp zu erkennen.
            //DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(DateRange.RangeTypeProperty,
            //    typeof (DateRange));
            //dpd.AddValueChanged(newRange, this.OnRangeTypeChanged);

            // Auswahlliste auf das ausgewählte Element setzen
            if (this.selectorControl != null)
                this.selectorControl.SelectedValue = this.DateRangeList.FindMatchingType(this.DateRange);

            //InvokeDateRangeChanged(new DateRangeChangedEventArgs(newRange));
        }

        /// <summary>
        ///     Tritt ein, wenn in der Auswahlliste ein anderes Element ausgewählt worden ist.
        /// </summary>
        /// <param name="sender">Die Auswahlliste</param>
        /// <param name="e">Ereignisdaten mit dem neuen ausgewählten Element</param>
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var item = (DateRangeType) e.AddedItems[0];

            if (this.DateRange != null)
            {
                if (!item.IsUserDefined)
                    this.DateRange.SetRange(item.GetDateRange());
            }
            else
            // Typischerweise tritt dieser Fall auf, wenn DateRange nicht gebunden ist
                this.DateRange = item.GetDateRange();
        }

        /// <summary>
        ///     Tritt ein, wenn sich der ausgewählte Zeitraum verändert hat.
        /// </summary>
        /// <summary>
        ///     Property, welches die Eigenschaft DateRange repräsentiert.
        /// </summary>
        public static readonly DependencyProperty DateRangeProperty =
            DependencyProperty.Register("DateRange", typeof (DateRange), typeof (DateRangeSelector),
                new PropertyMetadata(OnDateRangeChanged));

        /// <summary>
        ///     Liste der auswählbaren Zeiträume
        /// </summary>
        public static readonly DependencyProperty DateRangeListProperty =
            DependencyProperty.Register("DateRangeList", typeof (DateRangeTypes), typeof (DateRangeSelector));
    }
}