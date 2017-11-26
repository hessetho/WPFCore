using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCore.Data.FlexData;

namespace WPFCore.Data.StructuredDataReader
{
    public class FileReaderResult
    {
        internal FileReaderResult(List<FlexColumnDefinition> columnDefinitions)
        {
            this.Rows = new FlexTable<StructuredDataRow>(columnDefinitions);
            this.LeadingRows = new List<string>();
            this.TrailingRows = new List<string>();
        }

        public FlexTable<StructuredDataRow> Rows { get; }

        public List<string> LeadingRows { get; }
        public List<string> TrailingRows { get; }
    }
}
