using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace WPFCore.Data.StructuredDataReader
{
    public delegate void SkippedLinesProcessorDelegate(object sender, List<string> lines);

    public abstract class FileReaderBase : ReaderBase
    {
        public SkippedLinesProcessorDelegate ProcessLeadingLinesCallback { get; set; }

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


        public virtual void DumpReaderInfo()
        {
            Console.WriteLine("Reader: {0}", (object)this.GetType().Name);
            Console.WriteLine("\tCulture: {0}", (object)this.FileCultureName);
            Console.WriteLine("\tSkipping {0} leading rows", this.SkipLeadingRows);
            Console.WriteLine("\tSkipping {0} trailing rows", this.SkipTrailingRows);
            Console.WriteLine("\tFile has column headers: {0}", this.ContainsColumnHeaders ? "yes" : "no");
        }

    }
}
