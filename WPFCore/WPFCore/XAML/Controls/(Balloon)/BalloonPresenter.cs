using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WPFCore.XAML.Controls
{

    public enum CallOutPlacement
    {
        TopLeft, TopRight,
        LeftTop, LeftBottom,
        RightTop, RightBottom,
        BottomLeft, BottomRight
    }

    /// <summary>
    /// Stellt ein <see cref="ContentControl"/> bereit, welches einen <see cref="Balloon"/>-Shape als Umrandung verwendet
    /// </summary>
    public class BalloonPresenter : ContentControl
    {
        static BalloonPresenter()
        {
            // Dem System mitteilen, dass wir einen eigenen Default-Style liefern
            // Dazu werden die Metadaten für das DependencyProperty DefaultStyleKey auf diese Klasse "verbogen"
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BalloonPresenter),
                                                     new FrameworkPropertyMetadata(typeof(BalloonPresenter)));
        }


        /// <summary>
        /// Wir per ApplyTemplate ein neues Template zugewiesen, so stellen wir hier
        /// sicher, dass unser Button_Click noch korrekt funktioniert.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //// Wichtig: PART_Browse MUSS zwingend als TemplatePartAttribute der Klasse
            ////          deklariert worden sein!
            //var browseButton = GetTemplateChild("PART_Browse") as Button;
            //if (browseButton != null)
            //{
            //    browseButton.Click += this.BrowseButtonClick;
            //}
        }


        public static DependencyProperty CallOutPlacementProperty =
                    DependencyProperty.Register("CallOutPlacement", typeof(CallOutPlacement), typeof(BalloonPresenter),
                        new FrameworkPropertyMetadata(CallOutPlacement.TopLeft, FrameworkPropertyMetadataOptions.Inherits));

        public CallOutPlacement CallOutPlacement
        {
            get { return ((CallOutPlacement)(GetValue(CallOutPlacementProperty))); }
            set { SetValue(CallOutPlacementProperty, value); }
        }

    }
}
