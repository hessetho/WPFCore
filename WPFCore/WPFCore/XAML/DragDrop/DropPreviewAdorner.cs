using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFCore.XAML.DragDrop
{
    /// <summary>
    /// Definition eines Dekorators für die Drop-Vorschau.
    /// </summary>
    public class DropPreviewAdorner : Adorner
    {
        /// <summary>
        /// Distanz zum linken Rand.
        /// </summary>
        private double left;

        /// <summary>
        /// Distanz zum oberen Rand.
        /// </summary>
        private double top;

        /// <summary>
        /// Dient der Darstellung des Adorners
        /// </summary>
        private readonly ContentPresenter presenter;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="DropPreviewAdorner"/>-Klasse.
        /// </summary>
        /// <param name="feedbackUI">Das UI-Element, welches das Drag-Feedback liefern soll.</param>
        /// <param name="adornedElt">Das dekorierte UI-Element.</param>
        public DropPreviewAdorner(UIElement feedbackUI, UIElement adornedElt)
            : base(adornedElt)
        {
            this.presenter = new ContentPresenter {Content = feedbackUI, IsHitTestVisible = false};
        }

        /// <summary>
        /// Liefert die Distanz zum linken Rand bzw. setzt diesen.
        /// </summary>
        /// <value>Die Distanz zum linken Rand.</value>
        public double Left
        {
            get
            {
                return this.left;
            }

            set
            {
                this.left = value;
                this.UpdatePosition();
            }
        }

        /// <summary>
        /// Liefert die Distanz zum oberen Rand bzw. setzt diesen.
        /// </summary>
        /// <value>Die Distanz zum oberen Rand.</value>
        public double Top
        {
            get
            {
                return this.top;
            }

            set
            {
                this.top = value;
                this.UpdatePosition();
            }
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
        /// Gibt eine <see cref="T:System.Windows.Media.Transform"/> für den Adorner zurück, die derzeit auf das gestaltete Element angewendet wird.
        /// </summary>
        /// <param name="transform">Die Transformation, die gerade auf das gestaltete Element angewendet wird.</param>
        /// <returns>
        /// Eine Transformation, die auf den Adorner angewendet werden soll.
        /// </returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(this.Left, this.Top));
            if (this.Left > 0)
            {
                Visibility = Visibility.Visible;
            }

            result.Children.Add(base.GetDesiredTransform(transform));

            return result;
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
            this.presenter.Measure(constraint);
            return this.presenter.DesiredSize;
        }

        /// <summary>
        /// Positioniert beim Überschreiben in einer abgeleiteten Klasse untergeordnete Elemente und bestimmt eine Größe für eine abgeleitete <see cref="T:System.Windows.FrameworkElement"/>-Klasse.
        /// </summary>
        /// <param name="finalSize">Der letzte Bereich im übergeordneten Element, in dem dieses Element sich selbst und seine untergeordneten Elemente anordnen soll.</param>
        /// <returns>Die tatsächlich verwendete Größe.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.presenter.Arrange(new Rect(finalSize));
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
            return this.presenter;
        }

        /// <summary>
        /// Aktualisiert die Position des dekorierten Elements.
        /// </summary>
        private void UpdatePosition()
        {
            AdornerLayer layer = Parent as AdornerLayer;
            if (layer != null)
            {
                layer.Update(AdornedElement);
            }
        }
    }
}