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

namespace WPKeyboardHelper
{
    
    public class KeyboardDependency : DependencyObject
    {
        private static KeyboardHelper keyboardHelper;
        
        private KeyboardDependency() { }

        static KeyboardDependency() { }

        public static readonly DependencyProperty IsTabEnabledProperty = DependencyProperty.RegisterAttached(
          "IsTabbingEnabled",
          typeof(bool),
          typeof(KeyboardDependency), new PropertyMetadata(OnIsTabbingEnabledChanged)
          );

        static void OnIsTabbingEnabledChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement fe = target as FrameworkElement;
            if (fe == null)
            {
                keyboardHelper = null;
            }
            else
            {
                keyboardHelper = new KeyboardHelper(fe);
                fe.KeyDown += new KeyEventHandler(fe_KeyDown);
            }

        }

        static void fe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                keyboardHelper.HandleReturnKey();
            }
        }


        public static bool GetIsTabEnabled(DependencyObject source)
        {
            return (bool)source.GetValue(IsTabEnabledProperty);
        }

        public static void SetIsTabEnabled(DependencyObject source, bool value) { source.SetValue(IsTabEnabledProperty, value); }

    }
}
