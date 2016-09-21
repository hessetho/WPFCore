// ==========================================================================
// <summary>
// WpfHelper: Sammlung von Hilfsfunktionen und Klassen zur Verwendung mit WPF
// </summary>
//
// $Rev: 134 $
// $Id: EnumerableExtensions.cs 134 2011-05-26 14:48:03Z  $
// 
// <copyright file="EnumerableExtensions.cs" company="ICEP GmbH">
//      2009-2015 ICEP GmbH, T. Hesse
// </copyright>
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WPFCore.Helper
{
    /// <summary>
    ///     Extensions für Enumerationen
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Gibt alle Elemente einer Aufzählung im Debug-Fenster aus
        /// </summary>
        /// <remarks>
        ///     http://www.remondo.net/calculate-mean-median-mode-averages-csharp/
        /// </remarks>
        /// <param name="enumerable"></param>
        public static void Dump(this IEnumerable<object> enumerable)
        {
            foreach (var obj in enumerable)
                Debug.WriteLine(obj.ToString());
        }

        /// <summary>
        ///     Arithmetischer Mittelwert, Durchschnitt
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<double> list)
        {
            return list.Average(); // :-)
        }

        /// <summary>
        ///     Median.
        /// </summary>
        /// <remarks>
        ///     The median divides the ordered sequence of numbers in an upper and a lower section.
        ///     It takes the value in the middle of the list. If the list has an even number of
        ///     values it takes the mean average of the two values in the middle of the sequence.
        ///     http://de.wikipedia.org/wiki/Median
        ///     http://www.remondo.net/calculate-mean-median-mode-averages-csharp/
        /// </remarks>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double Median(this IEnumerable<double> list)
        {
            var orderedList = list
                .OrderBy(numbers => numbers)
                .ToList();

            var listSize = orderedList.Count;
            double result;

            if (listSize%2 == 0) // even
            {
                var midIndex = listSize/2;
                result = ((orderedList.ElementAt(midIndex - 1) +
                    orderedList.ElementAt(midIndex))/2);
            }
            else // odd
            {
                var element = (double) listSize/2;
                element = Math.Round(element, MidpointRounding.AwayFromZero);

                result = orderedList.ElementAt((int) (element - 1));
            }

            return result;
        }

        /// <summary>
        ///     Modus, Modalwert
        /// </summary>
        /// <remarks>
        ///     The mode gives us the value that is the most frequent in a given sequence of numbers.
        ///     If there are more than one we get a list of mode values.
        ///     http://de.wikipedia.org/wiki/Modus_%28Statistik%29
        ///     http://www.remondo.net/calculate-mean-median-mode-averages-csharp/
        /// </remarks>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<double> Modes(this IEnumerable<double> list)
        {
            var modesList = list
                .GroupBy(values => values)
                .Select(valueCluster =>
                    new
                    {
                        Value = valueCluster.Key,
                        Occurrence = valueCluster.Count()
                    })
                .ToList();

            var maxOccurrence = modesList
                .Max(g => g.Occurrence);

            return modesList
                .Where(x => x.Occurrence == maxOccurrence && maxOccurrence > 1) // Thanks Rui!
                .Select(x => x.Value);
        }

        public static double StdDevP(this IEnumerable<int> source)
        {
            return StdDevLogic(source, 0);
        }

        public static double StdDevP(this IEnumerable<double> source)
        {
            return StdDevLogic(source, 0);
        }

        public static double StdDevP(this IEnumerable<float> source)
        {
            return StdDevLogic(source, 0);
        }

        /// <summary>
        ///     Standardabweichung
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double StdDev(this IEnumerable<int> source)
        {
            return StdDevLogic(source);
        }

        /// <summary>
        ///     Standardabweichung
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double StdDev(this IEnumerable<double> source)
        {
            return StdDevLogic(source);
        }

        /// <summary>
        ///     Standardabweichung
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float StdDev(this IEnumerable<float> source)
        {
            return StdDevLogic(source);
        }

        /// <summary>
        ///     Standardabweichung
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static double StdDev<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return StdDevLogic(source, selector);
        }

        /// <summary>
        ///     Standardabweichung
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static double StdDev<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return StdDevLogic(source, selector);
        }

        /// <summary>
        ///     Standardabweichung, Berechnungslogik für double's
        /// </summary>
        /// <param name="source"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static double StdDevLogic(this IEnumerable<double> source, int buffer = 1)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var data = source.ToList();
            var average = data.Average();
            var differences = data.Select(u => Math.Pow(average - u, 2.0)).ToList();
            return Math.Sqrt(differences.Sum()/(differences.Count() - buffer));
        }

        private static double StdDevLogic<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return StdDevLogic(source.Select(selector));
        }

        private static double StdDevLogic<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return StdDevLogic(source.Select(selector));
        }


        public static double Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return Variance(source.Select(selector).Select(n => (double)n));
        }

        /// <summary>
        ///     Standardabweichung, Berechnungslogik für int's
        /// </summary>
        /// <param name="source"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static double StdDevLogic(this IEnumerable<int> source, int buffer = 1)
        {
            return StdDevLogic(source.Select(x => (double) x), buffer);
        }

        /// <summary>
        ///     Standardabweichung, Berechnungslogik für float's
        /// </summary>
        /// <param name="source"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static float StdDevLogic(this IEnumerable<float> source, int buffer = 1)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var data = source.ToList();
            var average = data.Average();
            var differences = data.Select(u => Math.Pow(average - u, 2.0)).ToList();

            return (float) Math.Sqrt(differences.Sum()/(differences.Count() - buffer));
        }


        /// <summary>
        /// Varianz.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static double Variance(this IEnumerable<double> source, int buffer = 1)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var data = source.ToList();
            var average = data.Average();
            var differences = data.Select(u => Math.Pow(average - u, 2.0)).ToList();

            return differences.Sum() / (differences.Count() - buffer);
        }

        /// <summary>
        /// Varianz.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static float Variance(this IEnumerable<float> source, int buffer = 1)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var data = source.ToList();
            var average = data.Average();
            var differences = data.Select(u => Math.Pow(average - u, 2.0)).ToList();

            return (float) (differences.Sum() / (differences.Count() - buffer));
        }
    }
}