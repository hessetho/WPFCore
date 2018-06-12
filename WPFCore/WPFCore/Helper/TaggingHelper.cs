using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFCore.Helper
{
    /// <summary>
    /// This (static) helper class provides methods to add multiple elements to a control's Tag property
    /// </summary>
    public static class TaggingHelper
    {
        /// <summary>
        /// Add a value to a FrameworkElement.Tag
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="tagValue">The tag value.</param>
        public static void SetTag(this FrameworkElement element, object tagValue)
        {
            if (element.Tag == null || !(element.Tag is List<object>))
            {
                element.Tag = new List<object> { tagValue };
            }
            else
            {
                // Okay, we already have a tag list
                var list = (List<object>)element.Tag;

                // if we already have an object of the same type in this list, remove this
                var oldTagValue = list.FirstOrDefault(e => e.GetType() == tagValue.GetType());
                if (oldTagValue != null)
                    list.Remove(oldTagValue);

                // add our tag value to the list
                list.Add(tagValue);
            }
        }

        /// <summary>
        /// Gets the tag of a specific type (or <c>null</c> if the type is not attached)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static T GetTag<T>(this FrameworkElement element)
        {
            if (element.Tag == null)
                return default(T);
            else if (element.Tag is List<object>)
            {
                var list = (List<object>)element.Tag;
                return (T)list.OfType<T>().FirstOrDefault();
            }
            else if (element.Tag.GetType() == typeof(T))
                return (T)element.Tag;

            return default(T);
        }
    }
}
