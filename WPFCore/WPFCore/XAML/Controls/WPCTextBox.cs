using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFCore.XAML.Controls
{
    [TemplatePart(Name = "PART_ClearButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class WPCTextBox : Control
    {

        public static DependencyProperty TextProperty =
                    DependencyProperty.Register("Text", typeof(string), typeof(WPCTextBox), 
                        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static WPCTextBox()
        {
            // Dem System mitteilen, dass wir einen eigenen Default-Style liefern
            // Dazu werden die Metadaten für das DependencyProperty DefaultStyleKey auf diese Klasse "verbogen"
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPCTextBox), new FrameworkPropertyMetadata(typeof(WPCTextBox), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        }

        private TextBox partTextBox;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Wichtig: PART_ClearButton MUSS zwingend als TemplatePartAttribute der Klasse deklariert worden sein!
            var clearButton = (Button)this.GetTemplateChild("PART_ClearButton");
            if (clearButton != null)
                clearButton.Click += this.ClearButton_Click;

            this.partTextBox = (TextBox)this.GetTemplateChild("PART_TextBox");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.Text = string.Empty;
        }

        public string Text
        {
            get { return ((string)(GetValue(WPCTextBox.TextProperty))); }
            set { SetValue(WPCTextBox.TextProperty, value); }
        }
    }
}
