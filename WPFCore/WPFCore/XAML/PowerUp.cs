using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WPFCore.Helper;

namespace WPFCore.XAML
{
    /// <summary>
    ///     Attached properties to support XAML constructs.
    /// </summary>
    /// <remarks>
    /// <para><c>IsSynchronizedWithCurrentItemFixEnabled</c></para>
    /// <para><c>SelectTextOnFocus</c></para>
    /// <para><c>FocusFirst</c></para>
    /// <para><c>Payload</c></para>
    /// <para><c>ExpectedDataContextType</c></para>
    /// <para>
    /// This property may be attached to any <see cref="FrameworkElement"/> which expects its data context
    /// to be of a specific type. Whenever the data context  is changed, it is checked to be of the
    /// expected type or may be derived from that. If not, an InvalidOperationException is thrown, giving
    /// some details about the actual and the expectet type. However, a DataContext of null is allowed.
    /// </para>
    /// </remarks>
    public static class PowerUp
    {
        #region IsSynchronizedWithCurrentItemFixEnabled

        // ----------------------------------------------------------------------------------------
        // Fix/Workaround to enable controls which derive from the Selector class to respond properly
        // in case the CurrentChanging event cancels a row change.
        // Check this link for more details:
        // http://coderelief.net/2011/11/07/fixing-issynchronizedwithcurrentitem-and-icollectionview-cancel-bug-with-an-attached-property/
        // ----------------------------------------------------------------------------------------

