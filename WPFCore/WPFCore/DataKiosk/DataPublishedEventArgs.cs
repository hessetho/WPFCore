namespace WPFCore.DataKiosk
{
    public class DataPublishedEventArgs
    {
        /// <summary>
        /// Returns the published data item.
        /// </summary>
        public object DataItem { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataItem">The published data item</param>
        public DataPublishedEventArgs(object dataItem)
        {
            this.DataItem = dataItem;
        }
    }
}
