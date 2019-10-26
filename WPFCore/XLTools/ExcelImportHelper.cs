using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WPFCore.Data.FlexData;
using WPFCore.Data.Performance;
using WPFCore.StatusText;
using Excel = NetOffice.ExcelApi;

namespace WPFCore.XLTools
{
    /// <summary>
    /// Install-Package NetOfficeFw.Excel -Version 1.7.4.6
    /// </summary>
    public class ExcelImportHelper
    {
        private int headerRow = 1;
        private int? firstDataRow;

        private object lockObj = new object();
        private bool stopProcessing;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExcelImportHelper()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="channel">Name of the channel to post processing updates to (uses <see cref="StatusTextBroker"/>)</param>
        public ExcelImportHelper(string channel)
        {
            this.ChannelName = channel;
        }

        /// <summary>
        /// Name of the channel to send status messages to (<see cref="StatusTextBroker.UpdateStatusText(string, object, string)"/> for details)
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// Gets (or sets) the row number containing the column headers.
        /// </summary>
        public int HeaderRow
        {
            get { return headerRow; }
            set { headerRow = value; }
        }

        /// <summary>
        /// Reads data from an excel file and retuns this as a <see cref="FlexTable{FlexRow}"/>
        /// </summary>
        /// <param name="fullFileName">full file name of the excel file</param>
        /// <returns></returns>
        public FlexTable<FlexRow> LoadFromExcel(string fullFileName)
        {
            return this.LoadFromExcel<FlexRow>(fullFileName);
        }

        /// <summary>
        /// Reads data from an excel file and retuns this as a <see cref="FlexTable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public FlexTable<T> LoadFromExcel<T>(string fullFileName) where T : FlexRow
        {
            if (!File.Exists(fullFileName))
                throw new FileNotFoundException(fullFileName);

            SingleStopWatch pi = null;
            var flexTable = new FlexTable<T>();
            this.stopProcessing = false;

            // start excel and turn off msg boxes

            this.UpdateStatus("Waiting for Excel to open...");
            using (Excel.Application excelApplication = new Excel.Application())
            {
                this.UpdateStatus("Excel opened");

                try
                {
                    excelApplication.DisplayAlerts = false;
                    excelApplication.EnableEvents = false;

                    var workbook = excelApplication.Workbooks.Open(fullFileName);
                    var worksheet = (Excel.Worksheet)workbook.Worksheets[1];

                    var allCells = worksheet.UsedRange;
                    var colCount = allCells.Columns.Count;
                    var rowCount = allCells.Rows.Count;
                    var colHeader = new Dictionary<int, string>();
                    var continueProcessing = true;

                    object[,] allCellsValues = (object[,])allCells.Value;

                    // get header row
                    this.UpdateStatus("Reading column headers");
                    for (int idx = 1; idx <= colCount; idx++)
                    {
                        if (this.stopProcessing) break;
                        var cell = allCellsValues[HeaderRow, idx];

                        if (cell != null)
                        {
                            var header = cell.ToString().Trim();
                            if (!colHeader.ContainsValue(header))
                                colHeader.Add(idx, header);
                            else
                            {
                                this.UpdateStatus(string.Format("ERROR! Duplicate column name found: {0}", header));
                                continueProcessing = false;
                            }
                        }
                    }

                    //// let the header be validated (if registered to the event)
                    //if (continueProcessing && this.ValidateHeader != null)
                    //{
                    //    this.UpdateStatus("Validating header against mapping specification");

                    //    var eventArgs = new ValidationEventArgs(colHeader.Values.ToList());
                    //    this.ValidateHeader(this, eventArgs);
                    //    if (eventArgs.Cancel)
                    //        continueProcessing = false;
                    //}

                    // now read the rows (if validation was a success)
                    if (continueProcessing)
                    {
                        //if (RequestRequiredColumnNames != null)
                        //{
                        //    var requiredColumnNames = this.RequestRequiredColumnNames(colHeader.Values.ToList());

                        //    // build the table: create all required columns
                        //    foreach (var header in colHeader.Values.Where(h => requiredColumnNames.Contains(h)))
                        //        flexTable.AddColumn(header, typeof(object));

                        //    // remove all unnecessary columns from the column lookup
                        //    foreach (var droppedColumnName in colHeader.Values.Except(requiredColumnNames).ToList())
                        //    {
                        //        var item = colHeader.First(ch => ch.Value == droppedColumnName);
                        //        colHeader.Remove(item.Key);
                        //    }

                        //}
                        //else
                        {
                            // build the table: create all columns found in the excel.
                            foreach (var columnName in colHeader.Values)
                                flexTable.AddColumn(columnName, typeof(object));
                        }

                        var startData = (!this.firstDataRow.HasValue) ? HeaderRow + 1 : this.firstDataRow.Value;

                        this.UpdateStatus("Reading data from Excel ...");
                        pi = PerformanceCenter.StartTiming("Excel", "reading cells");

                        for (int rowIdx = startData; rowIdx < rowCount; rowIdx++)
                        {
                            if (this.stopProcessing) break;
                            this.UpdateStatus("Progress", string.Format("{0}/{1}", rowIdx, rowCount));

                            // create a new row and copy all relevant cells
                            var row = flexTable.NewRow(rowIdx.ToString());
                            for (int colIdx = 1; colIdx <= colCount; colIdx++)
                                if (colHeader.ContainsKey(colIdx))
                                    row[colHeader[colIdx]] = allCellsValues[rowIdx, colIdx];

                            // skip completely empty rows!
                            if (row.ContainsValues())
                                flexTable.Add(row);
                        }
                    }
                    else
                        flexTable = null;
                }
                finally
                {
                    if (pi != null)
                    {
                        pi.StopTiming();
                        this.UpdateStatus(string.Format(@"{0} rows read in {1:hh\:mm\:ss}", flexTable.Count, pi.StoppedTime));
                    }

                    // ensure that the excel instance get's closed
                    excelApplication.Quit();
                    Debug.WriteLine("Excel closed");
                }
            }

            return flexTable;
        }

        /// <summary>
        /// Stops the current processing of a file.
        /// </summary>
        public void StopProcessing()
        {
            lock (this.lockObj)
                this.stopProcessing = true;
        }

        private void UpdateStatus(string message)
        {
            if (!string.IsNullOrEmpty(ChannelName))
                StatusTextBroker.UpdateStatusText(ChannelName, this, message);
        }
        private void UpdateStatus(string category, string message)
        {
            if (!string.IsNullOrEmpty(ChannelName))
                StatusTextBroker.UpdateStatusText(ChannelName, this, category, message);
        }

    }
}
