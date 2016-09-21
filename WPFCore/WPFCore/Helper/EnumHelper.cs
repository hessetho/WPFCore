using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace WPFCore.Helper
{
    /// <summary>
    /// Hilfsfunktionen für <see cref="Enum"/>s
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Prüft, ob ein Typ eine Enumeration ist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="withFlags"></param>
        private static void CheckIsEnum<T>(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            {
                throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute",
                                                          typeof(T).FullName));
            }
        }

        /// <summary>
        ///     Prüft, ob ein Flag gesetzt ist
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        /// <summary>
        ///     Liefert eine Liste der gesetzten Flags
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(true);
            return Enum.GetValues(typeof(T)).Cast<T>().Where(flag => value.IsFlagSet(flag));
        }

        /// <summary>
        ///     Setzt die übergebenen Flags, alle anderen werden entfernt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="flags"></param>
        /// <param name="on"></param>
        /// <returns></returns>
        public static T SetFlags<T>(this T value, T flags, bool on) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flags);
            if (on)
                lValue |= lFlag;
            else
                lValue &= (~lFlag);
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        /// <summary>
        ///     Setzt die übergebenen Flags
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static T SetFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, true);
        }

        /// <summary>
        ///     Löscht die übergebenen Flags
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static T ClearFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, false);
        }

        /// <summary>
        ///     Kombiniert die übergebenen Flags mit den vorhandenen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = flags.Select(flag => Convert.ToInt64(flag))
                               .Aggregate<long, long>(0, (current, lFlag) => current | lFlag);
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        /// <summary>
        ///     Liefert die Beschreibung zu gesetzten Flags (sofern eine solche anhand der DescriptionAttribute gesetzt ist)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(false);
            string name = Enum.GetName(typeof(T), value);
            if (name != null)
            {
                FieldInfo field = typeof(T).GetField(name);
                if (field != null)
                {
                    var attr =
                        Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                        return attr.Description;

                    return value.ToString();
                }
            }
            return null;
        }
    }
}
