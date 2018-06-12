using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFCore.XAML.Controls
{
    public sealed class ShowCapsLock : ContentControl
    {
        public static readonly DependencyProperty ShowMessageProperty =
            DependencyProperty.Register("ShowMessage", typeof(bool), typeof(ShowCapsLock), new PropertyMetadata(false));

        static ShowCapsLock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShowCapsLock), new FrameworkPropertyMetadata(typeof(ShowCapsLock)));
        }

        public bool ShowMessage
        {
            get { return (bool)GetValue(ShowMessageProperty); }
            set { SetValue(ShowMessageProperty, value); }
        }

        public ShowCapsLock()
        {
            IsKeyboardFocusWithinChanged += (s, e) => this.RecomputeShowMessage();
            PreviewKeyDown += (s, e) => this.RecomputeShowMessage();
            PreviewKeyUp += (s, e) => this.RecomputeShowMessage();
        }

        private void RecomputeShowMessage()
        {
            this.ShowMessage = IsKeyboardFocusWithin && System.Console.CapsLock;
        }
    }
}
