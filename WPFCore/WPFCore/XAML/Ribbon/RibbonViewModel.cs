using System.Collections.ObjectModel;

namespace WPFCore.XAML.Ribbon
{
    /// <summary>
    /// Represents the root items for a Ribbon
    /// </summary>
    public class RibbonViewModel : ViewModelSupport.ViewModelBase
    {

        private readonly ObservableCollection<RibbonTabViewModel> tabs = new ObservableCollection<RibbonTabViewModel>();
        private readonly ObservableCollection<RibbonContextualTabGroupViewModel> contextualTabs = new ObservableCollection<RibbonContextualTabGroupViewModel>();
        private readonly ObservableCollection<RibbonButtonViewModel> applicationMenuItems = new ObservableCollection<RibbonButtonViewModel>();

        /// <summary>
        /// Gets the ribbon tabs.
        /// </summary>
        public ObservableCollection<RibbonTabViewModel> Tabs
        {
            get { return this.tabs; }
        }

        /// <summary>
        /// Gets the contextual tabs.
        /// </summary>
        public ObservableCollection<RibbonContextualTabGroupViewModel> ContextualTabs
        {
            get { return this.contextualTabs; }
        }

        /// <summary>
        /// Gets the application menu items.
        /// </summary>
        public ObservableCollection<RibbonButtonViewModel> ApplicationMenuItems
        {
            get { return this.applicationMenuItems; }
        }
    }
}
