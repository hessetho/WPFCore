using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFCore.XAML.Controls
{
    /// <summary>
    /// A pie piece shape
    /// </summary>
    internal class PiePiece : Shape
    {
        #region dependency properties

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(PiePiece),
                                        new FrameworkPropertyMetadata(0.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender |
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadiusProperty", typeof(double), typeof(PiePiece),
                                        new FrameworkPropertyMetadata(0.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender |
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty WedgeAngleProperty =
            DependencyProperty.Register("WedgeAngle", typeof(double), typeof(PiePiece),
                                        new FrameworkPropertyMetadata(0.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender |
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(PiePiece),
                                        new FrameworkPropertyMetadata(0.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender |
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty CentreXProperty =
            DependencyProperty.Register("CentreX", typeof(double), typeof(PiePiece),
                                        new FrameworkPropertyMetadata(0.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender |
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty CentreYProperty =
            DependencyProperty.Register("CentreY", typeof(double), typeof(PiePiece),
                                        new FrameworkPropertyMetadata(0.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender |
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(PiePiece),
                                        new PropertyMetadata(OnBeginTimePropertyChanged));

        public static DependencyProperty DurationProperty =
           DependencyProperty.Register("Duration", typeof(Duration), typeof(PiePiece),
           new PropertyMetadata(new Duration(TimeSpan.FromSeconds(2.0)), DurationPropertyChanged));

        public static DependencyProperty AnimationTypeProperty =
            DependencyProperty.Register("AnimationType", typeof(WaitAnimation.WaitAnimationType), typeof(PiePiece),
            new PropertyMetadata(WaitAnimation.WaitAnimationType.SharpEdge, AnimationTypePropertyChanged));

        #endregion

        #region Properties
        /// <summary>
        /// The radius of this pie piece
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        /// <summary>
        /// The inner radius of this pie piece
        /// </summary>
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        /// <summary>
        /// The wedge angle of this pie piece in degrees
        /// </summary>
        public double WedgeAngle
        {
            get { return (double)GetValue(WedgeAngleProperty); }
            set { SetValue(WedgeAngleProperty, value); }
        }

        /// <summary>
        /// The rotation, in degrees, from the Y axis vector of this pie piece.
        /// </summary>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        /// <summary>
        /// The X coordinate of centre of the circle from which this pie piece is cut.
        /// </summary>
        public double CentreX
        {
            get { return (double)GetValue(CentreXProperty); }
            set { SetValue(CentreXProperty, value); }
        }

        /// <summary>
        /// The Y coordinate of centre of the circle from which this pie piece is cut.
        /// </summary>
        public double CentreY
        {
            get { return (double)GetValue(CentreYProperty); }
            set { SetValue(CentreYProperty, value); }
        }

        public TimeSpan BeginTime
        {
            get { return ((TimeSpan)(GetValue(BeginTimeProperty))); }
            set { SetValue(BeginTimeProperty, value); }
        }

        public Duration Duration
        {
            get { return ((Duration)(GetValue(DurationProperty))); }
            set { SetValue(DurationProperty, value); }
        }

        public WaitAnimation.WaitAnimationType AnimationType
        {
            get { return ((WaitAnimation.WaitAnimationType)(GetValue(AnimationTypeProperty))); }
            set { SetValue(AnimationTypeProperty, value); }
        }

        #endregion

        private DoubleAnimationUsingKeyFrames faderAnimation;
        //        private readonly DoubleAnimation faderAnimation;
        private bool animationStarted;

        public PiePiece()
        {
            //this.faderAnimation = new DoubleAnimation(0, 1.0, TimeSpan.FromSeconds(1))
            //                          {
            //                              AutoReverse = true,
            //                              RepeatBehavior = RepeatBehavior.Forever
            //                          };

            //this.faderAnimation = new DoubleAnimationUsingKeyFrames
            //{
            //    Duration = TimeSpan.FromSeconds(2),
            //    AutoReverse = false,
            //    RepeatBehavior = RepeatBehavior.Forever
            //};

            //// Variante A
            //faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.05, KeyTime.FromPercent(1)));
            //faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.1, KeyTime.FromPercent(0.35)));
            //faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.9, KeyTime.FromPercent(0.05)));
            //faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromPercent(0)));

            //// Variante B
            //faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.05, KeyTime.FromPercent(0)));
            //faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.1, KeyTime.FromPercent(0.65)));
            //faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.9, KeyTime.FromPercent(0.95)));
            //faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromPercent(1)));

            this.SetAnimationType(this.AnimationType);
            animationStarted = false;
        }

        private static void OnBeginTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var piePiece = d as PiePiece;
            if (piePiece != null)
                piePiece.StartAnimation((TimeSpan)e.NewValue);
        }

        private static void DurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var piePiece = d as PiePiece;
            if (piePiece != null)
                piePiece.SetDuration((Duration)e.NewValue);
        }

        private static void AnimationTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var piePiece = d as PiePiece;
            if (piePiece != null)
                piePiece.SetAnimationType((WaitAnimation.WaitAnimationType)e.NewValue);
        }

        private void StartAnimation(TimeSpan beginTime)
        {
            this.faderAnimation.BeginTime = beginTime;
            BeginAnimation(OpacityProperty, this.faderAnimation);
            animationStarted = true;
        }

        private void SetDuration(Duration newValue)
        {
            switch (this.AnimationType)
            {
                case WaitAnimation.WaitAnimationType.Smooth:
                    this.faderAnimation.Duration = TimeSpan.FromTicks(newValue.TimeSpan.Ticks / 2);
                    break;
                case WaitAnimation.WaitAnimationType.SharpEdge:
                    this.faderAnimation.Duration = TimeSpan.FromTicks(newValue.TimeSpan.Ticks);
                    break;
                default:
                    this.faderAnimation.Duration = TimeSpan.FromTicks(newValue.TimeSpan.Ticks / 2);
                    break;
            }

            if (animationStarted)
                BeginAnimation(OpacityProperty, this.faderAnimation);
        }

        private void SetAnimationType(WaitAnimation.WaitAnimationType animationType)
        {
            this.faderAnimation = new DoubleAnimationUsingKeyFrames
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            if (animationType == WaitAnimation.WaitAnimationType.SharpEdge)
            {
                // Variante A: SharpEdge
                faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.05, KeyTime.FromPercent(1)));
                faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.1, KeyTime.FromPercent(0.35)));
                faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.9, KeyTime.FromPercent(0.05)));
                faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromPercent(0)));

                faderAnimation.Duration = this.Duration;
                faderAnimation.AutoReverse = false;
            }
            else
            {
                // Variante B: Smooth
                faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.05, KeyTime.FromPercent(0)));
                faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.1, KeyTime.FromPercent(0.65)));
                faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.9, KeyTime.FromPercent(0.95)));
                faderAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromPercent(1)));

                faderAnimation.AutoReverse = true;
                this.faderAnimation.Duration = TimeSpan.FromTicks(this.Duration.TimeSpan.Ticks / 2);
            }

            if (animationStarted)
            {
                this.faderAnimation.BeginTime = this.BeginTime;
                BeginAnimation(OpacityProperty, this.faderAnimation);
            }
        }

        #region Shape-Geometry
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                var geometry = new StreamGeometry { FillRule = FillRule.EvenOdd };

                using (StreamGeometryContext context = geometry.Open())
                {
                    this.DrawGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        /// <summary>
        /// Draws the pie piece
        /// </summary>
        private void DrawGeometry(StreamGeometryContext context)
        {
            Point innerArcStartPoint = ComputeCartesianCoordinate(this.RotationAngle, this.InnerRadius);
            innerArcStartPoint.Offset(this.CentreX, this.CentreY);

            Point innerArcEndPoint = ComputeCartesianCoordinate(this.RotationAngle + this.WedgeAngle, this.InnerRadius);
            innerArcEndPoint.Offset(this.CentreX, this.CentreY);

            Point outerArcStartPoint = ComputeCartesianCoordinate(this.RotationAngle, this.Radius);
            outerArcStartPoint.Offset(this.CentreX, this.CentreY);

            Point outerArcEndPoint = ComputeCartesianCoordinate(this.RotationAngle + this.WedgeAngle, this.Radius);
            outerArcEndPoint.Offset(this.CentreX, this.CentreY);

            bool largeArc = this.WedgeAngle > 180.0;

            var outerArcSize = new Size(this.Radius, this.Radius);
            var innerArcSize = new Size(this.InnerRadius, this.InnerRadius);

            context.BeginFigure(innerArcStartPoint, true, true);
            context.LineTo(outerArcStartPoint, true, true);
            context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
            context.LineTo(innerArcEndPoint, true, true);
            context.ArcTo(innerArcStartPoint, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, true, true);
        }

        /// <summary>
        /// Converts a coordinate from the polar coordinate system to the cartesian coordinate system.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Point ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }
        #endregion
    }
}