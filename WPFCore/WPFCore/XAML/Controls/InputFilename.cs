// ==========================================================================
// <summary>
// WpfHelper: Sammlung von Hilfsfunktionen und Klassen zur Verwendung mit WPF
// </summary>
//
// $Rev: 97 $
// $Id: InputFilename.cs 97 2011-01-04 10:42:01Z  $
// 
// <copyright file="InputFilename.cs" company="ICEP GmbH">
//      2009-2012 ICEP GmbH, T. Hesse
// </copyright>
// ==========================================================================

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;

namespace WPFCore.XAML.Controls
{
    /// <summary>
    ///     Legt fest, welcher Dateidialog geöffnet wird
    /// </summary>
    public enum DialogStyleEnum
    {
        /// <summary>
        ///     Legt den DialogStyle auf OpenFile fest
        /// </summary>
        OpenFile,

        /// <summary>
        ///     Legt den DialogStyle auf SaveFile fest
        /// </summary>
        SaveFile
    }

    /// <summary>
    ///     Stellt ein Steuerelement dar, welches dem Anwender die Auswahl einer Datei üder den "Datei-Öffnen"-Dialog
    ///     ermöglicht.
    ///     Der Name der Datei wird in einer TextBox dargestellt (nur lesbar), der Dialog wird über einen Button geöffnet.
    /// </summary>
    /// <remarks>
    ///     ( Wichtiger Hinweis zur Entwicklung von Lookless Custom Controls: in AssemblyInfo.cs muss
    ///     "assembly: System.Windows.ThemeInfoAttribute(.., ..)" eingetragen sein! )
    /// </remarks>
    [TemplatePart(Name = "PART_Browse", Type = typeof (Button))]
    [ContentProperty("Filename")]
    public class InputFilename : Control
    {
        /// <summary>
        ///     Property, welches die Eigenschaft Filename repräsentiert.
        /// </summary>
        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.Register("Filename", typeof(string), typeof(InputFilename), new PropertyMetadata(OnFileNameChanged));

        /// <summary>
        ///     Property, welches die Eigenschaft Filename repräsentiert.
        /// </summary>
        public static readonly DependencyProperty FilenameOnlyProperty =
            DependencyProperty.Register("FilenameOnly", typeof(string), typeof(InputFilename));

        /// <summary>
        ///     Property, welches die Eigenschaft DialogStyle repräsentiert
        /// </summary>
        public static DependencyProperty DialogStyleProperty =
            DependencyProperty.Register("DialogStyle", typeof (DialogStyleEnum), typeof (InputFilename),
                new PropertyMetadata(DialogStyleEnum.OpenFile));

        /// <summary>
        ///     Property, welche die Eigenschaft FileFilter" repräsentiert
        /// </summary>
        public static DependencyProperty FileFilterProperty =
            DependencyProperty.Register("FileFilter", typeof (string), typeof (InputFilename));

        /// <summary>
        ///     Property, welche die Eigenschaft Title repräsentiert
        /// </summary>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (InputFilename));

        /// <summary>
        ///     Initialisiert die <see cref="InputFilename" />-Klasse.
        /// </summary>
        static InputFilename()
        {
            // Dem System mitteilen, dass wir einen eigenen Default-Style liefern
            // Dazu werden die Metadaten für das DependencyProperty DefaultStyleKey auf diese Klasse "verbogen"
            DefaultStyleKeyProperty.OverrideMetadata(typeof (InputFilename),
                new FrameworkPropertyMetadata(typeof (InputFilename)));
        }

        /// <summary>
        ///     Setzt den Titel des FileDialogs bzw. liefert diesen
        /// </summary>
        public string Title
        {
            get { return ((string) (this.GetValue(TitleProperty))); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        ///     Setzt den Datei-Filter des FileDialogs bzw. liefert diesen
        /// </summary>
        public string FileFilter
        {
            get { return ((string) (this.GetValue(FileFilterProperty))); }
            set { this.SetValue(FileFilterProperty, value); }
        }

        /// <summary>
        ///     Setzt/Liefert den Dateinamen.
        /// </summary>
        public string Filename
        {
            get { return (string) this.GetValue(FilenameProperty); }
            set { this.SetValue(FilenameProperty, value); }
        }

        /// <summary>
        ///     Setzt/liefert den DialogStyle
        /// </summary>
        public DialogStyleEnum DialogStyle
        {
            get { return ((DialogStyleEnum) (this.GetValue(DialogStyleProperty))); }
            set { this.SetValue(DialogStyleProperty, value); }
        }

        /// <summary>
        ///     Liefert den Pfad ohne den Dateinamen.
        /// </summary>
        /// <value>The directory.</value>
        public string Directory
        {
            get { return Path.GetDirectoryName(this.Filename); }
        }

        /// <summary>
        ///     Liefert nur den Dateinamen ohne den Pfad dazu.
        /// </summary>
        /// <value>The filename only.</value>
        public string FilenameOnly
        {
            get { return (string) this.GetValue(FilenameOnlyProperty); }
            private set { this.SetValue(FilenameOnlyProperty, value); }
        }

        /// <summary>
        ///     Wir per ApplyTemplate ein neues Template zugewiesen, so stellen wir hier
        ///     sicher, dass unser Button_Click noch korrekt funktioniert.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Wichtig: PART_Browse MUSS zwingend als TemplatePartAttribute der Klasse
            //          deklariert worden sein!
            var browseButton = this.GetTemplateChild("PART_Browse") as Button;
            if (browseButton != null)
            {
                browseButton.Click += this.BrowseButtonClick;
            }
        }

        /// <summary>
        ///     Click-Event auf unseren Button -> Den Datei-öffnen-Dialog anzeigen
        /// </summary>
        /// <param name="sender">Der Sender des Ereignisses.</param>
        /// <param name="e">Die Ereignis-Daten.</param>
        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            FileDialog fileDialog;

            if (this.DialogStyle == DialogStyleEnum.OpenFile)
            {
                fileDialog = new OpenFileDialog {Multiselect = false, CheckFileExists = true};


                if (this.Filename != string.Empty)
                {
                    if (File.Exists(this.Filename))
                    {
                        // Die Datei existiert, also wird sie verwendet
                        fileDialog.FileName = this.Filename;
                    }
                    else if (System.IO.Directory.Exists(this.Filename))
                    {
                        // Der Dateiname entspricht einem existierenden Pfad.
                        // Also wird dieser eingestellt und der Dateiname leer gelassen.
                        fileDialog.InitialDirectory = this.Filename;
                        fileDialog.FileName = string.Empty;
                    }
                }
            }
            else
            {
                fileDialog = new SaveFileDialog
                {
                    CheckFileExists = false,
                    OverwritePrompt = true,
                    FileName = this.Filename
                };
            }

            // Allgemeine Einstellungen
            fileDialog.Filter = this.FileFilter;
            fileDialog.Title = this.Title;
            fileDialog.AddExtension = true;
            fileDialog.CheckPathExists = true;

            if (fileDialog.ShowDialog() == true)
            {
                this.Filename = fileDialog.FileName;
            }
        }

        private static void OnFileNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (InputFilename)d;
            var fileName = (string)e.NewValue;

            ctrl.FilenameOnly = Path.GetFileName(fileName);
        }

    }
}