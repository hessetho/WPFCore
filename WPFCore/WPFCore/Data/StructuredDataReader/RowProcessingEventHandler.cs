namespace WPFCore.Data.StructuredDataReader
{
    public delegate void RowProcessingEventHandler(object sender, RowProgressEventArgs e);

    public class RowProgressEventArgs
    {
        public readonly string Message;
        public readonly int ProcessedRows;
        public readonly bool IsLastProcessedRow;

        public RowProgressEventArgs(string message, int processedRows)
        {
            this.Message = message;
            this.ProcessedRows = processedRows;
            this.IsLastProcessedRow = false;
        }

        public RowProgressEventArgs(string message, int processedRows, bool isLastProcessedRow)
        {
            this.Message = message;
            this.ProcessedRows = processedRows;
            this.IsLastProcessedRow = isLastProcessedRow;
        }
    }
}
