namespace WPFCore.DataKiosk
{
    public interface IDataSubscriber
    {
        /// <summary>
        /// Is invoked by the <see cref="DataKiosk"/> when a data item matching the criteria
        /// of the subscriber is published.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDataPublished(object sender, DataPublishedEventArgs e);
    }
}
