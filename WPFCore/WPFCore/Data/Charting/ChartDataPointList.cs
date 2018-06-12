using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.Charting
{
    public class ChartDataPointList<TKey, TPoint> : IReadOnlyList<TPoint>, INotifyCollectionChanged where TPoint : ChartDataPoint, new()
    {
        private readonly SortedDictionary<TKey, TPoint> dataPoints = new SortedDictionary<TKey, TPoint>();
        private readonly string[] dpPropertyNames;

        public ChartDataPointList(List<string> dataPointPropertyNames)
        {
            this.dpPropertyNames = dataPointPropertyNames.ToArray();
        }

        public ChartDataPointList(params string[] dataPointPropertyNames)
        {
            this.dpPropertyNames = dataPointPropertyNames;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public readonly object SynObject = new object();

        private TPoint NewDataPoint(TKey key)
        {

            var chartDp = (TPoint)Activator.CreateInstance(typeof(TPoint));
            chartDp.Initialize(this.dpPropertyNames);

            this.dataPoints.Add(key, chartDp);
            return chartDp;
        }

        /// <summary>
        /// Liefert den Datenpunkt für den Schlüssel <paramref name="key"/>. 
        /// Wenn <c>key</c> nicht vorhanden ist, wird ein neuer <see cref="ChartDataPoint"/> erzeugt.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TPoint this[TKey key]
        {
            get
            {
                this.dataPoints.TryGetValue(key, out var chartDp);
                if (chartDp == null)
                    chartDp = this.NewDataPoint(key);

                return chartDp;
            }
        }

        /// <summary>
        /// Liefert alle Datenpunkte
        /// </summary>
        public List<TPoint> DataPoints
        {
            get
            {
                return this.dataPoints.Values.ToList();
            }
        }

        public void SignalRefresh()
        {
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #region IReadOnlyList
        int IReadOnlyCollection<TPoint>.Count
        {
            get
            {
                return this.dataPoints.Count;
            }
        }

        TPoint IReadOnlyList<TPoint>.this[int index]
        {
            get
            {
                return this.dataPoints.Values.ElementAt(index);
            }
        }

        IEnumerator<TPoint> IEnumerable<TPoint>.GetEnumerator()
        {
            return this.dataPoints.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dataPoints.Values.GetEnumerator();
        }
        #endregion IReadOnlyList
    }
}
