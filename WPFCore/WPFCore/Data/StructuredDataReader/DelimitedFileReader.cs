using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using WPFCore.Data.FlexData;
using WPFCore.Helper;

namespace WPFCore.Data.StructuredDataReader
{
    public class DelimitedFileReader : FileReaderBase
    {
        public DelimitedFileReader()
        {
            this.ColumnDelimiter = "\\t";
            this.ColumnDefinitions = new FlexColumnDefinitionCollection();
        }

        /// <summary>
        ///     Gets or sets the column delimiter
        /// </summary>
        /// <value>The column delimiter.</value>
        public string ColumnDelimiter { get; set; }

        private string InternalColumnDelimiter
        {
            get { return this.ColumnDelimiter == "\\t" ? "\t" : this.ColumnDelimiter; }
        }

        /// <summary>
        ///     Beginnt die strukturelle Analyse der Datenquelle
        /// </summary>
        public override FlexColumnDefinitionCollection DoAnalyseStructure()
        {
            if (this.Filename == string.Empty)
                throw new ApplicationException("Filename not specified");

            if (!File.Exists(this.Filename))
                throw new FileNotFoundException(string.Format("File '{0}' not found", this.Filename));

            // Ländereinstellung merken und setzen
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = this.FileCultureInfo;
            var columnDefinitionCollection = new FlexColumnDefinitionCollection();

            try
            {
                columnDefinitionCollection.Clear();

                using (var tr = new StreamReader(this.Filename, Encoding.GetEncoding(1252)))
                {
                    string currentLine = tr.ReadLine();

                    // zunächst die geforderte Zahl Zeilen überlesen
                    int count = this.SkipLeadingRows;
                    while (currentLine != null && count-- > 0)
                        currentLine = tr.ReadLine();

                    // Die notwendigen Spalten anlegen
                    if (this.ContainsColumnHeaders)
                    {
                        var emptyCnt = 0;
                        // Wenn die erste Zeile die Spaltennamen enthält, werden die hier ausgelesen
                        string[] columnNames = currentLine.SplitQuoted(this.InternalColumnDelimiter[0]);
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            columnNames[i] = columnNames[i].Replace("\"", "").Trim(); // remove quotes and blanks!
                            if (string.IsNullOrEmpty(columnNames[i]))
                                columnNames[i] = string.Format("unnamed_{0}", emptyCnt++);

                            columnDefinitionCollection.Add(columnNames[i], typeof (object), columnNames[i]);
                        }

                        // nächste Zeile lesen (= 1. Datenzeile)
                        currentLine = tr.ReadLine();
                    }
                    else
                    {
                        // Die erste Zeile enthält keine Spaltennamen, also erzeugen wir "künstliche" Namen
                        string[] columnNames = currentLine.SplitQuoted(this.InternalColumnDelimiter[0]);
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            columnNames[i] = "col" + i;
                            columnDefinitionCollection.Add(columnNames[i], typeof (object), columnNames[i]);
                        }
                    }

                    int rowcount = 15;
                    int unIdentifiedCounter = columnDefinitionCollection.Count;

                    // Nun die Datenzeilen einlesen und deren Inhalt interpretieren
                    do
                    {
                        //string[] values = currentLine.Split(new[] {InternalColumnDelimiter}, StringSplitOptions.None);
                        string[] values = currentLine.SplitQuoted(this.InternalColumnDelimiter[0]);

                        for (int c = 0; c < Math.Min(values.Length, columnDefinitionCollection.Count); c++)
                        {
                            string currentValue = values[c];
                            var currentColumn = (FlexColumnDefinition)columnDefinitionCollection[c];

                            // Leere Spalten werden ignoriert
                            if (string.IsNullOrEmpty(currentValue)) continue;

                            // zunächst prüfen wir die möglichen Datentypen für den aktuellen Wert
                            bool isInt = false;
                            bool isDouble = false;
                            bool isDateTime = false;

                            int i;
                            double d;
                            DateTime dt;

                            isInt = int.TryParse(currentValue, out i);
                            //isDouble = double.TryParse(currentValue, out d);
                            isDouble = StringHelper.IsDouble(currentValue);
                            isDateTime = DateTime.TryParse(currentValue, out dt);

                            // nun legen wir fest, welchen davon wir nehmen
                            // dabei werden die Regeln zunehmend restriktiver
                            if (currentColumn.ColumnType == typeof (object))
                            {
                                // Bislang noch kein Typ vorgegeben
                                // Nacheinander prüfen: double, DateTime, int
                                if (isDouble)
                                    currentColumn.ColumnType = typeof(double);
                                else if (isDateTime)
                                    currentColumn.ColumnType = typeof (DateTime);
                                else if (isInt)
                                    currentColumn.ColumnType = typeof (int);

                                // Wenn ein Typ vergeben wurde, reduzieren wir den Zähler der Unindentifizierten
                                if (currentColumn.ColumnType != typeof (object))
                                    unIdentifiedCounter--;
                            }
                            else if (currentColumn.ColumnType == typeof (int))
                            {
                                // Bislang als int klassifiziert
                                if (isDateTime)
                                    // Problem: erst int, dann DateTime erkannt -> auf string setzen
                                    currentColumn.ColumnType = typeof(string);
                                else if (isDouble && !isInt)
                                    currentColumn.ColumnType = typeof(double);
                                else if (!isInt)
                                    // Oha! Zunächst als Int erkannt, nun nicht mehr -> muss ein String sein
                                    currentColumn.ColumnType = typeof (string);
                            }
                            else if (currentColumn.ColumnType == typeof (double))
                            {
                                // bislang als double klassifiziert
                                if (isDateTime)
                                    // Problem: erst double, dann DateTime erkannt -> auf string setzen
                                    currentColumn.ColumnType = typeof (string);
                                else if (!isDouble)
                                    // Oha! Zunächst als Double erkannt, nun nicht mehr -> muss ein String sein
                                    currentColumn.ColumnType = typeof(string);
                            }
                            else if (currentColumn.ColumnType == typeof (DateTime))
                                // bislang als DateTime klassifiziert
                                if (!isDateTime)
                                    // Problem: nicht mehr DateTime -> auf string setzen
                                    currentColumn.ColumnType = typeof (string);
                        }
                    } while ((currentLine = tr.ReadLine()) != null && unIdentifiedCounter > 0); // --rowcount > 0);

                    // zuletzt alle undefinierten Spaltentypen auf string setzen
                    foreach (FlexColumnDefinition colDef in columnDefinitionCollection)
                    {
                        if (colDef.ColumnType == typeof (object))
                            colDef.ColumnType = typeof (string);
                    }


                    foreach (FlexColumnDefinition colDef in columnDefinitionCollection)
                        Debug.WriteLine("{0} - {1}", colDef.ColumnPropertyName, colDef.ColumnType);
                }
            }
            catch
            {
                // hier hin wenn sich die Zeilenstruktur plötzlich ändert
                throw;
            }
            finally
            {
                // Ländereinstellung restaurieren
                Thread.CurrentThread.CurrentCulture = ci;
            }

