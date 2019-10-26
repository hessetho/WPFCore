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

        public ImageSource ButtonImage
        {
            get
            {
                if (this.SmallImageSource != null)
                    return this.SmallImageSource;
                else if (this.LargeImageSource != null)
                    return this.LargeImageSource;

                return null;
            }
        }

    }
}
