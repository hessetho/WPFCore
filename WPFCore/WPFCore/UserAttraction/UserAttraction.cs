using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using WPFCore.XAML.Controls;

namespace WPFCore.UserAttraction
{
    public static class UserAttraction
    {
        #region Content Property
        public static readonly DependencyProperty ContentProperty =
                    DependencyProperty.RegisterAttached("Content", typeof(object), typeof(UserAttraction), 
                        new PropertyMetadata(OnContentPropertyChanged));

        public static object GetContent(DependencyObject depObj)
        {
            return depObj.GetValue(ContentProperty);
        }

        public static void SetContent(DependencyObject depObj, object content)
        {
            depObj.SetValue(ContentProperty, content);
        }

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.OldValue == null)
            {
                RegisterContainer(d);
            }
            else if (e.NewValue == null && e.OldValue != null)
            {
                UnregisterContainer(d);
            }
        }
        #endregion Content Property

        #region Placement property
        public static readonly DependencyProperty PlacementProperty =
                    DependencyProperty.RegisterAttached("Placement", typeof(Dock), typeof(UserAttraction),
                        new PropertyMetadata(Dock.Bottom));

        public static Dock GetPlacement(DependencyObject depObj)
        {
            return (Dock)depObj.GetValue(PlacementProperty);
        }

        public static void SetPlacement(DependencyObject depObj, object Placement)
        {
            depObj.SetValue(PlacementProperty, Placement);
        }

        #endregion Placement property

        #region Control wiring / unwiring
        private static void RegisterContainer(DependencyObject d)
        {
            if (d is FrameworkElement)
                RegisterControl(d as FrameworkElement);
            //else if (d is FrameworkContentElement)
            //    RegisterFrameworkContentElement(d as FrameworkContentElement);
            else
                throw new ArgumentException(string.Format("Cannot attach a repository entry to this type of element ({0})", d.GetType()));
        }

        private static void UnregisterContainer(DependencyObject d)
        {
            if (d is FrameworkElement)
                UnregisterControl(d as FrameworkElement);
            //else if (d is FrameworkContentElement)
            //    UnregisterFrameworkContentElement(d as FrameworkContentElement);
        }

        private static void RegisterControl(FrameworkElement fwElement)
        {
            fwElement.MouseEnter += ControlMouseEnter;
            fwElement.MouseLeave += ControlMouseLeave;

            fwElement.Loaded += ControlLoaded;
        }

        private static void ControlLoaded(object sender, RoutedEventArgs e)
        {
            Show(sender as UIElement);
        }

        private static void UnregisterControl(FrameworkElement fwElement)
        {
            fwElement.MouseEnter -= ControlMouseEnter;
            fwElement.MouseLeave -= ControlMouseLeave;

            fwElement.Loaded -= ControlLoaded;
        }
        #endregion

        private static void ControlMouseLeave(object sender, MouseEventArgs e)
        {
            var control = sender as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(control);

            var adorners = layer.GetAdorners(control);
            if (adorners != null)
                foreach (var adorner in adorners.OfType<UserAttractionAdorner>())
                    layer.Remove(adorner);
        }

        private static void ControlMouseEnter(object sender, MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                Show(sender as UIElement);
                e.Handled = true;
            }
        }

        private static void Show(UIElement control)
        {
            if (control == null) return;

            // get the content to be shown
            var content = GetContent(control);
            var contentControl = content as UIElement;

            // create a default control in case the content is pure text
            if (contentControl == null && content is string)
                contentControl = new TextBlock { Text = (string)content };

            // get the placement
            var placement = GetPlacement(control);

            // create the adorner
            var adorner = new UserAttractionAdorner(control, contentControl, placement);
            var layer = AdornerLayer.GetAdornerLayer(control);
            layer.Add(adorner);
        }
    }
}
