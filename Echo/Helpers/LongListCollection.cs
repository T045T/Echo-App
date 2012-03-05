using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Echo.Helpers
{
    public class LongListCollection<T> : ObservableCollection<LongListItem<T>>
    {
        public LongListCollection()
        {
        }

        private readonly string alphabet = "#abcdefghijklmnopqrstuvwxyz";
        private Func<T, string> keySelector;
        private Func<T, string> orderFunction;
        private Dictionary<string, LongListItem<T>> groups;

        public LongListCollection(IEnumerable<T> items, Func<T, string> keySelector, Func<T, string> orderFunction)
        {
            this.keySelector = keySelector;
            this.orderFunction = orderFunction;
            if (items == null)
                throw new ArgumentException("items");

            groups = new Dictionary<string, LongListItem<T>>();

            foreach (char c in alphabet)
                groups.Add(c.ToString(), new LongListItem<T>(c.ToString()));

            foreach (var item in items)
            {
                var key = keySelector(item);

                if (groups.ContainsKey(key) == false)
                    groups.Add(key, new LongListItem<T>(key));

                groups[key].Add(item);
            }

            foreach (var value in groups.Values)
            {
                value.OrderBy(orderFunction);
                this.Add(value);
            }
        }
    }

    public class LongListItem<T> : ObservableCollection<T>
    {
        public LongListItem()
        {
        }

        public LongListItem(string Title)
        {
            this.Title = Title;
        }

        public string Title
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
