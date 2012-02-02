using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Echo.Helpers
{
    public class LongListCollection<T, TKey> : ObservableCollection<LongListItem<T, TKey>>
        where T : IComparable<T>
    {
        public LongListCollection()
        {
        }

        public LongListCollection(IEnumerable<T> items, Func<T, TKey> keySelector)
        {
            if (items == null)
                throw new ArgumentException("items");

            var groups = new Dictionary<TKey, LongListItem<T, TKey>>();

            foreach (var item in items.OrderBy(x => x))
            {
                var key = keySelector(item);

                if (groups.ContainsKey(key) == false)
                    groups.Add(key, new LongListItem<T, TKey>(key));

                groups[key].Add(item);
            }

            foreach (var value in groups.Values)
                this.Add(value);
        }
    }

    public class LongListItem<T, TKey> : ObservableCollection<T>
    {
        public LongListItem()
        {
        }

        public LongListItem(TKey key)
        {
            this.Key = key;
        }

        public TKey Key
        {
            get;
            set;
        }

        public bool HasItems
        {
            get
            {
                return Count > 0;
            }
        }
    }
}
