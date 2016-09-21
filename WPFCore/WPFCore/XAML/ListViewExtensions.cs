using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFCore.XAML
{
    public static class ListViewExtensions
    {
        /// <summary>
        /// Copies the contents of an <see cref="ItemsControl"/> to the clipboard,
        /// using TAB as separator
        /// </summary>
        /// <param name="itemslist">The ItemsControl</param>
        public static void CopyToClipboard(this ListView itemslist)
        {
            CopyToClipboard(itemslist, '\t');
        }

        /// <summary>
        /// Copies the contents of an <see cref="ItemsControl"/> to the clipboard        
        /// </summary>
        /// <param name="itemslist">The ItemsControl</param>
        /// <param name="delimiter">Column separator</param>
        public static void CopyToClipboard(this ListView itemslist, char delimiter)
        {
            StringBuilder cp = new StringBuilder(1000);

            GridView grid = (GridView)itemslist.View;

            // create the header
            StringBuilder header = new StringBuilder(100);
            foreach (GridViewColumn col in grid.Columns)
            {
                header.Append(col.Header);
                header.Append(delimiter);
            }
            cp.AppendLine(header.ToString());

            foreach (object item in itemslist.Items)
            {
                StringBuilder line = new StringBuilder(100);
                foreach (GridViewColumn col in grid.Columns)
                {

                    if (col.DisplayMemberBinding != null)
                    {
                        Binding b = (Binding)col.DisplayMemberBinding;

                        object o = DataBinder.Eval(item, b);
                        line.Append(o);
                        line.Append(delimiter);
                    }
                    else if (col.CellTemplate != null)
                    {
                        FrameworkElement elm = (FrameworkElement)((DataTemplate)col.CellTemplate).LoadContent();
                        BindingExpression be = null;

                        if (elm is TextBlock)
                        {
                            TextBlock tb = (TextBlock)elm;
                            be = tb.GetBindingExpression(TextBlock.TextProperty);
                        }
                        else if (elm is Label)
                        {
                            Label l = (Label)elm;
                            be = l.GetBindingExpression(Label.ContentProperty);
                        }
                        else
                        {
                            be = elm.GetBindingExpression(FrameworkElement.DataContextProperty);
                        }

                        if (be != null)
                        {
                            object o = DataBinder.Eval(item, be.ParentBinding);

                            line.Append(o != null ? o.ToString() : "");
                            line.Append(delimiter);
                        }
                    }
                }

                cp.AppendLine(line.ToString());
            }

            Clipboard.SetText(cp.ToString());
        }
    }
}
