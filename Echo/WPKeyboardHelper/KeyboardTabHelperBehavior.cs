// Keyboard helper from pauliom.wordpress.com, @pauliom
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
//using Microsoft.Expression.Interactivity;
using System.Windows.Interactivity;

namespace WPKeyboardHelper
{
    public class KeyboardTabHelperBehavior : Behavior<UIElement>
    {
        private KeyboardHelper keyboardHelper;
        protected override void OnAttached()
        {
            base.OnAttached();     
            keyboardHelper = new KeyboardHelper(this.AssociatedObject);
            this.AssociatedObject.KeyDown += new KeyEventHandler(AssociatedObject_KeyDown);
        }

        void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                keyboardHelper.HandleReturnKey();
            }
        }

       
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.KeyDown-=AssociatedObject_KeyDown;

        }
    }
}
