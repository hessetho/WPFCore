using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Point = System.Windows.Point;

namespace WPFCore.Helper
{
    /// <summary>
    /// Liefert Informationen über angeschlossene Bildschirme sowie deren physikalischen Eigenschaften.
    /// </summary>
    [DebuggerStepThrough]
    public class WpfScreen
    {
        /// <summary>
        /// Feld zur Speicherung des aktuellen Bildschirms dieser Instanz.
        /// </summary>
        private readonly Screen screen;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="WpfScreen"/>-Klasse.
        /// </summary>
        /// <param name="screen">Der Bildschirm</param>
        internal WpfScreen(Screen screen)
        {
            this.screen = screen;
        }

        /// <summary>
        /// Liefert den primären Bildschirm
        /// </summary>
        /// <value>Der primäre Bildschirm.</value>
        public static WpfScreen Primary
        {
            get { return new WpfScreen(Screen.PrimaryScreen); }
        }

        /// <summary>
        /// Liefert die Koordinaten und das Anzeigerechteck eines Ausgabegeräts
        /// </summary>
        public Rect DeviceBounds
        {
            get { return GetRect(this.screen.Bounds); }
        }

        /// <summary>
        /// Liefert den Anzeigebereich eines Ausgabegeräts
        /// </summary>
        public Rect WorkingArea
        {
            get { return GetRect(this.screen.WorkingArea); }
        }

        /// <summary>
        /// Liefert ein Kennzeichen, welches das Primäre Anzeigegerät identifiziert.
        /// </summary>
        public bool IsPrimary
        {
            get { return this.screen.Primary; }
        }

        /// <summary>
        /// Liefert den Namen eines Anzeigegeräts
        /// </summary>
        public string DeviceName
        {
            get
            {
                var pos = this.screen.DeviceName.IndexOf('\0');
                return this.screen.DeviceName.Substring(0, pos == -1 ? this.screen.DeviceName.Length : pos);
            }
        }

        /// <summary>
        /// Liefert alle angeschlossenen Bildschirme
        /// </summary>
        /// <returns>Liste der Bildschirme vom Typ <see cref="WpfScreen"/></returns>
        public static IEnumerable<WpfScreen> AllScreens()
        {
            return Screen.AllScreens.Select(screen => new WpfScreen(screen));
        }

        /// <summary>
        /// Liefert den Bildschirm zu einem Fenster
        /// </summary>
        /// <param name="window">Das Fenster.</param>
        /// <returns>Der Bildschirm</returns>
        public static WpfScreen GetScreenFrom(Window window)
        {
            var windowInteropHelper = new WindowInteropHelper(window);
            var screen = Screen.FromHandle(windowInteropHelper.Handle);
            var wpfScreen = new WpfScreen(screen);
            return wpfScreen;
        }

        /// <summary>
        /// Liefert den Bildschirm zu einem Bildschirmpunkt
        /// </summary>
        /// <param name="point">Der Punkt.</param>
        /// <returns>Der Bildschirm</returns>
        public static WpfScreen GetScreenFrom(Point point)
        {
            var x = (int) Math.Round(point.X);
            var y = (int) Math.Round(point.Y);

            // are x,y device-independent-pixels ??
            var drawingPoint = new System.Drawing.Point(x, y);
            var screen = Screen.FromPoint(drawingPoint);
            var wpfScreen = new WpfScreen(screen);
            return wpfScreen;
        }

        /// <summary>
        /// Liefert den Bildschirm anhand seines internen Gerätenamens
        /// </summary>
        /// <param name="deviceName">Der interne Gerätename.</param>
        /// <returns>Der Bildschirm</returns>
        public static WpfScreen GetScreenName(string deviceName)
        {
            return AllScreens().FirstOrDefault(screen => screen.DeviceName.Equals(deviceName));
        }

        /// <summary>
        /// Konvertiert ein <see cref="Rectangle"/>
        /// in ein <see cref="System.Windows.Rect"/>.
        /// </summary>
        /// <param name="value">Ein <see cref="Rectangle"/></param>
        /// <returns>Ein <see cref="System.Windows.Rect"/></returns>
        private static Rect GetRect(Rectangle value)
        {
            // should x, y, width, hieght be device-independent-pixels ??
            return new Rect
                       {
                           X = value.X,
                           Y = value.Y,
                           Width = value.Width,
                           Height = value.Height
                       };
        }
    }
}