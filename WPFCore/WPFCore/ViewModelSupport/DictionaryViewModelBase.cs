using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.ViewModelSupport
{
    public class DictionaryViewModelBase<TKey, TValue> : CollectionViewModelBase<TValue> where TValue : ViewModelBase
    {
        private readonly Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

        /// <summary>
        /// NOT SUPPORTED. Use Add(TKey, TValue) instead.
        /// </summary>
        /// <param name="item"></param>
        [Obsolete("Do not use!")]
        public new void Add(TValue item)
        {
            throw new InvalidOperationException("Use of DictionaryViewModelBase.Add(TValue) is not allowed. Use DictionaryViewModelBase.Add(TKey, TValue) instead");
        }

        /// <summary>
        /// NOT SUPPORTED. Use Remove(TKey) instead.
        /// </summary>
        /// <param name="item"></param>
        [Obsolete("Do not use!")]
        public new void Remove(TValue item)
        {
            throw new InvalidOperationException("Use of DictionaryViewModelBase.Remove(TValue) is not allowed. Use DictionaryViewModelBase.Remove(TKey) instead");
        }

        public new void Clear()
        {
            this.dict.Clear();
            base.Clear();
        }

        public void Add(TKey key, TValue item)
        {
            dict.Add(key, item);
            base.Add(item);
        }

        public TValue this[TKey key]
        {
            get
            {
                return dict[key];
            }
        }

        public void Remove(TKey key)
        {
            var item = dict[key];
            base.Remove(item);
            dict.Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }
    }
}
