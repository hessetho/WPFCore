using System.Windows;

namespace WPFCore.XAML
{
    /// <summary>
    /// This class is used as a data context "bridge" for elements, which are not part of the visual or logical tree.
    /// </summary>
    /// <remarks>
    /// <example>
    /// Declare an instance of <c>BindingBridge</c> in the resource section of your container, like this:
    /// <code>
    ///         &lt;ItemsControl.Resources&gt;
    ///             &lt;amr:BindingBridge x:Key="proxyToList" DataContext="{Binding}"/&gt;
    ///         &lt;/ItemsControl.Resources&gt;
    /// </code>
    /// And use the <c>BindingProxy</c> instance as the binding source for the binding of the element:
    /// <code>
    ///         &lt;ItemsControl.ItemTemplate&gt;
    ///             &lt;DataTemplate&gt;
    ///                 &lt;Button DockPanel.Dock="Left" 
    ///                         Command="{Binding Source={StaticResource proxyToList}, Path=DataContext.CommandRemoveDrilldown}"
    ///                         CommandParameter="{Binding}"&gt;
    ///                     &lt;Image Cursor="None" Source="/Monitoring;component/UI/Icons/delete_16x16.png"
    ///                                   Stretch="None"/&gt;
    ///                 &lt;/Button&gt;
    ///             &lt;/DataTemplate&gt;
    ///         &lt;/ItemsControl.ItemTemplate&gt;
    /// </code>
    /// </example>
    /// <para>
    /// Inspired by:
    /// http://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
    /// </para>
    /// </remarks>
    public class BindingBridge : Freezable
    {
        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object), typeof(BindingBridge), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        public object DataContext
        {
            get { return (object)GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        #region Overrides of Freezable

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingBridge();
        }

        #endregion
    }
}
