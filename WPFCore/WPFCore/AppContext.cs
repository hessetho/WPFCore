using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore
{
    public static class AppContext
    {
        static AppContext()
        {
            AppContext.CompanyName = "ICEP GmbH";
            AppContext.AppRegistryName = "CommonAppSpace";
        }

        public static void SetAppContext(string companyName, string appRegistryName)
        {
            AppContext.CompanyName = companyName;
            AppContext.AppRegistryName = appRegistryName;
        }

        public static string CompanyName { get; private set; }
        public static string AppRegistryName { get; private set; }


        /// <summary>
        /// Liefert den Registry-Pfad der Anwendung unter Software
        /// </summary>
        /// <remarks>
        /// Der Pfad wird aus dem Namen der Firma und dem Namen der Anwendung zusammen gesetzt.
        /// Standardmäßig ist dies "Software\Company\Application"
        /// </remarks>
        public static string RegistryPath
        {
            get { return string.Format(@"Software\{0}\{1}\", CompanyName, AppRegistryName); }
        }

    }
}
