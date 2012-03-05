using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Echo.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        public ModelBase()
        {
            IsValidationEnforced = false;
        }

        public void SafeNotify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        public virtual void RaisePropertyChangedEvent(string propertyName)
        {
            // Exit if no subscribers
            if (PropertyChanged == null) return;
            // Raise event
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }
        #endregion



        private bool _IsValidationEnforced;
        public bool IsValidationEnforced
        {
            get { return _IsValidationEnforced; }
            set { _IsValidationEnforced = value; }
        }
    }
}