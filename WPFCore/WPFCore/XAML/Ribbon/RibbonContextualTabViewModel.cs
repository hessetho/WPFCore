using System.Windows;
using System.Windows.Media;

namespace WPFCore.XAML.Ribbon
{
    public class RibbonContextualTabViewModel : RibbonTabViewModel
    {
        private Brush background;
        private Visibility visibility;

        public RibbonContextualTabViewModel(string header)
            : base(header)
        {
        }

        public Brush Background
        {
            get { return this.background; }
            set
            {
                this.background = value;
                this.OnPropertyChanged("Background");
            }
        }

        public Visibility Visibility
        {
            get { return this.visibility; }
            set
            {
                this.visibility = value;
                this.OnPropertyChanged("Visibility");
            }
        }
    }
}
