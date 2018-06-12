using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFCore.Helper
{
    /// <summary>
    /// Provides several useful helper functions
    /// </summary>
    public static class Toolbox
    {

        public static void DoEvents()
        {
            // does not work as intended: seems to occasionally stop the current program flow... --> commented out
            //if (Application.Current != null)
            //    Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new ThreadStart(delegate { }));
        }

        /// <summary>
        /// Liest eine String-Resource aus der aufrufenden Assembly.
        /// </summary>
        /// <remarks>
        /// Damit eine Resource (z.B. eine Textdatei) erkennt wird, muss diese dem Projekt
        /// als "Embedded Resource" hinzugefügt sein (Eigenschaft "Build Action" der Datei).
        /// </remarks>
        /// <param name="resourceName">Voll-Qualifizierter Name der angeforderten Resource.</param>
        /// <returns>Die ausgelesene resource als String.</returns>
        [DebuggerStepThrough]
        public static string ReadResourceStream(string resourceName)
        {
            var assembly = Assembly.GetCallingAssembly();
            return ReadResourceStream(assembly, resourceName);
        }

        /// <summary>
        /// Reads the specified resource stream from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">resourceName</exception>
        [DebuggerStepThrough]
        public static string ReadResourceStream(Assembly assembly, string resourceName)
        {
            string result;

            // liefert alle Resources:
            //assembly.GetManifestResourceNames()
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new ArgumentOutOfRangeException("resourceName", resourceName, string.Format("The requested resource does not exist in the assembly '{0}'.", assembly.GetName()));

                var textStreamreader = new StreamReader(stream);
                result = textStreamreader.ReadToEnd();
            }

            return result;
        }
        
        /// <summary>
        /// Prüft, ob die Anwendung aktuell im Design-Mode befindet.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsInDesignMode()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return true;

            if (Process.GetCurrentProcess().ProcessName.ToUpper().Equals("DEVENV"))
                return true;

            return false;
        }


        /// <summary>
        /// Gets the application folder for Navigator.net
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationFolder()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppContext.AppRegistryName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// Gets the referenced assemblies.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static List<string> GetReferencedAssemblies()
        {
            var result = new List<string>();

            var main = Assembly.GetEntryAssembly().GetName();
            result.Add(string.Format("{0} - {1}", main.Name, main.Version.ToString(4)));

            foreach (var asm in Assembly.GetEntryAssembly().GetReferencedAssemblies().OrderBy(n => n.Name))
            {
                var n = string.Format("{0} - {1}", asm.Name, asm.Version.ToString(4));
                result.Add(n);

                //Debug.WriteLine(n);
            }

            return result;
        }

        /// <summary>
        /// Returns a bitmap, which is identified by its resource name. The bitmap must be included in the project as a "Resource".
        /// </summary>
        /// <param name="imageResourceName"></param>
        /// <returns></returns>
        public static BitmapImage GetBitmap(string imageResourceName)
        {
            if (!imageResourceName.StartsWith("/"))
                imageResourceName = "/" + imageResourceName;

            var uri = new Uri(string.Format("pack://application:,,,/{0};component{1}", Assembly.GetCallingAssembly().GetName().Name, imageResourceName));
            return GetBitmap(uri);
        }

        /// <summary>
        /// Returns a bitmap, which is identified by a <see cref="Uri"/>
        /// </summary>
        /// <param name="imageUri"></param>
        /// <returns></returns>
        public static BitmapImage GetBitmap(Uri imageUri)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = imageUri;
            bmp.EndInit();
            return bmp;
        }

        /// <summary>
        /// Copies a UI element to the clipboard as an image.
        /// </summary>
        /// <param name="element">The element to copy.</param>
        public static void CopyToClipboard(this FrameworkElement element)
        {
            var width = element.ActualWidth;
            var height = element.ActualHeight;
            var bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            var dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen())
            {
                var vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }

            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }

#if DEBUG
        public static void DumpResources(Assembly assembly)
        {
            foreach (var name in assembly.GetManifestResourceNames())
                Debug.WriteLine(name);
        }
#endif
    }
}
