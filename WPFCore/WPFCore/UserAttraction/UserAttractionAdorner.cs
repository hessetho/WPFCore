using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WPFCore.XAML.Controls;

namespace WPFCore.UserAttraction
{
    public class UserAttractionAdorner : Adorner
    {
        private BalloonPresenter balloon;
        private Dock adornerPlacement;

        public UserAttractionAdorner(UIElement adornedElement, UIElement content, Dock adornerPlacement) : base(adornedElement)
        {
            this.adornerPlacement = adornerPlacement;
            var callOutPlacement = CallOutPlacement.TopLeft;
            switch (adornerPlacement)
            {
                case Dock.Bottom:
                    callOutPlacement = CallOutPlacement.TopLeft;
                    break;
                case Dock.Left:
                    callOutPlacement = CallOutPlacement.RightTop;
                    break;
                case Dock.Right:
                    callOutPlacement = CallOutPlacement.LeftTop;
                    break;
                case Dock.Top:
                    callOutPlacement = CallOutPlacement.BottomLeft;
                    break;
            }

            this.balloon = new BalloonPresenter()
            {
                Content = content,
                CallOutPlacement = callOutPlacement
            };
            this.balloon.IsHitTestVisible = false;
        }

        /// <summary>
        /// Ruft die Anzahl visueller untergeordneter Elemente innerhalb dieses Elements ab.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// Die Anzahl visueller untergeordneter Elemente für dieses Element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Implementiert ein beliebiges benutzerdefiniertes Messverhalten für den Adorner.
        /// </summary>
        /// <param name="constraint">Eine Größe, auf die der Adorner beschränkt werden soll.</param>
        /// <returns>
        /// Ein <see cref="T:System.Windows.Size"/>-Objekt, das den vom Adorner benötigten Layoutplatz darstellt.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.balloon.Measure(constraint);
            return this.balloon.DesiredSize;
        }

        /// <summary>
        /// Positioniert beim Überschreiben in einer abgeleiteten Klasse untergeordnete Elemente und bestimmt eine Größe für eine abgeleitete <see cref="T:System.Windows.FrameworkElement"/>-Klasse.
        /// </summary>
        /// <param name="finalSize">Der letzte Bereich im übergeordneten Element, in dem dieses Element sich selbst und seine untergeordneten Elemente anordnen soll.</param>
        /// <returns>Die tatsächlich verwendete Größe.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.balloon.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Überschreibt <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/> und gibt am angegebenen Index aus einer Auflistung untergeordneter Elemente ein untergeordnetes Element zurück.
        /// </summary>
        /// <param name="index">Der nullbasierte Index des angeforderten untergeordneten Elements in der Auflistung.</param>
        /// <returns>
        /// Das angeforderte untergeordnete Element. Dabei sollte nicht null zurückgegeben werden. Wenn der bereitgestellte Index außerhalb des Bereichs liegt, wird eine Ausnahme ausgelöst.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index > 1)
                throw new IndexOutOfRangeException();

            return this.balloon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();

            if (AdornedElement is FrameworkElement ctrl)
            {
                switch (adornerPlacement)
                {
                    case Dock.Left:
                        result.Children.Add(new TranslateTransform(-this.ActualWidth, -12.5));
                        break;
                    case Dock.Top:
                        result.Children.Add(new TranslateTransform(0, -this.ActualHeight));
                        break;
                    case Dock.Right:
                        result.Children.Add(new TranslateTransform(ctrl.ActualWidth, -12.5));
                        break;
                    case Dock.Bottom:
                        result.Children.Add(new TranslateTransform(0, ctrl.ActualHeight));
                        break;
                }
            }

            result.Children.Add(base.GetDesiredTransform(transform));
            return result;
        }
    }
}
