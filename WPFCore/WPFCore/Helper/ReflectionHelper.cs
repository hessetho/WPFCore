using System;
using System.Linq;

namespace WPFCore.Helper
{
    public static class ReflectionHelper
    {

        /// <summary>
        /// Returns the value of a nested property (identified by <paramref name="qualifiedProperty"/>)
        /// in a <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element holding the value.</param>
        /// <param name="qualifiedProperty">The qualified property.</param>
        /// <returns></returns>
        public static object GetValueNested(object element, string qualifiedProperty)
        {
            var propertyList = qualifiedProperty.Split('.').ToList();
            object currentElement = element;

            // if the class name (= first element) matches the current element, skip this
            if (currentElement.GetType().Name == propertyList[0])
                propertyList.RemoveAt(0);

            // now scan all property names of the qualified property
            foreach (var propertyName in propertyList)
            {
                var propInfo = currentElement.GetType().GetProperties().First(pi => pi.Name == propertyName);
                currentElement = propInfo.GetValue(currentElement, null);
            }

            return currentElement == element ? null : currentElement;
        }

        public static void DumpProperties(Type t)
        {
            var propInfo = t.GetProperties();

            Console.WriteLine(string.Format("Properties of the type {0}", t.Name));
            foreach (var prop in propInfo)
            {
                Console.WriteLine(string.Format("{0} - {1}", prop.Name, prop.PropertyType.Name));
            }
        }
    }
}
