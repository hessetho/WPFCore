using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace WPFCore.XAML
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType
        {
            get { return typeof(GridLength); }
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength From
        {
            get
            {
                return (GridLength)this.GetValue(GridLengthAnimation.FromProperty);
            }
            set
            {
                this.SetValue(GridLengthAnimation.FromProperty, value);
            }
        }

        private double FromAsDouble
        {
            get
            {
                return ((GridLength)this.From).Value;
            }
        }

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength To
        {
            get
            {
                return (GridLength)this.GetValue(GridLengthAnimation.ToProperty);
            }
            set
            {
                this.SetValue(GridLengthAnimation.ToProperty, value);
            }
        }

        private double ToAsDouble
        {
            get
            {
                return ((GridLength)this.To).Value;
            }
        }

        public GridUnitType GridUnitType
        {
            get { return (GridUnitType)this.GetValue(GridUnitTypeProperty); }
            set { this.SetValue(GridUnitTypeProperty, value); }
        }

        public static readonly DependencyProperty GridUnitTypeProperty =
            DependencyProperty.Register("GridUnitType", typeof(GridUnitType), typeof(GridLengthAnimation), new UIPropertyMetadata(GridUnitType.Pixel));

        public override object GetCurrentValue(object defaultOriginValue,
                                                object defaultDestinationValue,
                                                AnimationClock animationClock)
        {
            if (this.FromAsDouble > this.ToAsDouble)
                return new GridLength((1 - animationClock.CurrentProgress.Value) *
                    (this.FromAsDouble - this.ToAsDouble) + this.ToAsDouble, this.GridUnitType);

            return new GridLength(animationClock.CurrentProgress.Value *
                (this.ToAsDouble - this.FromAsDouble) + this.FromAsDouble, this.GridUnitType);
        }
    }
}
