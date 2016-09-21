using System.Windows.Input;
using System.Windows.Media;

namespace WPFCore.XAML.Ribbon
{
    public class RibbonButtonViewModel : RibbonItemBase
    {
        public RibbonButtonViewModel(string header)
            : base(header)
        {
        }

        public ImageSource SmallImageSource { get; set; }
        public ImageSource LargeImageSource { get; set; }

        public ICommand ItemCommand { get; set; }
        public object ItemCommandParameter { get; set; }
    }
}