        public static readonly DependencyProperty IsSynchronizedWithCurrentItemFixEnabledProperty =
            DependencyProperty.RegisterAttached("IsSynchronizedWithCurrentItemFixEnabled", typeof(bool), typeof(PowerUp),
                new PropertyMetadata(false, OnIsSynchronizedWithCurrentItemFixEnabledChanged));

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof (Selector))]
        public static bool GetIsSynchronizedWithCurrentItemFixEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsSynchronizedWithCurrentItemFixEnabledProperty);
        }

        public static void SetIsSynchronizedWithCurrentItemFixEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSynchronizedWithCurrentItemFixEnabledProperty, value);
        }

        private static void OnIsSynchronizedWithCurrentItemFixEnabledChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var selector = d as Selector;
            if (selector == null || !(e.OldValue is bool && e.NewValue is bool) || e.OldValue == e.NewValue)
                return;

            var enforceCurrentItemSync = (bool) e.NewValue;
            ICollectionView collectionView = null;

            EventHandler itemsSourceChangedHandler = null;
            itemsSourceChangedHandler = delegate
            {
                collectionView = selector.ItemsSource as ICollectionView;
                if (collectionView == null)
                    collectionView = CollectionViewSource.GetDefaultView(selector);
            };

            SelectionChangedEventHandler selectionChangedHandler = null;
            selectionChangedHandler = delegate
            {
                if (collectionView == null)
                    return;

                if (selector.IsSynchronizedWithCurrentItem == true && selector.SelectedItem != collectionView.CurrentItem)
                {
                    selector.IsSynchronizedWithCurrentItem = false;
                    selector.SelectedItem = collectionView.CurrentItem;
                    selector.IsSynchronizedWithCurrentItem = true;
                }
            };

            if (enforceCurrentItemSync)
            {
                TypeDescriptor.GetProperties(selector)["ItemsSource"].AddValueChanged(selector, itemsSourceChangedHandler);
                selector.SelectionChanged += selectionChangedHandler;
            }
            else
            {
                TypeDescriptor.GetProperties(selector)["ItemsSource"].RemoveValueChanged(selector, itemsSourceChangedHandler);
                selector.SelectionChanged -= selectionChangedHandler;
            }
        }

        #endregion IsSynchronizedWithCurrentItemFixEnabled

        #region SelectTextOnFocus

        public static readonly DependencyProperty SelectTextOnFocusProperty =
            DependencyProperty.RegisterAttached("SelectTextOnFocus", typeof(bool), typeof(PowerUp),
                new PropertyMetadata(false, SelectTextOnFocusPropertyChanged));

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof (TextBox))]
        public static bool GetSelectTextOnFocus(Control control)
        {
            return (bool) control.GetValue(SelectTextOnFocusProperty);
        }

        public static void SetSelectTextOnFocus(Control control, bool value)
        {
            control.SetValue(SelectTextOnFocusProperty, value);
        }

        private static void SelectTextOnFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

            var textBox = (TextBox) dependencyObject;
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

        #endregion SelectTextOnFocus

        #region FocusFirst

        public static readonly DependencyProperty FocusFirstProperty =
            DependencyProperty.RegisterAttached(
                "FocusFirst", typeof(bool), typeof(PowerUp),
                new PropertyMetadata(false, OnFocusFirstPropertyChanged));

        public static bool GetFocusFirst(Control control)
        {
            return (bool) control.GetValue(FocusFirstProperty);
        }

        public static void SetFocusFirst(Control control, bool value)
        {
            control.SetValue(FocusFirstProperty, value);
        }

        private static void OnFocusFirstPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = obj as Control;
            if (control == null || !(args.NewValue is bool))
                return;

            if ((bool) args.NewValue)
            {
                control.Loaded += (sender, e) =>
                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        #endregion FocusFirst

        #region Payload

        public static readonly DependencyProperty PayloadProperty = DependencyProperty.RegisterAttached(
            "Payload",
            typeof (object),
            typeof(PowerUp)
            );

        public static void SetPayload(DependencyObject element, object payload)
        {
            element.SetValue(PayloadProperty, payload);
        }

        public static object GetPayload(DependencyObject element)
        {
            return element.GetValue(PayloadProperty);
        }

        #endregion paylod

        #region ExpectedDataContextType

        /// <summary>
        /// This property may be attached to any <see cref="FrameworkElement"/> which expects its data context
        /// to be of a specific type. Whenever the data context  is changed, it is checked to be of the
        /// expected type or may be derived from that. If not, an InvalidOperationException is thrown, giving
        /// some details about the actual and the expectet type. However, a DataContext of null is allowed.
        /// </summary>
        public static DependencyProperty ExpectedDataContextTypeProperty =
            DependencyProperty.RegisterAttached("ExpectedDataContextType", typeof (Type), typeof (PowerUp),
                new PropertyMetadata(null, OnExpectedDataContextTypeChanged));

        /// <summary>
        /// Gets the expected type of the data context.
        /// </summary>
        /// <param name="depObj">The dep object.</param>
        /// <returns></returns>
        public static Type GetExpectedDataContextType(DependencyObject depObj)
        {
            return (Type) depObj.GetValue(ExpectedDataContextTypeProperty);
        }

        /// <summary>
        /// Sets the expected type of the data context.
        /// </summary>
        /// <param name="depObj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetExpectedDataContextType(DependencyObject depObj, Type value)
        {
            depObj.SetValue(ExpectedDataContextTypeProperty, value);
        }

        /// <summary>
        /// Called when the expected data context type changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.InvalidOperationException">ExpectedDataContext must be used on elements of type FrameworkElement</exception>
        private static void OnExpectedDataContextTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Toolbox.IsInDesignMode()) return;

            var ctrl = d as FrameworkElement;
            if (ctrl == null)
                throw new InvalidOperationException("ExpectedDataContext must be used on elements of type FrameworkElement");

            // check the DataContext AFTER the control has been loaded
            ctrl.Loaded += (sender, eventArgs) =>
            {
                var owner = sender as FrameworkElement;
                Debug.Assert(owner != null, "Owner is null! --> sender is not a FrameworkElement");
                if (owner.DataContext == null) return;

                if (owner.DataContext != null && !GetExpectedDataContextType(owner).IsInstanceOfType(owner.DataContext))
                    throw new InvalidOperationException(
                        string.Format("The DataContext is not of the expected type. Owner: {0} ('{1}'), expected type: {2}, current type: {3}",
                                            owner.GetType().Name,
                                            owner.Name,
                                            GetExpectedDataContextType(owner).Name,
                                            owner.DataContext.GetType().Name));
            };

            // check changes of the DataContext AFTER the control has been loaded
            ctrl.DataContextChanged += (sender, eventArgs) =>
            {
                var owner = sender as FrameworkElement;
                Debug.Assert(owner != null, "Owner is null! --> sender is not a FrameworkElement");
                if (owner.DataContext == null || owner.IsLoaded == false) return;

                if (owner.DataContext != null && !GetExpectedDataContextType(owner).IsInstanceOfType(owner.DataContext))
                    throw new InvalidOperationException(
                        string.Format("The DataContext is not of the expected type. Owner: {0} ('{1}'), expected type: {2}, current type: {3}",
                                            owner.GetType().Name,
                                            owner.Name,
                                            GetExpectedDataContextType(owner).Name,
                                            owner.DataContext.GetType().Name));
            };
        }

        #endregion ExpectedDataContextType
    }
}