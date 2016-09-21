using System.Collections.ObjectModel;

namespace WPFCore.XAML.Ribbon
{
    public class RibbonTabGroupViewModel : RibbonItemBase
    {
        private readonly ObservableCollection<RibbonItemBase> items = new ObservableCollection<RibbonItemBase>();

        public RibbonTabGroupViewModel(string header)
            : base(header)
        {
        }

        public ObservableCollection<RibbonItemBase> Items
        {
            get { return this.items; }
        }

        public RibbonItemBase AddItem(RibbonItemBase item)
        {
            this.items.Add(item);
            return item;
        }
    }
}
