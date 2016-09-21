namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// Represents the method that will handle the <see cref=""/> event that has no event data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SelectedRowChangedEventHandler<T>(object sender, SelectedRowChangedEventArgs<T> e);
    
    public class SelectedRowChangedEventArgs<T>
    {
        public T OldRow { get; private set; }
        public T NewRow { get; private set; }

        public SelectedRowChangedEventArgs(T oldRow, T newRow)
        {
            this.OldRow = oldRow;
            this.NewRow = newRow;
        }
    }
}