            return columnDefinitionCollection;
        }

        public override FileReaderResult DoReadData(string dataFileName)
        {
            if (dataFileName == string.Empty)
                throw new ApplicationException("Filename not specified");

            if (!File.Exists(dataFileName))
                throw new FileNotFoundException(string.Format("File '{0}' not found", dataFileName));

            var result = new FileReaderResult(this.ColumnDefinitions.AsList());
            var lastRows = new Queue<StructuredDataRow>();
            int rowCounter = 0;

            // Ländereinstellung merken und setzen
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = this.FileCultureInfo;

            try
            {
                using (var tr = new StreamReader(dataFileName, Encoding.GetEncoding(1252)))
                {
                    string currentLine = tr.ReadLine();
                    rowCounter++;

                    // zunächst die geforderte Zahl Zeilen überlesen
                    int count = this.SkipLeadingRows;
                    while (currentLine != null && count-- > 0)
                    {
                        result.LeadingRows.Add(currentLine);
                        currentLine = tr.ReadLine();
                        rowCounter++;
                    }

                    // die übersprungenen Zeilen verarbeiten lassen (wenn eingerichtet)
                    this.ProcessLeadingLinesCallback?.Invoke(this, result.LeadingRows);

                    // Wenn die erste Zeile die Spaltennamen enthält, werden die hier ausgelesen und überprüft
                    if (this.ContainsColumnHeaders)
                    {
                        string invalidColNames = string.Empty;
                        var emptyCnt = 0;

                        string[] columnNames = currentLine.SplitQuoted(this.InternalColumnDelimiter[0]);
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            columnNames[i] = columnNames[i].Trim().Replace("\"", ""); // remove quotes and leading/trailing blanks!
                            if (string.IsNullOrEmpty(columnNames[i]))
                                columnNames[i] = string.Format("unnamed_{0}", emptyCnt++);

                            if (!this.ColumnDefinitions.Contains(columnNames[i]))
                                invalidColNames += (invalidColNames == string.Empty ? "" : ", ") + columnNames[i];
                        }

                        if (invalidColNames != string.Empty)
                        {
                            var ex = new InvalidDataException( string.Format( "Unbekannte Spalten: {0}",invalidColNames));
//                            rows.AddError(ex, "Ungültige Dateistruktur erkannt.");
                            return result;
                        }

                        // nächste Zeile lesen (= 1. Datenzeile)
                        currentLine = tr.ReadLine();
                        rowCounter++;
                    }

                    var errorCount = 0;
                    // Nun die Daten einlesen ==================================================
                    // Zeilenweise einlesen
                    do
                    {
                        //    Zu beachten: OnRowProcessing wird im Thread-Kontext des Readers ausgeführt. Kontextwechsel (etwa zur Anzeige des Fortschritts) kosten enorm viel Zeit
                        this.OnRowProcessing(dataFileName, rowCounter);

                        // Die aktuelle Zeile zerlegen
                        string[] values = currentLine.SplitQuoted(this.InternalColumnDelimiter[0]);

                        // Eine neue Datenzeile erzeugen
                        var row = result.Rows.NewRow<StructuredDataRow>("");
                        row.RowNumber = rowCounter;
                        row.RawData = currentLine;

                        // und befüllen
                        row.Populate(values);

                        if (row.HasReadError)
                            errorCount++;

                        // Die neue Datenzeile der Liste anhängen
                        result.Rows.Add(row);

                        // Die neue Zeile dem Speicher der letzten Zeilen hinzufügen und in diesem nur soviele Zeilen behalten, wie SkipTrailingRows erfordert
                        if (this.SkipTrailingRows > 0)
                        {
                            lastRows.Enqueue(row);
                            if (lastRows.Count > this.SkipTrailingRows) lastRows.Dequeue();
                        }

                        // Und mit der nächsten Zeile weiter machen
                        currentLine = tr.ReadLine();
                        rowCounter++;
                    } while (currentLine != null && errorCount<50);
                }
            }
            catch (Exception e)
            {
                //rows.AddError(e, "Unbehandelte Ausnahme entdeckt.");
            }
            finally
            {
                // Zum Schluss löschen wir die Zeilen, welche
                // am Dateiende überlesen werden sollen, wieder raus
                if (this.SkipTrailingRows > 0)
                    while (lastRows.Count > 0)
                    {
                        var row = lastRows.Dequeue();
                        result.Rows.Remove(row);
                        result.TrailingRows.Add(row.RawData);
                    }

                // Ländereinstellung restaurieren
                Thread.CurrentThread.CurrentCulture = ci;
            }

            // abschließend die Anzahl gelesener Zeilen veröffentlichen (muss nicht mit der tatsächlichen übereinstimmen)
            this.OnRowProcessing(dataFileName, rowCounter, true);

            return result;
        }


        public override void DumpReaderInfo()
        {
            base.DumpReaderInfo();
            Console.WriteLine("\tDelimiter: {0}", (object)this.ColumnDelimiter);
        }
    }
}
