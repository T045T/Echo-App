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
using System.Windows.Interactivity;

namespace Echo.Helpers
{
    public class TextBoxUpdateBehavior : Behavior<TextBox>
    {
        public TextBoxUpdateBehavior() { }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, TextChangedEventArgs args)
        {
            var binding = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }
    }
}

