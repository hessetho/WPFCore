using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using WPFCore.ViewModelSupport;

namespace WPFCore.XAML.Ribbon
{
    public class RibbonContextualTabGroupViewModel : ViewModelCore
    {
        private string header;
        private readonly ObservableCollection<RibbonTabViewModel> ribbonTabs = new ObservableCollection<RibbonTabViewModel>();
        private readonly ReadOnlyObservableCollection<RibbonTabViewModel> readOnlyRibbonTabs;
        private Visibility visibility;

        public RibbonContextualTabGroupViewModel(string header)
        {
            this.header = header;

            this.readOnlyRibbonTabs = new ReadOnlyObservableCollection<RibbonTabViewModel>(this.ribbonTabs);
        }

        public string Header
        {
            get { return this.header; }
            set
            {
                this.header = value;
                OnPropertyChanged("Header");
            }
        }

        public Brush Background { get; set; }

        public Visibility Visibility
        {
            get { return this.visibility; }
            set
            {
                this.visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        public ReadOnlyObservableCollection<RibbonTabViewModel> RibbonTabs
        {
            get { return this.readOnlyRibbonTabs; }
        }

        public RibbonTabViewModel AddTab(string header)
        {
            var tab = new RibbonTabViewModel(header);
            tab.ContextualTabGroupHeader = this.Header;

            this.ribbonTabs.Add(tab);
            return tab;
        }
    }
}
