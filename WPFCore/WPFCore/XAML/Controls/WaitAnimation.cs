using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFCore.XAML.Controls
{
    /// <summary>
    /// </summary>
    [TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
    public class WaitAnimation : Control
    {
        private Canvas myCanvas;

        public enum WaitAnimationType
        {
            Smooth,
            SharpEdge
        }

        public static DependencyProperty NumberOfPiecesProperty =
           DependencyProperty.Register("NumberOfPieces", typeof(int), typeof(WaitAnimation),
           new PropertyMetadata(16, GeometryDataChanged),
           ValidateNumberOfPieces);

        public static DependencyProperty RotationDurationProperty =
           DependencyProperty.Register("RotationDuration", typeof(TimeSpan), typeof(WaitAnimation),
           new PropertyMetadata(TimeSpan.FromSeconds(2), GeometryDataChanged),
           ValidateRotationDuration);

        public static DependencyProperty StrokeThicknessProperty =
           DependencyProperty.Register("StrokeThickness", typeof(double), typeof(WaitAnimation),
           new PropertyMetadata(1.0, GeometryDataChanged),
           ValidateStrokeThickness);

        public static DependencyProperty StrokeProperty =
           DependencyProperty.Register("Stroke", typeof(Brush), typeof(WaitAnimation),
           new PropertyMetadata(Brushes.LightGray, GeometryDataChanged));

        public static DependencyProperty FillProperty =
           DependencyProperty.Register("Fill", typeof(Brush), typeof(WaitAnimation),
           new PropertyMetadata(Brushes.Gray, GeometryDataChanged));

        public static DependencyProperty InnerRadiusRatioProperty = 
            DependencyProperty.Register("InnerRadiusRatio", typeof(double), typeof(WaitAnimation),
            new PropertyMetadata(0.25, GeometryDataChanged), 
            ValidateInnerRadiusRatio);

        public static DependencyProperty AnimationTypeProperty = 
            DependencyProperty.Register("AnimationType", typeof(WaitAnimationType), typeof(WaitAnimation),
            new PropertyMetadata(WaitAnimationType.SharpEdge, GeometryDataChanged));

        static WaitAnimation()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitAnimation), new FrameworkPropertyMetadata(typeof(WaitAnimation)));
        }

        public int NumberOfPieces
        {
            get { return ((int)(GetValue(NumberOfPiecesProperty))); }
            set { SetValue(NumberOfPiecesProperty, value); }
        }

        public TimeSpan RotationDuration
        {
            get { return ((TimeSpan)(GetValue(RotationDurationProperty))); }
            set { SetValue(RotationDurationProperty, value); }
        }

        public WaitAnimationType AnimationType
        {
            get { return ((WaitAnimationType)(GetValue(AnimationTypeProperty))); }
            set { SetValue(AnimationTypeProperty, value); }
        }

        public double InnerRadiusRatio
        {
            get { return ((double)(GetValue(WaitAnimation.InnerRadiusRatioProperty))); }
            set { SetValue(WaitAnimation.InnerRadiusRatioProperty, value); }
        }

        public Brush Fill
        {
            get { return ((Brush)(GetValue(FillProperty))); }
            set { SetValue(FillProperty, value); }
        }

        public Brush Stroke
        {
            get { return ((Brush)(GetValue(StrokeProperty))); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return ((double)(GetValue(StrokeThicknessProperty))); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            myCanvas = GetTemplateChild("PART_Canvas") as Canvas;

            if (this.myCanvas != null)
            {
                this.myCanvas.SizeChanged += this.MyCanvasSizeChanged;
            }
        }

        void MyCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SetLayout();
        }

        private static void GeometryDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as WaitAnimation;

            if (ctrl != null)
            {
                ctrl.SetLayout();
            }
        }

        private static bool ValidateNumberOfPieces(object value)
        {
            return (int) value >= 0 && (int) value <= 1000;
        }

        private static bool ValidateRotationDuration(object value)
        {
            return (TimeSpan) value > TimeSpan.FromTicks(0);
        }

        private static bool ValidateStrokeThickness(object value)
        {
            return (double) value >= 0;
        }

        private static bool ValidateInnerRadiusRatio(object value)
        {
            return (double) value >= 0 && (double) value <= 1;
        }

        private void SetLayout()
        {
            if (myCanvas == null) return;
            // Geometrie-Daten einer Kuchenstücks berechnen
            double angle = 360.0 / this.NumberOfPieces;
            double centerX = myCanvas.ActualWidth / 2.0;
            double centerY = myCanvas.ActualHeight / 2.0;
            double radius = Math.Min(myCanvas.ActualWidth, myCanvas.ActualHeight) / 2.0;
            double innerRadius = radius * this.InnerRadiusRatio;

            // Intervall für den Animationsstart zweier aufeinander folgender Kuchenstücke
            var startIntervall = TimeSpan.FromTicks(this.RotationDuration.Ticks / this.NumberOfPieces);

            double currentRotation = 0;
            var currentBeginTime = TimeSpan.FromTicks(1);

            myCanvas.Children.Clear();
            for (int i = 0; i < this.NumberOfPieces; i++)
            {
                var piece = new PiePiece
                                {
                                    CentreX = centerX,
                                    CentreY = centerY,
                                    WedgeAngle = angle,
                                    Radius = radius,
                                    InnerRadius = innerRadius,
                                    RotationAngle = currentRotation,
                                    Fill = this.Fill,
                                    Stroke = this.Stroke,
                                    StrokeThickness = this.StrokeThickness,
                                    BeginTime = currentBeginTime,
                                    Duration = this.RotationDuration,
                                    AnimationType = this.AnimationType,
                                    Opacity = 0
                                };
                myCanvas.Children.Add(piece);
                currentRotation += angle;
                currentBeginTime += startIntervall;
            }
        }
    }
}
