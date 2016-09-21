namespace WPFCore.ViewModelSupport
{
    public delegate void SelectedRowChangingEventHandler<T>(object sender, SelectedRowChangingEventArgs<T> e);
    
    public class SelectedRowChangingEventArgs<T>
    {
        public T OldRow { get; private set; }
        public T NewRow { get; private set; }

        public bool Accepted { get; set; }

        public SelectedRowChangingEventArgs(T oldRow, T newRow)
        {
            this.OldRow = oldRow;
            this.NewRow = newRow;
            this.Accepted = true;
        }
    }
}
