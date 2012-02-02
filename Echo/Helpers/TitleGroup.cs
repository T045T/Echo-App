using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;

namespace Echo.Helpers
{
    //public class TitleGroup<T> : ObservableCollection<T>
    //{
    //    public string Title { get; set; }

    //    public TitleGroup(string name, IEnumerable<T> items) : base(items)
    //    {
    //        this.Title = name;
    //    }

    //    public bool HasItems
    //    {
    //        get { return Items.Count > 0; }
    //    }

    //    public Brush GroupBackgroundBrush
    //    {
    //        get
    //        {
    //            if (HasItems)
    //                return (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
    //            else
    //                return (SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"];
    //        }
    //    }
    //}


    public class TitleGroup<T> : IEnumerable<T>, INotifyPropertyChanged
    {
        public TitleGroup(string name, IEnumerable<T> items)
        {
            this.Title = name;
            this.Items = new List<T>(items);
        }

        public override bool Equals(object obj)
        {
            TitleGroup<T> that = obj as TitleGroup<T>;

            return (that != null) && (this.Title.Equals(that.Title));
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }

        public bool HasItems
        {
            get { return Items.Count > 0; }
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (value != _Title)
                {
                    _Title = value;
                    RaisePropertyChangedEvent("Title");
                    RaisePropertyChangedEvent("TitleGroup");
                }
            }
        }

        private List<T> _Items;
        public List<T> Items
        {
            get { return _Items; }
            set
            {
                if (value != _Items)
                {
                    _Items = value;
                    RaisePropertyChangedEvent("Items");
                    RaisePropertyChangedEvent("TitleGroup");
                }
            }
        }

        public void Remove(T Item)
        {
            Items.Remove(Item);
            RaisePropertyChangedEvent("Items");
            RaisePropertyChangedEvent("TitleGroup");
        }

        public void Add(T Item)
        {
            Items.Add(Item);
            RaisePropertyChangedEvent("Items");
            RaisePropertyChangedEvent("TitleGroup");
        }

        public void OrderBy(Comparison<T> comp)
        {
            Items.Sort(comp);
        }

        public Brush GroupBackgroundBrush
        {
            get
            {
                if (HasItems)
                    return (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                else
                    return (SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"];
            }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Administrative Properties
        /// <summary>
        /// Whether the view model should ignore property-change events.
        /// </summary>
        public virtual bool IgnorePropertyChangeEvents { get; set; }
        #endregion
        #region Public Methods
        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        public virtual void RaisePropertyChangedEvent(string propertyName)
        {
            // Exit if changes ignored
            if (IgnorePropertyChangeEvents) return;
            // Exit if no subscribers
            if (PropertyChanged == null) return;
            // Raise event
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }
        #endregion Public Methods
    }
}
