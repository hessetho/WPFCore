using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WPFCore.XAML.Ribbon
{
    public class RibbonTabViewModel : RibbonItemBase
    {
        private bool isSelected;
        private string contextualTabGroupHeader;
        private readonly ObservableCollection<RibbonTabGroupViewModel> groups = new ObservableCollection<RibbonTabGroupViewModel>();

        public RibbonTabViewModel(string header):base(header)
        {
        }

        public ObservableCollection<RibbonTabGroupViewModel> Groups
        {
            get { return this.groups; }
        }

        public RibbonTabGroupViewModel AddGroup(string header)
        {
            var group = new RibbonTabGroupViewModel(header);
            this.Groups.Add(group);
            return group;
        }

        public RibbonTabGroupViewModel AddGroup(RibbonTabGroupViewModel group)
        {
            this.Groups.Add(group);
            return group;
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                this.isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        public string ContextualTabGroupHeader
        {
            get { return this.contextualTabGroupHeader; }
            set
            {
                this.contextualTabGroupHeader = value;
                this.OnPropertyChanged("ContextualTabGroupHeader");
            }
        }

    }
}
