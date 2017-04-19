using System;
using System.Linq;
using System.Windows;
using WPFCore.ViewModelSupport;

namespace WPFCore.Data.FlexData
{
    public class ProxyProperty : ViewModelCore, IComparable
    {
        private readonly FlexRow row;

        private readonly string qualifiedProperty;
        private readonly string displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyProperty"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="qualifiedProperty">The qualified property.</param>
        /// <param name="displayName">The display name.</param>
        public ProxyProperty(FlexRow row, string qualifiedProperty, string displayName)
        {
            this.row = row;
            this.qualifiedProperty = qualifiedProperty;
            this.displayName = displayName;
        }

        /// <summary>
        /// Gets the value of this <c>ProxyProperty</c> instance.
        /// </summary>
        /// <returns></returns>
        public object Value
        {
            get 
            {
                return GetValue(this.row, this.qualifiedProperty);
            }
        }

        /// <summary>
        ///     returns the cell's value, formatted if available
        /// </summary>
        public string FormattedValue
        {
            get
            {
                if (this.Value == null)
                    return "";

                var tp = this.Value.GetType();

                if (tp == typeof(double))
                    return ((double)this.Value).ToString(this.StringFormat);

                if (tp == typeof(int))
                    return ((int)this.Value).ToString(this.StringFormat);

                if (tp == typeof(Int16))
                    return ((Int16)this.Value).ToString(this.StringFormat);

                if (tp == typeof(DateTime))
                    return ((DateTime)this.Value).ToString(this.StringFormat);

                if (tp == typeof(string))
                    return (string)this.Value;

                if (tp == typeof(Boolean))
                    return ((Boolean)this.Value) ? "Yes" : "No";

                return string.Format("!!unhandled data type: {0}!!", tp);
            }
        }

        /// <summary>
        /// Gets the qualified property.
        /// </summary>
        public string QualifiedProperty
        {
            get { return this.qualifiedProperty; }
        }

        /// <summary>
        /// Gets or sets the string format.
        /// </summary>
        public string StringFormat { get; set; }

        /// <summary>
        ///     Returns the proper alignment to display the expressions value
        /// </summary>
        /// <remarks>
        ///     The alignment depends on the type of the expressions value. It'll be set to "Left" for
        ///     texts, and to "Right" for all other data types.
        /// </remarks>
        public TextAlignment ValueAlignment
        {
            get
            {
                if (this.Value!=null && this.Value.GetType() == typeof(string))
                    return TextAlignment.Left;

                return TextAlignment.Right;
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName
        {
            get { return this.displayName; }
        } 

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Returns the value of a "proxied" cell (identified by <paramref name="qualifiedProperty"/>)
        /// in a <paramref name="row"/>.
        /// </summary>
        /// <param name="row">The row holding the value.</param>
        /// <param name="qualifiedProperty">The qualified property which identifies the cell within the row.</param>
        /// <returns></returns>
        public static object GetValue(FlexRow row, string qualifiedProperty)
        {
            var propertyList = qualifiedProperty.Split('.').ToList();
            object currentElement = row;

            // if the class name (= first element) matches the current element,
            // skip this
            if (currentElement.GetType().Name == propertyList[0])
                propertyList.RemoveAt(0);

            // now scan all property names of the qualified property
            foreach (var propertyName in propertyList)
            {
                try
                {
                    var propInfo = currentElement.GetType().GetProperties().First(pi => pi.Name == propertyName);
                    currentElement = propInfo.GetValue(currentElement, null);
                }
                catch
                {
                    currentElement = "#error: unknown virtual field name#";
                }
            }

            return currentElement == row ? null : currentElement;
        }

        public int CompareTo(object obj)
        {
            var other = (ProxyProperty)obj;
            var thisValue = this.Value;
            var otherValue = other.Value;

            // handle null values and #error#
            if (thisValue == null && otherValue == null)
                return 0;
            if (thisValue == null)
                return -1;
            if (otherValue == null)
                return 1;

            var thisType = this.Value.GetType();
            var otherType = other.Value.GetType();

            // check for identical types
            if (thisType != otherType)
                throw new InvalidOperationException(string.Format("Comparison failed. Incompatible data types: {0} / {1}", thisType, otherType));

            // string
            if (thisType == typeof(string))
                return String.Compare(((string)thisValue), ((string)otherValue), StringComparison.Ordinal);

            // DateTime
            if (thisType == typeof(DateTime))
                return ((DateTime)thisValue < (DateTime)otherValue)
                    ? -1
                    : ((DateTime)thisValue > (DateTime)otherValue) ? 1 : 0;

            // double
            if (thisType == typeof(double))
                return ((double)thisValue < (double)otherValue)
                    ? -1
                    : ((double)thisValue > (double)otherValue) ? 1 : 0;

            // decimal
            if (thisType == typeof(decimal))
                return ((decimal)thisValue < (decimal)otherValue)
                    ? -1
                    : ((decimal)thisValue > (decimal)otherValue) ? 1 : 0;

            // int
            if (thisType == typeof(int))
                return ((int)thisValue < (int)otherValue)
                    ? -1
                    : ((int)thisValue > (int)otherValue) ? 1 : 0;

            // Int16
            if (thisType == typeof(Int16))
                return ((int)thisValue < (Int16)otherValue)
                    ? -1
                    : ((int)thisValue > (Int16)otherValue) ? 1 : 0;

            return 0;
        }
    }
}
