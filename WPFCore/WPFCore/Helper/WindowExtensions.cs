using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;

namespace WPFCore.Helper
{
    ///<summary>
    /// Erweiterungs-Methoden für die Window-Klasse
    ///</summary>
    [DebuggerStepThrough]
    public static class WindowExtensions
    {
        /// <summary>
        /// Speichert die Größe und Position des Fensters
        /// </summary>
        /// <param name="win">Das Fenster.</param>
        private static void SaveSizeAndPosition(Window win)
        {
            if (win == null) return;
            Debug.Assert(string.IsNullOrEmpty(win.Name) == false, string.Format("Name property of the window is not set. ({0})", win.GetType().Name));

            // Registry-Schlüssel holen bzw. erzeugen, unter dem die Daten gespeichert werden
            var key = Registry.CurrentUser.CreateSubKey(AppContext.RegistryPath + win.Name);
            if (key == null)
                throw new ApplicationException(string.Format("Could not create registry key '{0}'.", AppContext.RegistryPath + win.Name));

            // Aktuelles Display ermitteln
            var screen = WpfScreen.GetScreenFrom(win);

            // Fensterposition und -größe ermitteln und auf absolute Werte des aktuellen Displays umrechnen
            var boundaryRect = win.RestoreBounds;
            if (boundaryRect != Rect.Empty)     // Empty kommt in seltenen Fällen vor! Dann nix speichern
            {
                boundaryRect.Offset(-screen.WorkingArea.Left, -screen.WorkingArea.Top);

                // Und in die Registry schreiben
                using (CultureHelper.UseSpecificCulture("en-US"))
                {
                    key.SetValue("Bounds", boundaryRect.ToString().Replace(';', ','));
                    key.SetValue("Display", screen.DeviceName);
                }
            }
        }

        public static bool HasStoredSizeAndPosition(this Window win)
        {
            if (win == null) return false;
            Debug.Assert(string.IsNullOrEmpty(win.Name) == false, string.Format("Name property of the window is not set. ({0})", win.GetType().Name));

            // Callback einrichten, der dafür sorgt, dass die Daten beim Schließen des Fensters gespeichert werden
            win.Closing += delegate(object sender, CancelEventArgs e)
                               {
                                   var window = sender as Window;
                                   SaveSizeAndPosition(window);
                               };

            // Zugriff auf den Schlüssel in der Registry. Ist dieser noch nicht vorhanden, wird kommentarlos abgebrochen.
            var key = Registry.CurrentUser.OpenSubKey(AppContext.RegistryPath + win.Name);
            if (key == null)
                return false;

            var bk = key.GetValue("Bounds");
            if (bk == null || bk.ToString() == "Empty")
                return false;

            return true;
        }

        /// <summary>
        /// Stellt die letzte bekannte Position und Größe eines Fensters wieder her.
        /// Sorgt dafür, dass beim Schließen des Fensters, diese Angaben in der Registry gespeichert werden.
        /// Um effizient zu funktionieren, empfiehlt es sich <c>WindowStartupLocation</c> auf den Wert <c>Manual</c> zu setzen.
        /// </summary>
        /// <param name="win">Das Fenster.</param>
        public static void RestoreSizeAndPosition(this Window win)
        {
            if (win == null) return;
            Debug.Assert(string.IsNullOrEmpty(win.Name) == false, string.Format("Name property of the window is not set. ({0})", win.GetType().Name));

            // Callback einrichten, der dafür sorgt, dass die Daten beim Schließen des Fensters gespeichert werden
            win.Closing += delegate(object sender, CancelEventArgs e)
                               {
                                   var window = sender as Window;
                                   SaveSizeAndPosition(window);
                               };

            // Zugriff auf den Schlüssel in der Registry. Ist dieser noch nicht vorhanden, wird kommentarlos abgebrochen.
            var key = Registry.CurrentUser.OpenSubKey(AppContext.RegistryPath + win.Name);
            if (key == null)
                return;

            using (CultureHelper.UseSpecificCulture("en-US"))
            {
                var bk = key.GetValue("Bounds");
                if (bk == null || bk.ToString() == "Empty")
                    return;

                var bounds = Rect.Parse(bk.ToString());
                var screenName = (string)key.GetValue("Display");
                var screen = WpfScreen.GetScreenName(screenName) ?? WpfScreen.Primary;

                win.Top = bounds.Top + screen.WorkingArea.Top;
                win.Left = bounds.Left + screen.WorkingArea.Left;

                // Die Fenstergröße nur bei manuell vergrößerbaren Fenstern speichern
                if (win.SizeToContent == SizeToContent.Manual)
                {
                    win.Width = bounds.Width;
                    win.Height = bounds.Height;
                }
            }
        }

