using System.ComponentModel;

namespace WPFCore.XAML.Ribbon
{
    /// <summary>
    /// Represents the base view model for a ribbon item
    /// </summary>
    public abstract class RibbonItemBase : INotifyPropertyChanged
    {
        private string header;
        private string keyTip;

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonItemBase"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        public RibbonItemBase(string header)
        {
            this.header = header;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public string Header
        {
            get { return this.header; }
            set
            {
                this.header = value;
                this.OnPropertyChanged("Header");
            }
        }

        /// <summary>
        /// Gets or sets the key tip.
        /// </summary>
        /// <value>
        /// The key tip.
        /// </value>
        public string KeyTip
        {
            get { return this.keyTip; }
            set
            {
                this.keyTip = value;
                this.OnPropertyChanged("KeyTip");
            }
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Called when a property has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
