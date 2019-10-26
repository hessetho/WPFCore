using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFCore.XAML.Controls
{
    public class Balloon : Shape
    {
        public static DependencyProperty CallOutPlacementProperty = BalloonPresenter.CallOutPlacementProperty
            .AddOwner(typeof(Balloon), new FrameworkPropertyMetadata(CallOutPlacement.TopLeft, FrameworkPropertyMetadataOptions.Inherits, OnCalloutPlacementChanged));

        public CallOutPlacement CallOutPlacement
        {
            get { return (CallOutPlacement)GetValue(CallOutPlacementProperty); }
            set { SetValue(CallOutPlacementProperty, value); }
        }

        /// <summary>
        /// Ruft einen Wert ab, der die <see cref="T:System.Windows.Media.Geometry"/> der <see cref="T:System.Windows.Shapes.Shape"/> darstellt.
        /// </summary>
        /// <returns>
        /// Die <see cref="T:System.Windows.Media.Geometry"/> der <see cref="T:System.Windows.Shapes.Shape"/>.
        /// </returns>
        protected override Geometry DefiningGeometry
        {
            get
            {
                var geometry = new StreamGeometry { FillRule = FillRule.EvenOdd };

                using (StreamGeometryContext context = geometry.Open())
                {
                    this.DrawGeometry(context);
                }

                geometry.Freeze();

                return geometry;
            }
        }

        private void DrawGeometry(StreamGeometryContext context)
        {
            double h = ActualHeight;
            double w = ActualWidth;

            if (w == 0 && h == 0)
            {
                return;
            }

            Debug.WriteLine(string.Format("Drawing balloon geometry ({0:0}|{1:0})", w,h));
            w--;
            h--;

            if (CallOutPlacement == CallOutPlacement.LeftTop || CallOutPlacement == CallOutPlacement.LeftBottom)
            {
                double myW = (w - 30);
                double myH = (h - 20);

                // Starting point: top left corner, indented by 10
                context.BeginFigure(new Point(10, 10), true, true);

                // first arc right
                context.ArcTo(new Point(20, 0), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // line to top right corner and arc down
                context.LineTo(new Point(20 + myW, 0), true, true);
                context.ArcTo(new Point(30 + myW, 10), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // line to bottom right corner and arc left
                context.LineTo(new Point(30 + myW, 10 + myH), true, true);
                context.ArcTo(new Point(20 + myW, 20 + myH), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // line to bottom left corner and arc up
                context.LineTo(new Point(20, 20 + myH), true, true);
                context.ArcTo(new Point(10, 10 + myH), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                if (CallOutPlacement == CallOutPlacement.LeftTop)
                {
                    // line to top with CallOut to the left
                    context.LineTo(new Point(10, 30), true, true);
                    context.LineTo(new Point(0, 20), true, true);
                    context.LineTo(new Point(10, 10), true, true);
                }
                else if (CallOutPlacement == CallOutPlacement.LeftBottom)
                {
                    // line to top with CallOut to the left
                    context.LineTo(new Point(0, myH), true, true);
                    context.LineTo(new Point(10, myH-10), true, true);
                    // let the shape auto close
                    //context.LineTo(new Point(10, 15), true, true);
                }
            }
            else if (CallOutPlacement == CallOutPlacement.RightBottom || CallOutPlacement == CallOutPlacement.RightTop)
            {
                double myW = (w - 30);
                double myH = (h - 20);

                // Starting point: top left corner
                context.BeginFigure(new Point(0, 10), true, true);

                // first arc right
                context.ArcTo(new Point(10, 0), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // line to top right corner and arc down
                context.LineTo(new Point(10 + myW, 0), true, true);
                context.ArcTo(new Point(20 + myW, 10), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                if (CallOutPlacement == CallOutPlacement.RightTop)
                {
                    // CallOut to the right
                    context.LineTo(new Point(30+myW, 20), true, true);
                    context.LineTo(new Point(20+myW, 30), true, true);

                    // line to bottom
                    context.LineTo(new Point(20 + myW, 10 + myH), true, true);
                }
                else if (CallOutPlacement == CallOutPlacement.RightBottom)
                {
                    // line to bottom
                    context.LineTo(new Point(20 + myW, myH - 10), true, true);

                    // CallOut to the right
                    context.LineTo(new Point(30 + myW, myH), true, true);
                    context.LineTo(new Point(20 + myW, myH + 10), true, true);
                }

                // arc left
                context.ArcTo(new Point(10 + myW, 20 + myH), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // line to bottom left corner and arc up
                context.LineTo(new Point(10, 20 + myH), true, true);
                context.ArcTo(new Point(0, 10 + myH), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // let the shape auto-close
            }
            else if (CallOutPlacement == CallOutPlacement.TopLeft || CallOutPlacement == CallOutPlacement.TopRight)
            {
                double myW = (w - 20);
                double myH = (h - 30);

                // Starting point: top left corner
                context.BeginFigure(new Point(0, 20), true, true);

                // first arc right
                context.ArcTo(new Point(10, 10), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                if (CallOutPlacement == CallOutPlacement.TopLeft)
                {
                    // CallOut to the right
                    context.LineTo(new Point(20, 0), true, true);
                    context.LineTo(new Point(30, 10), true, true);

                    // line to right
                    context.LineTo(new Point(10 + myW, 10), true, true);
                }
                else if (CallOutPlacement == CallOutPlacement.TopRight)
                {
                    // line to right
                    context.LineTo(new Point(myW - 10, 10), true, true);

                    // CallOut to the right
                    context.LineTo(new Point(myW, 0), true, true);
                    context.LineTo(new Point(10 + myW, 10), true, true);
                }

                // arc down
                context.ArcTo(new Point(20 + myW, 20), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // line to to bottom right corner
                context.LineTo(new Point(20 + myW, 20 + myH), true, true);
                // arc left
                context.ArcTo(new Point(10 + myW, 30 + myH), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // line to bottom left corner and arc up
                context.LineTo(new Point(10, 30 + myH), true, true);
                context.ArcTo(new Point(0, 20 + myH), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // let the shape auto-close
            }
            else if (CallOutPlacement == CallOutPlacement.BottomLeft || CallOutPlacement == CallOutPlacement.BottomRight)
            {
                double myW = (w - 20);
                double myH = (h - 30);

                // Starting point: top left corner
                context.BeginFigure(new Point(0, 10), true, true);

                // first arc right
                context.ArcTo(new Point(10, 0), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);
                context.LineTo(new Point(10 + myW, 0), true, true);

                // arc down
                context.ArcTo(new Point(20 + myW, 10), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // line to to bottom right corner
                context.LineTo(new Point(20 + myW, 10 + myH), true, true);
                // arc left
                context.ArcTo(new Point(10 + myW, 20 + myH), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);


                if (CallOutPlacement == CallOutPlacement.BottomLeft)
                {
                    // line to left
                    context.LineTo(new Point(30, 20 + myH), true, true);

                    // CallOut down
                    context.LineTo(new Point(20, 30 + myH), true, true);
                    context.LineTo(new Point(10, 20 + myH), true, true);
                }
                else if (CallOutPlacement == CallOutPlacement.BottomRight)
                {

                    // CallOut to the right
                    context.LineTo(new Point(myW, 30 + myH), true, true);
                    context.LineTo(new Point(myW - 10, 20 + myH), true, true);

                    // line to left
                    context.LineTo(new Point(10, 20 + myH), true, true);
                }

                // arc up
                context.ArcTo(new Point(0, 10 + myH), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                // let the shape auto-close
            }

        }

        private static void OnCalloutPlacementChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var shape = (Shape)depObj;
            shape.InvalidateVisual();
            Debug.WriteLine("Placement changed from {0} to {1}", e.OldValue, e.NewValue);
        }

        private static void OnCallOutLocationChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var shape = (Shape)depObj;
            shape.InvalidateVisual();
            Debug.WriteLine("Location changed from {0} to {1}", e.OldValue, e.NewValue);
        }
    }
}
