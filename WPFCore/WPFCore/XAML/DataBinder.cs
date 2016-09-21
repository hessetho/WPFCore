using System.Windows;
using System.Windows.Data;

namespace WPFCore.XAML
{
    /// <summary>
    /// This static class is a helper to retrieve the value of a bound item.
    /// It creates a fake property, binds this to a data item using a given 
    /// binding expression and lets the system do the business to retrieve 
    /// the value (by calling .GetValue)
    /// </summary>
    /// <remarks>
    /// This is extremely useful to extract data from bound lists as the data is displayed there
    /// </remarks>
    public static class DataBinder
    {
        public static System.Windows.DependencyProperty DummyProperty =
                System.Windows.DependencyProperty.Register("Dummy", typeof(object), typeof(DataBinder));

        public static object Eval(object dataContainer, string bindingExpression)
        {
            var dummyDO = new DependencyObject();
            var binding = new Binding(bindingExpression) { Source = dataContainer };
            BindingOperations.SetBinding(dummyDO, DummyProperty, binding);
            return dummyDO.GetValue(DummyProperty);
        }

        public static object Eval(object dataContainer, Binding binding)
        {
            return Eval(dataContainer, binding.Path.Path);
        }
    }
}
