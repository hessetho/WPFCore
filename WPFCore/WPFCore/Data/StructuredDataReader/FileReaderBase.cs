using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace WPFCore.Data.StructuredDataReader
{
    public abstract class FileReaderBase : ReaderBase
    {
        protected FileReaderBase()
        {
            // initialize correctly
            this.FileCultureInfo = Thread.CurrentThread.CurrentCulture;
            this.SkipLeadingRows = 0;
            this.SkipTrailingRows = 0;
            this.TrailingNullColumns = false;
            this.ContainsColumnHeaders = true;
        }

        /// <summary>
        ///     Gets or sets the file culture info.
        /// </summary>
        /// <value>The file culture info.</value>
        [XmlIgnore]
        public CultureInfo FileCultureInfo { get; set; }

        /// <summary>
        ///     Gets or sets the name of the file culture.
        /// </summary>
        /// <value>The name of the file culture.</value>
        public string FileCultureName
        {
            get { return this.FileCultureInfo.Name; }
            set
            {
                this.FileCultureInfo = value == string.Empty
                                           ? Thread.CurrentThread.CurrentCulture
                                           : CultureInfo.CreateSpecificCulture(value);
            }
        }

        /// <summary>
        ///     Gets or sets the number of leading rows to skip.
        /// </summary>
        /// <value>The number of rows.</value>
        public int SkipLeadingRows { get; set; }

        /// <summary>
        ///     Gets or sets the number of trailing rows to skip.
        /// </summary>
        /// <value>The number of rows.</value>
        public int SkipTrailingRows { get; set; }

        /// <summary>
        ///    Gets or sets whether missing data columns will be treated as NULL values.
        /// </summary>
        public bool TrailingNullColumns { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the file contains column headers.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the file contains column headers; otherwise, <c>false</c>.
        /// </value>
        public bool ContainsColumnHeaders { get; set; }

        /// <summary>
        ///     Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename { get; set; }

    }
}