        ///<summary>
        /// Speichert den Wert einer Einstellung für ein Fenster
        ///</summary>
        ///<param name="win">Das zugehörige Fenster</param>
        ///<param name="settingName">Name der Einstellung</param>
        ///<param name="value">Wert der Einstellung</param>
        public static void SaveSetting(this Window win, string settingName, object value)
        {
            if (win == null) return;
            Debug.Assert(string.IsNullOrEmpty(win.Name) == false, string.Format("Name property of the window is not set. ({0})", win.GetType().Name));
            RegistryKey key = Registry.CurrentUser.CreateSubKey(AppContext.RegistryPath + win.Name);

            key.SetValue(settingName, value);
        }

        ///<summary>
        /// Liefert den gespeicherten Wert einer Einstellung für ein Fenster
        ///</summary>
        ///<param name="win">Das zugehörige Fenster</param>
        ///<param name="settingName">Name der Einstellung</param>
        ///<returns>Der aktuelle Wert der Einstellung, oder <c>null</c> wenn die Einstellung nicht existiert</returns>
        public static object GetSetting(this Window win, string settingName)
        {
            if (win == null) return null;
            Debug.Assert(string.IsNullOrEmpty(win.Name) == false, string.Format("Name property of the window is not set. ({0})", win.GetType().Name));
            RegistryKey key = Registry.CurrentUser.CreateSubKey(AppContext.RegistryPath + win.Name);

            return key.GetValue(settingName);
        }

        ///<summary>
        /// Speichert den Wert einer Einstellung für ein Fenster
        ///</summary>
        /// <remarks>
        /// Der <see cref="double"/>-Wert wird in der Formatierung der <see cref="CultureInfo.InvariantCulture"/> als
        /// Zeichenfolge gespeichert
        /// </remarks>
        ///<param name="win">Das zugehörige Fenster</param>
        ///<param name="settingName">Name der Einstellung</param>
        ///<param name="value">Wert der EInstellung als <see cref="double"/></param>
        public static void SaveSettingDouble(this Window win, string settingName, double value)
        {
            if (win == null) return;
            win.SaveSetting(settingName, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        ///<summary>
        /// Liefert den gespeicherten Wert einer Einstellung für ein Fenster
        ///</summary>
        /// <remarks>
        /// Der <see cref="double"/>-Wert wird in der Formatierung der <see cref="CultureInfo.InvariantCulture"/> aus
        /// der gespeicherten Zeichenfolge konvertiert.
        /// </remarks>
        ///<param name="win">Das zugehörige Fenster</param>
        ///<param name="settingName">Name der Einstellung</param>
        ///<param name="defaultValue">Standardwert, der geliefert wird, wenn die Einstellung nicht vorhanden ist</param>
        ///<returns>Wert der Einstellung als <see cref="double"/></returns>
        public static double GetSettingDouble(this Window win, string settingName, double defaultValue)
        {
            if (win == null) return defaultValue;
            object value = win.GetSetting(settingName);
            return value == null ? defaultValue : Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Bestimmt, ob ein Fenster Modal (d.h. per ShowDialog) geöffnet worden ist
        /// </summary>
        /// <param name="window">Das zu prüfende Fenster</param>
        /// <returns><c>True</c>, wenn das Fenster modal ist, sonst <c>False</c></returns>
        public static bool IsModal(this Window window)
        {
            var field =
                typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic);

            return field !=null && (bool)field.GetValue(window);
        }

        [DllImport("user32.dll")]
        private static extern void FlashWindow(IntPtr a, bool b);

        /// <summary>
        /// Flashes the windows icon in the task bar
        /// </summary>
        /// <param name="win"></param>
        public static void FlashWindow(this Window win)
        {
            // Flash the window's icon!
            var windowHandle = new WindowInteropHelper(win).Handle;
            FlashWindow(windowHandle, true);
        }
    }
}