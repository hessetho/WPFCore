using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace WPFCore.XAML.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_MoveNext", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MovePrev", Type = typeof(Button))]
    [ContentProperty("Exception")]
    public class ExceptionGallery : Control
    {
        /// <summary>
        /// Tritt auf, wenn die ausgewählte <c>Exception</c> geändert wid.
        /// </summary>
        public event EventHandler SelectedExceptionChanged;

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung der <see cref="Exception"/>
        /// </summary>
        public static DependencyProperty ExceptionProperty =
                        DependencyProperty.Register("Exception", typeof(Exception), typeof(ExceptionGallery),
                        new PropertyMetadata(OnExceptionPropertyChanged));

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung der flachen <c>Exception</c>-Liste
        /// </summary>
        public static DependencyProperty FlatExceptionListProperty =
                        DependencyProperty.Register("FlatExceptionList", typeof(List<Exception>), typeof(ExceptionGallery));

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung der aktuell ausgewählten Ausnahme
        /// </summary>
        public static DependencyProperty SelectedExceptionProperty =
                        DependencyProperty.Register("SelectedException", typeof(Exception), typeof(ExceptionGallery),
                        new PropertyMetadata(OnSelectedExceptionPropertyChanged));

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung von Datum/Uhrzeit der Ausnahme
        /// </summary>
        public static DependencyProperty ExceptionTimeProperty =
                        DependencyProperty.Register("ExceptionTime", typeof(DateTime), typeof(ExceptionGallery),
                        new PropertyMetadata(DateTime.Now));

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung des aktuellen Users/Logins
        /// </summary>
        public static DependencyProperty CurrentUserProperty =
                        DependencyProperty.Register("CurrentUser", typeof(string), typeof(ExceptionGallery),
                        new PropertyMetadata(System.Security.Principal.WindowsIdentity.GetCurrent().Name));

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung der aktuellen Maschine
        /// </summary>
        public static DependencyProperty CurrentMachineProperty =
                        DependencyProperty.Register("CurrentMachine", typeof(string), typeof(ExceptionGallery),
                        new PropertyMetadata(System.Environment.MachineName));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty AdditionalDataProperty =
                        DependencyProperty.Register("AdditionalData", typeof(Dictionary<string, object>), typeof(ExceptionGallery));

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung zusätzlicher Details zur Exception.
        /// </summary>
        public static DependencyProperty ExceptionDetailsProperty =
                        DependencyProperty.Register("ExceptionDetails", typeof(string), typeof(ExceptionGallery));

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung des Flags, das anzeigt, wenn es zusätzlich Details zur Exception gibt.
        /// </summary>
        public static DependencyProperty HasExceptionDetailsProperty =
                        DependencyProperty.Register("HasExceptionDetails", typeof(bool), typeof(ExceptionGallery));

        /// <summary>
        /// <see cref="DependencyProperty"/> zur Speicherung des Flags, das anzeigt, wenn dies eine nicht-fatale Ausnahme ist
        /// </summary>
        public static DependencyProperty IsNonFatalExceptionProperty =
                        DependencyProperty.Register("IsNonFatalException", typeof(bool), typeof(ExceptionGallery));

        private MoveNextCommand moveNextCmd;
        private MovePrevCommand movePrevCmd;

        static ExceptionGallery()
        {
            // Dem System mitteilen, dass wir einen eigenen Default-Style liefern
            // Dazu werden die Metadaten für das DependencyProperty DefaultStyleKey auf diese Klasse "verbogen"
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExceptionGallery),
                                                     new FrameworkPropertyMetadata(typeof(ExceptionGallery)));
        }

        /// <summary>
        /// Setzt die <see cref="Exception"/> welche ausgegeben wird, bzw. liefert diese
        /// </summary>
        public Exception Exception
        {
            get { return ((Exception)(this.GetValue(ExceptionProperty))); }
            set { this.SetValue(ExceptionProperty, value); }
        }

        /// <summary>
        /// Liefert Datum/Uhrzeit der <see cref="Exception"/>
        /// </summary>
        public DateTime ExceptionTime
        {
            get { return ((DateTime)(this.GetValue(ExceptionTimeProperty))); }
            //set { SetValue(ExceptionTimeProperty, value); }
        }

        /// <summary>
        /// Liefert den aktuellen User/Login zur <see cref="Exception"/>
        /// </summary>
        public string CurrentUser
        {
            get { return ((string)(this.GetValue(CurrentUserProperty))); }
            //set { SetValue(CurrentUserProperty, value); }
        }

        /// <summary>
        /// Liefert die aktuelle Maschine zur <see cref="Exception"/>
        /// </summary>
        public string CurrentMachine
        {
            get { return ((string)(this.GetValue(CurrentMachineProperty))); }
            //set { SetValue(CurrentMachineProperty, value); }
        }

        /// <summary>
        /// Liefert eine flache Liste der <c>Exception's</c>
        /// </summary>
        public List<Exception> FlatExceptionList
        {
            get { return ((List<Exception>)(this.GetValue(FlatExceptionListProperty))); }
            private set { this.SetValue(FlatExceptionListProperty, value); }
        }

        /// <summary>
        /// Liefert eine Liste mit Wertepaaren als Zusatzdaten zur Ausnahme bzw. legt diese fest
        /// </summary>
        public Dictionary<string, object> AdditionalData
        {
            get { return ((Dictionary<string, object>)(this.GetValue(ExceptionGallery.AdditionalDataProperty))); }
            set { this.SetValue(ExceptionGallery.AdditionalDataProperty, value); }
        }

        /// <summary>
        /// Liefert zusätzliche Details zur Ausnahme
        /// </summary>
        public string ExceptionDetails
        {
            get { return ((string)(this.GetValue(ExceptionGallery.ExceptionDetailsProperty))); }
            set { this.SetValue(ExceptionGallery.ExceptionDetailsProperty, value); }
        }

        /// <summary>
        /// <c>true</c> wenn es zusätzliche Details gibt, andernfalls <c>false</c>.
        /// </summary>
        public bool HasExceptionDetails
        {
            get { return ((bool)(this.GetValue(ExceptionGallery.HasExceptionDetailsProperty))); }
            set { this.SetValue(ExceptionGallery.HasExceptionDetailsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a non-fatal exception.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a non-fatal exception; otherwise, <c>false</c>.
        /// </value>
        public bool IsNonFatalException
        {
            get { return ((bool)(this.GetValue(ExceptionGallery.IsNonFatalExceptionProperty))); }
            set { this.SetValue(ExceptionGallery.IsNonFatalExceptionProperty, value); }
        }

        /// <summary>
        /// Liefert die aktuell ausgewählte <c>Exception</c> bzw. liefert diese.
        /// </summary>
        /// <remarks>
        /// Style-Templates, welche die <see cref="FlatExceptionList"/> in einer Liste darstellen, sollten
        /// SelectedItem an diese Eigenschaft binden.
        /// </remarks>
        public Exception SelectedException
        {
            get { return ((Exception)(this.GetValue(SelectedExceptionProperty))); }
            set { this.SetValue(SelectedExceptionProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.moveNextCmd = new MoveNextCommand(this);
            this.movePrevCmd = new MovePrevCommand(this);
            this.CommandBindings.AddRange(new[] { new CommandBinding(this.moveNextCmd), new CommandBinding(this.movePrevCmd) });

            // Wichtig: PART_MoveNext und PART_MovePrev MÜSSEN zwingend als TemplatePartAttribute der Klasse
            //          deklariert worden sein!
            var button = this.GetTemplateChild("PART_MoveNext") as Button;
            if (button != null)
            {
                button.Command = this.moveNextCmd;
            }

            button = this.GetTemplateChild("PART_MovePrev") as Button;
            if (button != null)
            {
                button.Command = this.movePrevCmd;
            }
        }

        /// <summary>
        /// Die <see cref="Exception"/>-Eigenschaft wurde geändert.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnExceptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ExceptionGallery;
            var exception = e.NewValue as Exception;
            Debug.Assert(ctrl != null, "DependencyObject is not of type ExceptionGallery");

            if (exception != null)
                ctrl.InitializeForException(exception);
        }

        /// <summary>
        /// Die <see cref="SelectedExceptionProperty"/>-Eigenschaft wurde geändert.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSelectedExceptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ExceptionGallery;
            var exception = e.NewValue as Exception;
            Debug.Assert(ctrl != null, "DependencyObject is not of type ExceptionGallery");

            if (exception != null)
                ctrl.OnSelectedExceptionChanged();

        }

        private void InitializeForException(Exception ex)
        {
            // wenn die erste Ausnahme nicht-fatal ist, wird diese aus der Kette genommen
            // und das interne Flag entsprechend gesetzt
            if (ex is NonFatalException)
            {
                this.IsNonFatalException = true;

                // entferne die nicht-fatale Ausnahme aus der Liste, sofern das "RemoveWrapperException" gesetzt ist.
                if(((NonFatalException)ex).RemoveWrapperException)
                    ex = ex.InnerException;
            }

            // aus der Exception-Hierarchie die flache Liste erzeugen
            this.FlatExceptionList = ListFlattener.GetFlatList(ex);
            this.SelectedException = this.FlatExceptionList[0];
        }

        /// <summary>
        /// Die Auswahl der Ausnahme wurde geändert.
        /// </summary>
        private void OnSelectedExceptionChanged()
        {
            this.ExceptionDetails = this.GetExceptionDetails(this.SelectedException);
            this.HasExceptionDetails = !string.IsNullOrEmpty(this.ExceptionDetails);

            if (this.SelectedExceptionChanged != null)
                this.SelectedExceptionChanged(this, new EventArgs());
        }

        private string GetExceptionDetails(Exception e)
        {
            var details = new StringBuilder();

            if (e is System.Data.SqlClient.SqlException)
            {
                var se = (System.Data.SqlClient.SqlException)e;

                details.AppendLine(string.Format("Class: {0}", se.Class));
                details.AppendLine(string.Format("ClientConnectionId: {0}", se.ClientConnectionId));
                details.AppendLine(string.Format("LineNumber: {0}", se.LineNumber));
                details.AppendLine(string.Format("Number: {0}", se.Number));
                details.AppendLine(string.Format("Procedure: {0}", se.Procedure));
                details.AppendLine(string.Format("Server: {0}", se.Server));
                details.AppendLine(string.Format("State: {0}", se.State));

                foreach (System.Data.SqlClient.SqlError error in se.Errors)
                {
                    details.AppendLine("");
                    details.AppendLine(string.Format("Error: {0}", error.Message));
                    details.AppendLine(string.Format("Class: {0}", error.Class));
                    details.AppendLine(string.Format("LineNumber: {0}", error.LineNumber));
                    details.AppendLine(string.Format("Number: {0}", error.Number));
                    details.AppendLine(string.Format("Procedure: {0}", error.Procedure));
                    details.AppendLine(string.Format("Source: {0}", error.Source));
                    details.AppendLine(string.Format("Server: {0}", error.Server));
                    details.AppendLine(string.Format("State: {0}", error.State));
                }
            }

            return details.ToString();
        }

        public static string GetExceptionText(Exception e)
        {
            var ctrl = new ExceptionGallery();
            ctrl.Exception = e;

            return ctrl.ExceptionText;
        }

        public string ExceptionText
        {
            get
            {
                var sb = new StringBuilder(1000);

                var asmbly = System.Reflection.Assembly.GetEntryAssembly();

                sb.AppendFormat("Application name: {0}\r\n", asmbly.FullName);
                sb.AppendFormat("Application version: {0}\r\n", asmbly.GetName().Version);

                sb.AppendFormat("Exception timestamp: {0}\r\n", this.ExceptionTime);
                sb.AppendFormat("Current user/login: {0}\r\n", this.CurrentUser);
                sb.AppendFormat("Machine name: {0}\r\n", this.CurrentMachine);
                sb.AppendFormat("Thread name: {0}\r\n", System.Threading.Thread.CurrentThread.Name);

                if (this.AdditionalData != null && this.AdditionalData.Count > 0)
                {
                    sb.AppendFormat("Additional data:\r\n");
                    foreach (var data in this.AdditionalData)
                        sb.AppendFormat("\t{0}: {1}\r\n", data.Key, data.Value);
                }

                sb.AppendLine("Assemblies:");
                //sb.Append(this.FormatException(this.FlatExceptionList[0]));
                sb.AppendLine(string.Join("\r\n", Helper.Toolbox.GetReferencedAssemblies().Select(a => string.Format("\t{0}", a))));
                sb.AppendLine("");

                for (var i = 0; i < this.FlatExceptionList.Count; i++)
                {
                    sb.AppendLine(Helper.StringHelper.Replicate("=", 80));
                    sb.AppendFormat("Inner exception {0}:\r\n", i);
                    sb.Append(this.FormatException(this.FlatExceptionList[i]));
                }

                return sb.ToString();
            }
        }

        private StringBuilder FormatException(Exception ex)
        {
            var sb = new StringBuilder(500);

            sb.AppendFormat("Exception of type: {0}\r\n", ex.GetType().Name);
            sb.AppendLine("Message text:");
            sb.AppendLine(ex.Message);

            sb.AppendLine("Stack trace:");
            sb.AppendLine(ex.StackTrace);

            sb.AppendLine("Details:");
            sb.AppendLine(this.GetExceptionDetails(ex));

            if (ex.Data != null && ex.Data.Count > 0)
            {
                sb.AppendLine("Exception data:");
                foreach(DictionaryEntry d in ex.Data)
                    sb.AppendLine(string.Format("{0}: {1}", d.Key, d.Value ?? "<null>" ));
            }

            return sb;
        }
    }

    class ListFlattener
    {
        private readonly List<Exception> exceptions = new List<Exception>();

        public ListFlattener(Exception ex)
        {
            this.AddExceptionToList(ex);
        }

        private void AddExceptionToList(Exception ex)
        {
            this.exceptions.Add(ex);
            if (ex.InnerException != null)
                this.AddExceptionToList(ex.InnerException);
        }

        public static List<Exception> GetFlatList(Exception ex)
        {
            var flattener = new ListFlattener(ex);
            return flattener.exceptions;
        }
    }

    class MoveNextCommand : ICommand
    {
        private readonly ExceptionGallery host;

        public MoveNextCommand(ExceptionGallery host)
        {
            this.host = host;
            host.SelectedExceptionChanged += this.OnSelectExceptionChanged;
        }

        private void OnSelectExceptionChanged(object sender, EventArgs e)
        {
            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Definiert die Methode, die aufgerufen werden soll, wenn der Befehl aufgerufen wird.
        /// </summary>
        /// <param name="parameter">Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null festgelegt werden.</param>
        public void Execute(object parameter)
        {
            var idx = this.host.FlatExceptionList.IndexOf(this.host.SelectedException);
            Debug.Assert(idx < this.host.FlatExceptionList.Count - 1);

            this.host.SelectedException = this.host.FlatExceptionList[idx + 1];

            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Definiert die Methode, mit der ermittelt wird, ob der Befehl im aktuellen Zustand ausgeführt werden kann.
        /// </summary>
        /// <returns>
        /// true, wenn der Befehl ausgeführt werden kann, andernfalls false.
        /// </returns>
        /// <param name="parameter">Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null festgelegt werden.</param>
        public bool CanExecute(object parameter)
        {
            if (this.host.FlatExceptionList == null || this.host.SelectedException == null)
                return false;

            var idx = this.host.FlatExceptionList.IndexOf(this.host.SelectedException);
            return (idx < this.host.FlatExceptionList.Count - 1);
        }

        public event EventHandler CanExecuteChanged;
        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, new EventArgs());
        }

    }

    class MovePrevCommand : ICommand
    {
        private readonly ExceptionGallery host;

        public MovePrevCommand(ExceptionGallery host)
        {
            this.host = host;
            host.SelectedExceptionChanged += this.OnSelectExceptionChanged;
        }

        private void OnSelectExceptionChanged(object sender, EventArgs e)
        {
            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Definiert die Methode, die aufgerufen werden soll, wenn der Befehl aufgerufen wird.
        /// </summary>
        /// <param name="parameter">Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null festgelegt werden.</param>
        public void Execute(object parameter)
        {
            var idx = this.host.FlatExceptionList.IndexOf(this.host.SelectedException);
            Debug.Assert(idx > 0);

            this.host.SelectedException = this.host.FlatExceptionList[idx - 1];

            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Definiert die Methode, mit der ermittelt wird, ob der Befehl im aktuellen Zustand ausgeführt werden kann.
        /// </summary>
        /// <returns>
        /// true, wenn der Befehl ausgeführt werden kann, andernfalls false.
        /// </returns>
        /// <param name="parameter">Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null festgelegt werden.</param>
        public bool CanExecute(object parameter)
        {
            if (this.host.FlatExceptionList == null || this.host.SelectedException == null)
                return false;

            var idx = this.host.FlatExceptionList.IndexOf(this.host.SelectedException);
            return (idx > 0);
        }

        public event EventHandler CanExecuteChanged;
        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, new EventArgs());
        }

    }
}
