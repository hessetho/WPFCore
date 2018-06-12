using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using WPFCore.Helper;

namespace WPFCore.XAML
{
    public static class FlowDocumentLoader
    {

        public static DependencyProperty DocumentNameProperty =
                    DependencyProperty.RegisterAttached("DocumentName", typeof(string), typeof(FlowDocumentLoader),
                        new PropertyMetadata(OnDocumentNameChanged));

        public static string GetDocumentName(DependencyObject depObj)
        {
            return ((string)(depObj.GetValue(FlowDocumentLoader.DocumentNameProperty)));
        }

        public static void SetDocumentName(DependencyObject depObj, string value)
        {
            depObj.SetValue(FlowDocumentLoader.DocumentNameProperty, value);
        }

        private static void OnDocumentNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Toolbox.IsInDesignMode()) return;

            if (e.NewValue != null && e.OldValue == null)
            {
                var flowDocContainer = d as FlowDocumentScrollViewer;
                if (flowDocContainer == null) return;

                var xamlName = (string)e.NewValue;

                var assembly = Assembly.GetEntryAssembly();

                using (var stream = assembly.GetManifestResourceStream(xamlName))
                {
                    if (stream == null)
                        throw new ArgumentOutOfRangeException("DocumentName", e.NewValue, string.Format("The requested resource does not exist in the assembly '{0}'. Make sure that you set the Build Action to 'Embedded Resource'.", assembly.GetName()));

                    flowDocContainer.Document = XamlReader.Load(stream) as FlowDocument;
                }

            }

        }
    }
}
