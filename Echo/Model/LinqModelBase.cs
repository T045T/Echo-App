using System.ComponentModel;
using Caliburn.Micro;

namespace Echo.Model
{
    public class LinqModelBase : PropertyChangedBase, INotifyPropertyChanging
    {
        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

    }
}
