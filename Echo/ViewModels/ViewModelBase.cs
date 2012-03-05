using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Echo.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        /*
        #region INotifyPropertyChanging Members            
        public event PropertyChangingEventHandler PropertyChanging;
        #endregion
        */

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
        
        ///// <summary>
        ///// Raises the PropertyChanging event.
        ///// </summary>
        ///// <param name="propertyName">The name of the changing property.</param>
        //public virtual void RaisePropertyChangingEvent(string propertyName)
        //{
        //    // Exit if changes ignored
        //    if (IgnorePropertyChangeEvents) return;
        //    // Exit if no subscribers
        //    if (PropertyChanging == null) return;
        //    // Raise event
        //    var e = new PropertyChangingEventArgs(propertyName);
        //    PropertyChanging(this, e);
        //}
        #endregion
    }
}
