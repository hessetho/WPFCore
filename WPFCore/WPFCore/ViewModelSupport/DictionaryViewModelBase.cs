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


        public new void Add(TValue item)
        {
            throw new InvalidOperationException("Use of DictionaryViewModelBase.Add(TValue) is not allowed. Use DictionaryViewModelBase.Add(TKey, TValue) instead");
        }

        public new void Remove(TValue item)
        {
            throw new InvalidOperationException("Use of DictionaryViewModelBase.Remove(TValue) is not allowed. Use DictionaryViewModelBase.Remove(TKey) instead");
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
    }
}
