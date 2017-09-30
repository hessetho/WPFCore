using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using WPFCore.Data.FlexData;

namespace WPFCore.Data.StructuredDataReader
{
    [XmlInclude(typeof (DelimitedFileReader))]
    public abstract class ReaderBase
    {

        protected ReaderBase()
        {
            this.ReaderType = this.GetType().Name;
        }

        /// <summary>
        ///     Liefert <c>true</c> wenn die Reader-Konfiguration ungültig ist, andernfalls <c>false</c>.
        /// </summary>
        /// <remarks>
        ///     Das Kennzeichen wird auf <c>true</c> gesetzt, wenn die Konfiguration nicht zu einer gültigen
        ///     Klasse kompiliert werden kann.
        /// </remarks>
        [XmlIgnore]
        public bool IsInvalidConfiguration { get; protected set; }

        /// <summary>
        ///     Liefert die Spaltendefinitionen bzw. legt diese fest
        /// </summary>
        public FlexColumnDefinitionCollection ColumnDefinitions { get; set; }

        /// <summary>
        ///     Liefert die ID-Spalten bzw. legt diese fest
        /// </summary>
        //public RowIdentifierProperties RowIdentifierProperties { get; set; }

        /// <summary>
        ///     Liefert den Typ des Readers
        /// </summary>
        public string ReaderType { get; set; }

        /// <summary>
        ///     Liefert den Namen der readerDefinition bzw. legt diese fest
        /// </summary>
        public string ReaderDefinitionName { get; set; }


        /// <summary>
        ///     Beginnt die strukturelle Analyse der Datenquelle
        /// </summary>
        public abstract FlexColumnDefinitionCollection DoAnalyseStructure();

        /// <summary>
        ///     Liest die Daten aus der Datenquelle ein
        /// </summary>
        public abstract FlexTable<StructuredDataRow> DoReadData(string dataFileName);

        /*
        private DataRowFactory rowFactory;

        /// <summary>
        /// Erzeugt eine neue Instanz von <see cref="DataRowBase"/>
        /// </summary>
        /// <remarks>
        /// Wenn erforderlich wird die <see cref="DataRowFactory"/> erzeugt.
        /// </remarks>
        /// <returns></returns>
        internal DataRowBase CreateDataRow()
        {
            if(this.rowFactory==null)
                rowFactory=new DataRowFactory(this.ColumnDefinitions, this.RowIdentifierProperties);

            return rowFactory.CreateDataRow();
        }

        /// <summary>
        /// Erzeugt eine neue DataRowFactory.
        /// Das ist sinnvoll, wenn sich zwischenzeitlich die ColumnDefinition geändert hat!
        /// </summary>
        internal void ResetDataRowFactory()
        {
            rowFactory = new DataRowFactory(this.ColumnDefinitions, this.RowIdentifierProperties);
        }
        */

        #region RowProcessingEvent
        public event RowProcessingEventHandler RowProcessing;

        protected void OnRowProcessing(string filename, int rowsProcessed)
        {
            this.OnRowProcessing(filename, rowsProcessed, false);
        }

        protected void OnRowProcessing(string filename, int rowsProcessed, bool isLastRow)
        {
            if (this.RowProcessing != null)
            {
                string context = "unknown";
                this.RowProcessing(this,
                                   new RowProgressEventArgs(string.Format("{0}: {1}", context, filename), rowsProcessed,
                                                            isLastRow));
            }
        }
        #endregion RowProcessingEvent

        #region Load & Save
        public void Save(string filename)
        {
            var serializer = new XmlSerializer(typeof (ReaderBase));
            using (TextWriter tw = new StreamWriter(filename))
            {
                serializer.Serialize(tw, this);
            }
        }

        public static ReaderBase Load(string filename)
        {
            ReaderBase m = null;
            var serializer = new XmlSerializer(typeof (ReaderBase));

            using (TextReader tr = new StreamReader(filename))
            {
                m = (ReaderBase) serializer.Deserialize(tr);
            }
            return m;
        }
#endregion
    }
}
