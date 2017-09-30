using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCore.Data.FlexData;

namespace WPFCore.Data.StructuredDataReader
{
    public class StructuredDataRow : FlexRow
    {
        private readonly StringBuilder readErrors = new StringBuilder();
        private readonly List<DataRowError> rowErrors = new List<DataRowError>();


        public int RowNumber { get; set; }

        /// <summary>
        ///     Liefert die Rohdaten bzw. setzt diese
        /// </summary>
        [Display(AutoGenerateField = false)]
        public string RawData { get; set; }

        /// <summary>
        ///     Liefert den Hash der Datenzeile bzw. setzt diesen
        /// </summary>
        [Display(AutoGenerateField = false)]
        internal byte[] Hash { get; private set; }

        /// <summary>
        ///     Liefert <c>true</c>, wenn es Einlesefehler gab.
        /// </summary>
        public bool HasReadError
        {
            get { return this.rowErrors.Count > 0; }
        }

        /// <summary>
        ///     Liefert eine (optional) Fehlermeldung, welche über Einlesefehler Auskunft gibt
        ///     bzw. setzt diese
        /// </summary>
        public string ReadErrorMessages
        {
            get { return this.readErrors.ToString(); }
        }

        /// <summary>
        ///     Liefert die Liste der Einlesefehler
        /// </summary>
        [Display(AutoGenerateField = false)]
        public List<DataRowError> RowErrors
        {
            get { return this.rowErrors; }
        }

        protected void AddReadError(string propertyName, string msg)
        {
            this.AddReadError(propertyName, msg, null);
        }

        protected void AddReadError(string propertyName, string msg, Exception e)
        {
            if (e != null)
                Debug.WriteLine("  --> Exception recorded as row error.");

            this.readErrors.AppendLine(string.Format("{0}: {1}", propertyName, msg));
            this.rowErrors.Add(new DataRowError(this, e, propertyName, msg));
        }

        protected void AddReadError(string propertyName, string msg, Exception e, string readValue)
        {
            if (e != null)
                Debug.WriteLine("  --> Exception recorded as row error.");

            this.readErrors.AppendLine(string.Format("{0}: {1} ({2})", propertyName, msg, readValue));
            this.rowErrors.Add(new DataRowError(this, e, propertyName, msg, readValue));
        }


        /// <summary>
        ///     Populates the item with data
        /// </summary>
        /// <param name="values"></param>
        public void Populate(string[] values)
        {
            // zunächst alle verfügbaren Spalten übernehmen
            for (int i = 0; i < Math.Min(values.Count(), base.ColumnPropertyDescriptors.Count); i++)
            {

                var pd = ColumnPropertyDescriptors[i];
                string val = values[i];

                // evtl. vorhandene Null-Ersetzung übernehmen
                //if (string.IsNullOrEmpty(values[i]) && pd.NullValueHandler.TreatmentRule == NullTreatmentEnum.FixedText)
                //    val = pd.NullValueHandler.ReplacementString.Replace("\"", "\\\"");

                // Und wenn wir einen Wert haben, wird dieser gespeichert
                if (!string.IsNullOrEmpty(val))
                    try
                    {
                        if (pd.PropertyType == typeof(string))
                            this.Values[pd.Name] = val;
                        else if (pd.PropertyType == typeof(int))
                            this.Values[pd.Name] = Convert.ToInt32(val);
                        else if (pd.PropertyType == typeof(double))
                            this.Values[pd.Name] = Convert.ToDouble(val);
                        else if (pd.PropertyType == typeof(DateTime))
                            this.Values[pd.Name] = Convert.ToDateTime(val);
                    }
                    catch (FormatException fe)
                    {
                        this.AddReadError(pd.Name,
                                          "Der gelesene Wert konnte nicht in den Typ der Spalte konvertiert werden. Spaltendefinition prüfen.",
                                          fe, val);
                    }
                    catch (InvalidCastException ice)
                    {
                        this.AddReadError(pd.Name,
                                          "Der gelesene Wert konnte nicht in den Typ der Spalte konvertiert werden. Spaltendefinition prüfen.",
                                          ice, val);
                    }
                    catch (OverflowException oe)
                    {
                        this.AddReadError(pd.Name, "OverflowException detected.", oe);
                    }
            }
/*
            // Nun noch jene Properties bearbeiten, die sich Werte aus anderen Spalten kopieren
            foreach (
                ColumnPropertyDescriptor pdc in
                    this.propertyDescriptors.Cast<ColumnPropertyDescriptor>()
                        .Where(pdc => pdc.NullValueHandler.TreatmentRule == NullTreatmentEnum.OtherColumn)
                        .Where(pdc => this.Values[pdc.Name] == null))
            {
                if (this.Values.ContainsKey(pdc.NullValueHandler.ReplacementString))
                    // ReplacementString enthält den Namen der anderen Eigenschaft
                    this.Values[pdc.Name] = this.Values[pdc.NullValueHandler.ReplacementString];
                else
                    this.AddReadError(pdc.Name,
                                      string.Format(
                                          "Null-value replacement definition points to an invalid column: '{0}' does not exist",
                                          pdc.NullValueHandler.ReplacementString));
            }

            // Jetzt noch alle Properties prüfen, die keine Null-Werte erlauben
            foreach (
                ColumnPropertyDescriptor pdc in
                    this.propertyDescriptors.Cast<ColumnPropertyDescriptor>()
                        .Where(pdc => !pdc.AllowsNull)
                        .Where(pdc => this.Values[pdc.Name] == null))
            {
                if (this.rowIdentifierProperties != null && this.rowIdentifierProperties.Contains(pdc.Name))
                {
                    this.AddReadError(pdc.Name, "Null-Value on identifier property detected. Row is invalid.");

                    // Standard-Fehler-Identifier setzen
                    this.IdentifyingValues.Clear();
                    this.IdentifyingValues.Add("InvalidPK");

                    // Datensatz ist ungültig
                    this.IsInvalid = true;
                }
                else
                    this.AddReadError(pdc.Name, "Null-Value detected.");
            }

            // Die Werte des RowIdentifiers übernehmen
            foreach (var rowIdentifier in this.rowIdentifierProperties)
            {
                this.IdentifyingValues.Add(string.Format("{0}", this[rowIdentifier] ?? "<NULL>"));
            }

            // Zuletzt den Hash berechnen
            this.ComputeHash();
*/
        }

    }
}
