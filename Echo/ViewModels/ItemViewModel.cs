using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Echo.ViewModels;

namespace Echo
{
    public class ItemViewModel : ViewModelBase
    {
        private string _lineOne;
        /// <summary>
        /// Eigenschaft des Beispiel-ViewModel; diese Eigenschaft wird in der Ansicht verwendet, um deren Wert unter Verwendung einer Bindung anzuzeigen.
        /// </summary>
        /// <returns></returns>
        public string LineOne 
        {
            get 
            {
                return _lineOne;
            }
            set 
            {
                _lineOne = value;
                RaisePropertyChangedEvent("LineOne");
            }
        }
        
        private string _lineTwo;
        /// <summary>
        /// Eigenschaft des Beispiel-ViewModel; diese Eigenschaft wird in der Ansicht verwendet, um deren Wert unter Verwendung einer Bindung anzuzeigen.
        /// </summary>
        /// <returns></returns>
        public string LineTwo
        {
            get
            {
                return _lineTwo;
            }
            set
            {
                _lineTwo = value;
                RaisePropertyChangedEvent("LineTwo");
            }
        }

        private string _lineThree;
        /// <summary>
        /// Eigenschaft des Beispiel-ViewModel; diese Eigenschaft wird in der Ansicht verwendet, um deren Wert unter Verwendung einer Bindung anzuzeigen.
        /// </summary>
        /// <returns></returns>
        public string LineThree
        {
            get
            {
                return _lineThree;
            }
            set
            {
                _lineThree = value;
                RaisePropertyChangedEvent("LineThree");
            }
        }
    }
}