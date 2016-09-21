using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFCore.XAML
{
    /// <summary>
    /// XAML-Attached Property. Sorgt dafür, dass beim Ansteuern eines Controls (z.B. einer TextBox), der gesamte Text ausgewählt wird
    /// </summary>
    public static class SelectTextOnFocus
    {
        public static readonly DependencyProperty ActiveProperty = 
            DependencyProperty.RegisterAttached(
                "Active", typeof (bool), typeof (SelectTextOnFocus),
                new PropertyMetadata(false, ActivePropertyChanged));

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetActive(Control control)
        {
            return (bool)control.GetValue(ActiveProperty);
        }

        public static void SetActive(Control control, bool value)
        {
            control.SetValue(ActiveProperty, value);
        }

        private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var textBox = d as TextBox;
                if ((e.NewValue as bool?).GetValueOrDefault(false))
                {
                    textBox.GotKeyboardFocus += OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                }
                else
                {
                    textBox.GotKeyboardFocus -= OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                }
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = GetParentFromVisualTree(e.OriginalSource);

            if (dependencyObject == null)
                return;

            var textBox = (TextBox) dependencyObject as TextBox;
            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static DependencyObject GetParentFromVisualTree(object source)
        {
            DependencyObject parent = source as UIElement;
            while (parent != null && !(parent is TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        private static void OnKeyboardFocusSelectText(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }
    }
}