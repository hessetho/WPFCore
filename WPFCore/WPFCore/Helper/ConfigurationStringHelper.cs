using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;

namespace WPFCore.Helper
{
    public static class ConfigurationStringHelper
    {
        /// <summary>
        /// Gets the configuration string for an object.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static string GetConfigurationString(object element)
        {
            return GetConfigurationString(element, null);
        }

        /// <summary>
        /// Gets the configuration string for an object.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="additionalData">Additional configuration items.</param>
        /// <returns></returns>
        public static string GetConfigurationString(object element, Dictionary<string, object> additionalData)
        {
            var config = string.Empty;
            var configItems = new Dictionary<string, object>();
            if (additionalData != null)
                foreach (var addItem in additionalData)
                    configItems.Add(addItem.Key, addItem.Value);

            // get all properties (public and private) with read/write access
            var properties = element.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                             .Where(p => p.CanWrite && p.CanRead);

            foreach (var property in properties)
            {
                // skip properties that do not have the "IsConfigurationParameter" attribute
                if (!property.IsDefined(typeof(IsConfigurationParameterAttribute), true))
                    continue;
                //if (property.IsDefined(typeof(BrowsableAttribute), true))
                //    continue;

                // add the (non-null) value of the property to the item list
                var value = property.GetValue(element, null);
                if ((value != null && !(value is string) ) || (value is string && !string.IsNullOrEmpty((string)value)))
                    configItems.Add(property.Name, value);
            }

            // now convert the items into a string
            // we're forcing en-US to have a unified string conversion for dates and numbers
            var currentCI = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            try
            {
                config = string.Join(";", configItems.Select(ci => string.Format("{0}={1}", ci.Key, ci.Value)));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCI;
            }

            return config;
        }

        /// <summary>
        /// Split a configuration string into its components and return these as a list of property name/value pairs
        /// </summary>
        /// <remarks>
        /// While a property name may only consist of letters and digits, a value may also contain "#", "." or ",".
        /// </remarks>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDefinitionItems(string configuration)
        {
            var configItems = new Dictionary<string, string>();
            var regex = new Regex(@"(?<name>\w*)=(?<value>[ +\-@#\.,\w]*)");

            var matches = regex.Matches(configuration);
            foreach (Match match in matches)
                configItems.Add(match.Groups["name"].Value, match.Groups["value"].Value);

            return configItems;
        }

        /// <summary>
        /// Applies a configuration string to an object. Returns those configuration items
        /// which could not be applied to a property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ApplyConfigurationString(object element, string configuration)
        {
            var configItems = GetDefinitionItems(configuration);

            using (var preservedCulture = CultureHelper.UseSpecificCulture("en-US"))
            {
                var properties = element.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                                .Where(p => p.CanWrite && p.CanRead);

                foreach (var property in properties)
                {
                    string configValue;
                    if (configItems.TryGetValue(property.Name, out configValue))
                    {
                        object value = null;
                        if (property.PropertyType == typeof(Brush))
                            value = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configValue));
                        if (property.PropertyType == typeof(Color))
                            value = (Color)ColorConverter.ConvertFromString(configValue);
                        else
                            value = Convert.ChangeType(configValue, property.PropertyType);

                        property.SetValue(element, value, null);

                        configItems.Remove(property.Name);
                    }
                }
            }

            // return the remaining configuration items
            return configItems;
        }


        /// <summary>
        /// Applies a single configuration value to a specific property of the target element
        /// </summary>
        /// <remarks>
        /// No checks are done if the property really exists!
        /// An explicit conversion is done for the <see cref="Brush"/> and the <see cref="Color"/> types.
        /// </remarks>
        /// <param name="element">target element</param>
        /// <param name="propertyName">name of the property</param>
        /// <param name="configValue">configuration value</param>
        public static void ApplyConfigurationValue(object element, string propertyName, string configValue)
        {
            var property = element.GetType().GetProperty(propertyName);

            object value = null;

            if (property.PropertyType == typeof(Brush))
                value = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configValue));
            if (property.PropertyType == typeof(Color))
                value = (Color)ColorConverter.ConvertFromString(configValue);
            else
                value = Convert.ChangeType(configValue, property.PropertyType);

            property.SetValue(element, value, null);
        }
    }
}
