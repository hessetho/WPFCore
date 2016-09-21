using System;
using System.Collections.Generic;

namespace WPFCore.Data.Performance
{
    public static class PerformanceCenter
    {
        private static readonly PerformanceCollector collector = new PerformanceCollector();

        public static Dictionary<string, List<PerformanceItem>> PerformanceCategories
        {
            get { return collector.PerformanceCategories; }
        }

        internal static void AddPerformanceItem(PerformanceItem pItem)
        {
            collector.AddPerformanceItem(pItem);
        }

        internal static void RemovePerformanceItem(PerformanceItem pItem)
        {
            collector.RemovePerformanceItem(pItem);
        }

        public static void AddItemDuration(string category, string itemName, TimeSpan duration)
        {
            collector.AddItemDuration(category, itemName, duration);
        }

        public static PerformanceItem StartTiming(string category, string itemName)
        {
            return collector.StartTiming(category, itemName);
        }

        public static void StopTiming(string category, string itemName)
        {
            collector.StopTiming(category, itemName);
        }

        public static void Reset()
        {
            collector.Reset();
        }

        public static PerformanceCollector GetInternalCollector()
        {
            return collector;
        }
    }
}