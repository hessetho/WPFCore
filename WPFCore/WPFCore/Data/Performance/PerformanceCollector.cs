using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace WPFCore.Data.Performance
{
    public class PerformanceCollector
    {
        private readonly object lockObj = new object();

        private readonly Dictionary<string, Dictionary<string, PerformanceItem>> performanceCategories =
            new Dictionary<string, Dictionary<string, PerformanceItem>>();

        public CategoryDictionary PerformanceCategories
        {
            get
            {
                var d = new CategoryDictionary();

                foreach (var category in this.performanceCategories)
                    d.Add(category.Key, category.Value.Select(c => c.Value).ToList());

                return d;
            }
            set
            {
                this.performanceCategories.Clear();
                foreach (var cat in value)
                {
                    this.performanceCategories.Add(cat.Key, new Dictionary<string, PerformanceItem>());
                    foreach (var pItem in cat.Value)
                    {
                        this.performanceCategories[cat.Key].Add(pItem.ItemName, pItem);
                    }
                }
            }
        }

        [XmlIgnore]
        public string Filename { get; private set; }

        internal void AddPerformanceItem(PerformanceItem pItem)
        {
            var pItems = this.GetPerformanceItems(pItem.Category);

            if (!pItems.ContainsKey(pItem.ItemName))
                pItems.Add(pItem.ItemName, pItem);
        }

        internal void RemovePerformanceItem(PerformanceItem pItem)
        {
            var pItems = this.GetPerformanceItems(pItem.Category);

            if (pItems.ContainsKey(pItem.ItemName))
                pItems.Remove(pItem.ItemName);
        }

        public void AddItemDuration(string category, string itemName, TimeSpan duration)
        {
            var pItem = this.GetPerformanceItem(category, itemName);
            pItem.Add(duration);
        }

        public PerformanceItem StartTiming(string category, string itemName)
        {
            var pItem = this.GetPerformanceItem(category, itemName);
            pItem.StartTiming();
            return pItem;
        }

        public void StopTiming(string category, string itemName)
        {
            var pItem = this.GetPerformanceItem(category, itemName);
            pItem.StopTiming();
        }

        public void Reset()
        {
            this.performanceCategories.Clear();
        }

        private Dictionary<string, PerformanceItem> GetPerformanceItems(string categoryName)
        {
            Monitor.Enter(this.lockObj);
            Dictionary<string, PerformanceItem> pItems;
            var category = string.IsNullOrEmpty(categoryName) ? "General" : categoryName;
            if (this.performanceCategories.TryGetValue(category, out pItems) == false)
            {
                pItems = new Dictionary<string, PerformanceItem>();
                this.performanceCategories.Add(category, pItems);
            }
            Monitor.Exit(this.lockObj);

            return pItems;
        }

        private PerformanceItem GetPerformanceItem(string category, string itemName)
        {
            var pItems = this.GetPerformanceItems(category);

            Monitor.Enter(this.lockObj);
            PerformanceItem pItem;
            if (pItems.TryGetValue(itemName, out pItem) == false)
            {
                pItem = new PerformanceItem(category, itemName);
                pItems.Add(itemName, pItem);
            }
            Monitor.Exit(this.lockObj);

            return pItem;
        }

        #region Load & Save

        private bool isInitialized;

        public void Save()
        {
            Debug.Assert(!string.IsNullOrEmpty(this.Filename));

            this.Save(this.Filename);
        }

        /// <summary>
        ///     Speichert die anwenderspezifischen Einstellungen in eine Datei
        /// </summary>
        /// <remarks>
        ///     Es wird nur dann gespeichert, wenn die Einstellungen vorher komplett
        ///     initialisiert worden sind. Das verhindert, dass Einstellungen verloren
        ///     gehen...
        /// </remarks>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            if (!this.isInitialized) return;
            var serializer = new XmlSerializer(typeof (PerformanceCollector));
            using (TextWriter tw = new StreamWriter(filename))
            {
                serializer.Serialize(tw, this);
            }
        }

        /// <summary>
        ///     Liest die anwenderspezifischen Einstellungen ein, sofern vorhanden.
        ///     Andernfalls wird eine neue (unbearbeitete) Instanz geliefert.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static PerformanceCollector Load(string filename)
        {
            PerformanceCollector collector = null;

            // Wenn die gewünschte Datei (nun) da ist, wird sie gelesen, ansonsten gibt's neue (leere) Instanz
            if (File.Exists(filename))
            {
                var serializer = new XmlSerializer(typeof (PerformanceCollector));
                using (TextReader tr = new StreamReader(filename))
                    collector = (PerformanceCollector) serializer.Deserialize(tr);
            }
            else
                collector = new PerformanceCollector();

            collector.Filename = filename;
            collector.isInitialized = true;
            return collector;
        }

        #endregion Load & Save
    }
}